using Application.Cqrs.Queris;
using Application.Reports.DTOs;
using Common.Utilities;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Reports.Queries
{
    public record GetGroupPerformanceChartQuery(
        List<long> UserIds,
        int Periods = 6,
        PerformanceGroupingType Grouping = PerformanceGroupingType.Weekly)
        : IQuery<ServiceResult<List<GroupPerformanceChartSeriesDTO>>>;

    public class GetGroupPerformanceChartQueryHandler(
    IRepository<UserPlanAnswer> answerRepo,
    IRepository<PointTransaction> pointRepo,
    IRepository<User> userRepo
) : IQueryHandler<GetGroupPerformanceChartQuery, ServiceResult<List<GroupPerformanceChartSeriesDTO>>>
    {
        public async Task<ServiceResult<List<GroupPerformanceChartSeriesDTO>>> Handle(GetGroupPerformanceChartQuery request, CancellationToken cancellationToken)
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

            var userMap = await userRepo.TableNoTracking
                .Where(u => request.UserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, cancellationToken);

            var answers = await answerRepo.TableNoTracking.Include(t => t.UserPlan)
                .Where(a => request.UserIds.Contains(a.UserPlan.UserId) && a.CreatedAt.Date >= startDate)
                .ToListAsync(cancellationToken);

            var points = await pointRepo.TableNoTracking
                .Where(p => request.UserIds.Contains(p.UserId) && p.TransactionType == PointTransactionType.Earn && !p.IsReverted && p.TransactionDate.Date >= startDate)
                .ToListAsync(cancellationToken);

            var result = new List<GroupPerformanceChartSeriesDTO>();

            foreach (var userId in request.UserIds)
            {
                var series = new GroupPerformanceChartSeriesDTO
                {
                    UserId = userId,
                    UserName = userMap.TryGetValue(userId, out var u) ? u.FullName : $"User {userId}",
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

                    var userAnswers = answers.Where(a => a.UserPlan.UserId == userId && a.CreatedAt.Date >= from && a.CreatedAt.Date <= to).Count();
                    var userPoints = points.Where(p => p.UserId == userId && p.TransactionDate.Date >= from && p.TransactionDate.Date <= to).Sum(p => p.Amount);

                    series.DataPoints.Add(new PerformanceChartPointDTO
                    {
                        PeriodLabel = label,
                        AnswerCount = userAnswers,
                        EarnedPoints = userPoints
                    });
                }

                // روند تعیین کنیم (اختیاری)
                for (int j = 1; j < series.DataPoints.Count; j++)
                {
                    var prev = series.DataPoints[j - 1];
                    var curr = series.DataPoints[j];

                    curr.Trend = curr.EarnedPoints > prev.EarnedPoints ? "📈"
                                : curr.EarnedPoints < prev.EarnedPoints ? "📉"
                                : "➖";
                }

                if (series.DataPoints.Any())
                    series.DataPoints[0].Trend = "🔹";

                result.Add(series);
            }

            return ServiceResult.Ok(result);
        }
    }


}
