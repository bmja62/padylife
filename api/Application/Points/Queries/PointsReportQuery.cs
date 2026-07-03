using Application.Cqrs.Queris;
using Application.Points.DTOs;
using Common.GridResults;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Points.Queries
{
    public record PointsReportQuery(long userId, int? pageNumber, int? count) : IQuery<ServiceResult<PointsReportDTO>>;

    public class MyPointsReportQueryHandler(
        IRepository<PointTransaction> pointTransactionRepo,
        IRepository<UserPoints> userPointsRepo
    ) : IQueryHandler<PointsReportQuery, ServiceResult<PointsReportDTO>>
    {
        public async Task<ServiceResult<PointsReportDTO>> Handle(PointsReportQuery request, CancellationToken cancellationToken)
        {
            var userId = request.userId;

            // مجموع امتیاز من
            var myTotalPoints = await pointTransactionRepo.TableNoTracking
                .Where(t => t.UserId == userId && t.TransactionType == PointTransactionType.Earn && !t.IsReverted)
                .SumAsync(t => (int?)t.Amount, cancellationToken) ?? 0;

            // لیست کاربران با امتیازشان
            var userPointGroups = await pointTransactionRepo.TableNoTracking
                .Where(t => t.TransactionType == PointTransactionType.Earn && !t.IsReverted)
                .GroupBy(t => t.UserId)
                .Select(g => new { UserId = g.Key, Total = g.Sum(x => x.Amount) })
                .OrderByDescending(x => x.Total)
                .ToListAsync(cancellationToken);

            var totalUsers = userPointGroups.Count;
            var myRank = userPointGroups.FindIndex(x => x.UserId == userId) + 1;
            var myPercentile = totalUsers > 0 ? Math.Round(((totalUsers - myRank) * 100m) / totalUsers, 2) : 0;

            GlobalGrid globalGrid = new GlobalGrid();
            globalGrid.DefaultPagination(request.pageNumber, request.count);

            // تاریخچه امتیاز من - گروه‌بندی روزانه با جزئیات ساعتی
            var dailyHistory = await pointTransactionRepo.TableNoTracking
                .Where(t => t.UserId == userId && t.TransactionType == PointTransactionType.Earn && !t.IsReverted)
                .GroupBy(t => t.TransactionDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalPoints = g.Sum(x => x.Amount),
                    Transactions = g.Select(t => new
                    {
                        t.TransactionDate,
                        t.Amount,
                        t.Reason
                    }).ToList()
                })
                .OrderBy(x => x.Date)
                .Skip(globalGrid.Skip)
                .Take(globalGrid.Take)
                .ToListAsync(cancellationToken);

            // سپس در حافظه جزئیات ساعتی را پردازش کنیم
            var history = dailyHistory.Select(d => new PointsOverTimeDTO
            {
                Date = d.Date,
                PointsEarned = d.TotalPoints,
                HourlyDetails = d.Transactions
                    .GroupBy(t => new { t.TransactionDate.Hour, t.TransactionDate.Minute })
                    .OrderBy(hg => hg.Key.Hour).ThenBy(hg => hg.Key.Minute)
                    .Select(hg => new HourlyDetailDTO
                    {
                        Time = $"{hg.Key.Hour:00}:{hg.Key.Minute:00}",
                        Points = hg.Sum(x => x.Amount),
                        Reasons = hg.Select(t => t.Reason).ToList()
                    })
                    .ToList()
            }).ToList();

            var result = new PointsReportDTO
            {
                MyTotalPoints = myTotalPoints,
                MyRank = myRank,
                TotalUsers = totalUsers,
                MyPercentile = myPercentile,
                History = history
            };

            return ServiceResult.Ok(result);
        }
    }
}
