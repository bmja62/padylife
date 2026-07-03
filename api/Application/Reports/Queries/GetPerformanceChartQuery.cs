using Application.Cqrs.Queris;
using Application.Reports.DTOs;
using Common.Utilities;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Reports.Queries
{
    public enum PerformanceGroupingType
    {
        Daily,
        Weekly,
        Monthly
    }

    public record GetPerformanceChartQuery(long UserId, int Periods = 6, PerformanceGroupingType Grouping = PerformanceGroupingType.Weekly)
        : IQuery<ServiceResult<List<PerformanceChartPointDTO>>>;
    public class GetPerformanceChartQueryHandler(
    IRepository<UserPlanAnswer> answerRepo,
    IRepository<PointTransaction> pointRepo
) : IQueryHandler<GetPerformanceChartQuery, ServiceResult<List<PerformanceChartPointDTO>>>
    {
        public async Task<ServiceResult<List<PerformanceChartPointDTO>>> Handle(GetPerformanceChartQuery request, CancellationToken cancellationToken)
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

            // پاسخ‌ها
            var answers = await answerRepo.TableNoTracking
                .Where(a => a.UserPlan.UserId == request.UserId && a.CreatedAt.Date >= startDate)
                .ToListAsync(cancellationToken);

            // امتیازها
            var points = await pointRepo.TableNoTracking
                .Where(p => p.UserId == request.UserId && p.TransactionType == PointTransactionType.Earn && !p.IsReverted && p.TransactionDate.Date >= startDate)
                .ToListAsync(cancellationToken);

            var data = new List<PerformanceChartPointDTO>();

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

                var periodAnswers = answers.Count(a => a.CreatedAt.Date >= from && a.CreatedAt.Date <= to);
                var periodPoints = points.Where(p => p.TransactionDate.Date >= from && p.TransactionDate.Date <= to)
                                         .Sum(p => p.Amount);

                data.Add(new PerformanceChartPointDTO
                {
                    PeriodLabel = label,
                    AnswerCount = periodAnswers,
                    EarnedPoints = periodPoints
                });
            }

            // تعیین Trend
            for (int i = 1; i < data.Count; i++)
            {
                var prev = data[i - 1];
                var curr = data[i];

                curr.Trend = curr.EarnedPoints > prev.EarnedPoints ? "📈 رشد"
                            : curr.EarnedPoints < prev.EarnedPoints ? "📉 نزول"
                            : "➖ ثابت";
            }

            if (data.Any())
                data[0].Trend = "🔹 شروع";

            return ServiceResult.Ok(data);
        }
    }

}
