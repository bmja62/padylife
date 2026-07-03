using Application.Cqrs.Queris;
using Data.Contracts;
using Entities.DailyFeelings;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Reports.Queries
{
    public record GetWeeklyFeelingsReportQuery(long UserId, int Weeks = 4)
        : IQuery<ServiceResult<WeeklyFeelingsReportDTO>>;

    public class GetWeeklyFeelingsReportQueryHandler(
        IRepository<DailyFeeling> feelingsRepo
    ) : IQueryHandler<GetWeeklyFeelingsReportQuery, ServiceResult<WeeklyFeelingsReportDTO>>
    {
        public async Task<ServiceResult<WeeklyFeelingsReportDTO>> Handle(
            GetWeeklyFeelingsReportQuery request,
            CancellationToken cancellationToken)
        {
            var endDate = DateTime.UtcNow.Date.AddDays(1);
            var startDate = endDate.AddDays(-7 * request.Weeks);

            var feelings = await feelingsRepo.TableNoTracking
                .Where(f => f.CreatedByUserId == request.UserId &&
                           f.CreatedAt >= startDate &&
                           f.CreatedAt <= endDate &&
                           !f.IsDeleted)
                .ToListAsync(cancellationToken);

            var weeklyData = new List<WeeklyFeelingDataDTO>();

            for (int i = 0; i < request.Weeks; i++)
            {
                var weekStart = endDate.AddDays(-7 * (i + 1));
                var weekEnd = endDate.AddDays(-7 * i);

                var weekFeelings = feelings
                    .Where(f => f.CreatedAt >= weekStart && f.CreatedAt < weekEnd)
                    .ToList();

                var averageFeeling = weekFeelings.Any() ?
                    (int)weekFeelings.Average(f => (int)f.Type) : 0;

                weeklyData.Add(new WeeklyFeelingDataDTO
                {
                    WeekNumber = i + 1,
                    WeekLabel = $"هفته {i + 1}",
                    FeelingEntries = weekFeelings.Count,
                    AverageFeeling = averageFeeling,
                    FeelingTypes = weekFeelings
                        .GroupBy(f => f.Type)
                        .ToDictionary(g => g.Key.ToString(), g => g.Count())
                });
            }

            var result = new WeeklyFeelingsReportDTO
            {
                Weeks = request.Weeks,
                WeeklyData = weeklyData.OrderBy(w => w.WeekNumber).ToList()
            };

            return ServiceResult.Ok(result);
        }
    }

    public class WeeklyFeelingsReportDTO
    {
        public int Weeks { get; set; }
        public List<WeeklyFeelingDataDTO> WeeklyData { get; set; } = new();
    }

    public class WeeklyFeelingDataDTO
    {
        public int WeekNumber { get; set; }
        public string WeekLabel { get; set; } = string.Empty;
        public int FeelingEntries { get; set; }
        public int AverageFeeling { get; set; }
        public Dictionary<string, int> FeelingTypes { get; set; } = new();
    }
}