using Application.Cqrs.Queris;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Points.Queries
{
    public record MyPointsSummaryQuery(long userId) : IQuery<ServiceResult<MyPointsSummaryDTO>>;

    public class MyPointsSummaryQueryHandler(
        IRepository<PointTransaction> pointTransactionRepo
    ) : IQueryHandler<MyPointsSummaryQuery, ServiceResult<MyPointsSummaryDTO>>
    {
        public async Task<ServiceResult<MyPointsSummaryDTO>> Handle(MyPointsSummaryQuery request, CancellationToken cancellationToken)
        {
            var userId = request.userId;
            var now = DateTime.Now;

            // تاریخ شروع هفته جاری (شنبه)
            var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek);
            // تاریخ شروع ماه جاری
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            // مجموع امتیاز کل
            var totalPoints = await pointTransactionRepo.TableNoTracking
                .Where(t => t.UserId == userId &&
                           t.TransactionType == PointTransactionType.Earn &&
                           !t.IsReverted)
                .SumAsync(t => (int?)t.Amount, cancellationToken) ?? 0;

            // مجموع امتیاز این ماه
            var monthlyPoints = await pointTransactionRepo.TableNoTracking
                .Where(t => t.UserId == userId &&
                           t.TransactionType == PointTransactionType.Earn &&
                           !t.IsReverted &&
                           t.TransactionDate >= startOfMonth)
                .SumAsync(t => (int?)t.Amount, cancellationToken) ?? 0;

            // مجموع امتیاز این هفته
            var weeklyPoints = await pointTransactionRepo.TableNoTracking
                .Where(t => t.UserId == userId &&
                           t.TransactionType == PointTransactionType.Earn &&
                           !t.IsReverted &&
                           t.TransactionDate >= startOfWeek)
                .SumAsync(t => (int?)t.Amount, cancellationToken) ?? 0;

            var result = new MyPointsSummaryDTO
            {
                TotalPoints = totalPoints,
                MonthlyPoints = monthlyPoints,
                WeeklyPoints = weeklyPoints
            };

            return ServiceResult.Ok(result);
        }
    }

    public class MyPointsSummaryDTO
    {
        public int TotalPoints { get; set; }      // امتیاز کل
        public int MonthlyPoints { get; set; }    // امتیاز این ماه
        public int WeeklyPoints { get; set; }     // امتیاز این هفته
    }
}
