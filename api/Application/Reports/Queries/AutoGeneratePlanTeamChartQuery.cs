using Application.Cqrs.Queris;
using Application.Reports.DTOs;
using Common.Utilities;
using Data.Contracts;
using Entities.Plans;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Reports.Queries
{
    public record AutoGeneratePlanTeamChartQuery(
      int TopPlans = 3,
      int Periods = 6,
      PerformanceGroupingType Grouping = PerformanceGroupingType.Weekly)
      : IQuery<ServiceResult<List<TeamPerformanceChartSeriesDTO>>>;

    public class AutoGeneratePlanTeamChartQueryHandler(
    IRepository<UserPlanAnswer> answerRepo,
    IRepository<PointTransaction> pointRepo,
    IRepository<Plan> planRepo
) : IQueryHandler<AutoGeneratePlanTeamChartQuery, ServiceResult<List<TeamPerformanceChartSeriesDTO>>>
    {
        public async Task<ServiceResult<List<TeamPerformanceChartSeriesDTO>>> Handle(AutoGeneratePlanTeamChartQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow.Date;
            var fromDate = request.Grouping switch
            {
                PerformanceGroupingType.Daily => now.AddDays(-request.Periods + 1),
                PerformanceGroupingType.Weekly => now.AddDays(-7 * (request.Periods - 1)),
                PerformanceGroupingType.Monthly => now.AddMonths(-request.Periods - 1),
                _ => now
            };

            // استخراج پرکاربردترین PlanId ها
            var topPlans = await answerRepo.TableNoTracking
                .Where(a => a.CreatedAt >= fromDate)
                .GroupBy(a => a.PlanQuestion.PlanId)
                .OrderByDescending(g => g.Count())
                .Take(request.TopPlans)
                .Select(g => g.Key)
                .ToListAsync(cancellationToken);

            if (!topPlans.Any())
                return ServiceResult.Ok(new List<TeamPerformanceChartSeriesDTO>());

            var planTitles = await planRepo.TableNoTracking
                .Where(p => topPlans.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => new { p.Title, p.ImageUrl }, cancellationToken);

            var answers = await answerRepo.TableNoTracking.Include(t => t.UserPlan).Include(t => t.PlanQuestion)
                .Where(a => topPlans.Contains(a.PlanQuestion.PlanId) && a.CreatedAt.Date >= fromDate)
                .ToListAsync(cancellationToken);

            var userIds = answers.Select(a => a.UserPlan.UserId).Distinct().ToList();

            var points = await pointRepo.TableNoTracking
                .Where(p => userIds.Contains(p.UserId) && p.TransactionType == PointTransactionType.Earn && !p.IsReverted && p.TransactionDate.Date >= fromDate)
                .ToListAsync(cancellationToken);

            var result = new List<TeamPerformanceChartSeriesDTO>();

            foreach (var planId in topPlans)
            {
                var planAnswers = answers.Where(a => a.PlanQuestion.PlanId == planId).ToList();
                var planUserIds = planAnswers.Select(a => a.UserPlan.UserId).Distinct().ToList();

                var team = new TeamPerformanceChartSeriesDTO
                {
                    PlanName = planTitles[planId].Title,
                    PlanImageUrl = planTitles[planId].ImageUrl,
                    DataPoints = new()
                };

                for (int i = 0; i < request.Periods; i++)
                {
                    DateTime from, to;
                    string label;

                    switch (request.Grouping)
                    {
                        case PerformanceGroupingType.Daily:
                            from = now.AddDays(-request.Periods + 1 + i);
                            to = from;
                            label = from.ToString("yyyy-MM-dd");
                            break;

                        case PerformanceGroupingType.Weekly:
                            from = now.AddDays(-7 * (request.Periods - i - 1)).StartOfWeek(DayOfWeek.Saturday);
                            to = from.AddDays(6);
                            label = $"Week {i + 1}";
                            break;

                        case PerformanceGroupingType.Monthly:
                            from = now.AddMonths(-request.Periods + 1 + i).StartOfMonth();
                            to = from.EndOfMonth();
                            label = from.ToString("yyyy-MM");
                            break;

                        default:
                            from = to = now;
                            label = "";
                            break;
                    }

                    var periodAnswers = planAnswers.Count(a => planUserIds.Contains(a.UserPlan.UserId) && a.CreatedAt.Date >= from && a.CreatedAt.Date <= to);
                    var periodPoints = points.Where(p => planUserIds.Contains(p.UserId) && p.TransactionDate.Date >= from && p.TransactionDate.Date <= to)
                                             .Sum(p => p.Amount);

                    team.DataPoints.Add(new PerformanceChartPointDTO
                    {
                        PeriodLabel = label,
                        AnswerCount = periodAnswers,
                        EarnedPoints = periodPoints
                    });
                }

                result.Add(team);
            }

            return ServiceResult.Ok(result);
        }
    }

}
