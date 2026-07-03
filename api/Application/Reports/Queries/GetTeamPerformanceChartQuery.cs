using Application.Cqrs.Queris;
using Application.Reports.DTOs;
using Common.Utilities;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Reports.Queries
{
    public record GetTeamPerformanceChartQuery(
        Dictionary<string, List<long>> Teams, // کلید: نام تیم، مقدار: لیست UserIdها
        int Periods = 6,
        PerformanceGroupingType Grouping = PerformanceGroupingType.Weekly
    ) : IQuery<ServiceResult<List<TeamPerformanceChartSeriesDTO>>>;

    public class GetTeamPerformanceChartQueryHandler(
    IRepository<UserPlan> userPlanRepo,
    IRepository<UserPlanAnswer> answerRepo,
    IRepository<PointTransaction> pointRepo
) : IQueryHandler<GetTeamPerformanceChartQuery, ServiceResult<List<TeamPerformanceChartSeriesDTO>>>
    {
        public async Task<ServiceResult<List<TeamPerformanceChartSeriesDTO>>> Handle(GetTeamPerformanceChartQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow.Date;
            var periods = request.Periods;

            var startDate = request.Grouping switch
            {
                PerformanceGroupingType.Daily => now.AddDays(-periods + 1),
                PerformanceGroupingType.Weekly => now.AddDays(-7 * (periods - 1)),
                PerformanceGroupingType.Monthly => now.AddMonths(-periods + 1),
                _ => now
            };

            var requestUserIds = request.Teams.SelectMany(t => t.Value).Distinct().ToList();

            var allUserIds = await userPlanRepo.Table.Where(t => requestUserIds.Contains(t.Id)).Select(t => t.UserId).ToListAsync();

            var answers = await answerRepo.TableNoTracking
                .Where(a => allUserIds.Contains(a.UserPlan.UserId) && a.CreatedAt.Date >= startDate)
                .ToListAsync(cancellationToken);

            var points = await pointRepo.TableNoTracking
                .Where(p => allUserIds.Contains(p.UserId) && p.TransactionType == PointTransactionType.Earn && !p.IsReverted && p.TransactionDate.Date >= startDate)
                .ToListAsync(cancellationToken);

            var result = new List<TeamPerformanceChartSeriesDTO>();

            foreach (var (teamName, userIds) in request.Teams)
            {
                var teamSeries = new TeamPerformanceChartSeriesDTO
                {
                    PlanName = teamName,
                    DataPoints = new()
                };

                for (int i = 0; i < periods; i++)
                {
                    DateTime from, to;
                    string label;

                    switch (request.Grouping)
                    {
                        case PerformanceGroupingType.Daily:
                            from = now.AddDays(-periods + 1 + i);
                            to = from;
                            label = from.ToString("yyyy-MM-dd");
                            break;

                        case PerformanceGroupingType.Weekly:
                            from = now.AddDays(-7 * (periods - i - 1)).StartOfWeek(DayOfWeek.Saturday);
                            to = from.AddDays(6);
                            label = $"Week {i + 1}";
                            break;

                        case PerformanceGroupingType.Monthly:
                            from = now.AddMonths(-periods + 1 + i).StartOfMonth();
                            to = from.EndOfMonth();
                            label = from.ToString("yyyy-MM");
                            break;

                        default:
                            from = to = now;
                            label = "";
                            break;
                    }

                    var teamAnswers = answers
                        .Where(a => userIds.Contains(a.UserPlan.UserId) && a.CreatedAt.Date >= from && a.CreatedAt.Date <= to)
                        .Count();

                    var teamPoints = points
                        .Where(p => userIds.Contains(p.UserId) && p.TransactionDate.Date >= from && p.TransactionDate.Date <= to)
                        .Sum(p => p.Amount);

                    teamSeries.DataPoints.Add(new PerformanceChartPointDTO
                    {
                        PeriodLabel = label,
                        AnswerCount = teamAnswers,
                        EarnedPoints = teamPoints
                    });
                }

                result.Add(teamSeries);
            }

            return ServiceResult.Ok(result);
        }
    }

}
