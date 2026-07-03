using Application.Cqrs.Queris;

using Application.Dashboards.DTOs;
using Common.Utilities;
using Data.Contracts;
using Entities.Baskets;
using Entities.Orders;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Dashboard.Queries
{
    public record GetSystemSummaryQuery() : IQuery<ServiceResult<SystemSummaryDto>>;

    public class GetSystemSummaryQueryHandler(
        IRepository<Entities.Users.User> userRepository,
        IRepository<Basket> basketRepository,
        IRepository<Order> orderRepository)
        : IQueryHandler<GetSystemSummaryQuery, ServiceResult<SystemSummaryDto>>
    {
        public async Task<ServiceResult<SystemSummaryDto>> Handle(GetSystemSummaryQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(today.Year, today.Month, 1);

            // آمار کاربران
            var totalUsers = await userRepository.Table
                .CountAsync(u => u is User && u.IsActive, cancellationToken);

            var totalExperts = await userRepository.Table
                .CountAsync(u => u is Expert && u.IsActive, cancellationToken);

            var totalActiveUsers = await userRepository.Table
                .CountAsync(u => u.IsActive, cancellationToken);

            // آمار سبدهای خرید
            var activeBaskets = await basketRepository.Table
                .CountAsync(b => b.Status == BasketStatus.Active, cancellationToken);

            // آمار فروش - محاسبه دستی FinalAmount
            var ordersQuery = orderRepository.Table
                .Where(o => o.PaymentStatus == PaymentStatus.Paid &&
                           o.Status != OrderStatus.Cancelled);

            // محاسبه فروش روزانه
            var dailyOrders = await ordersQuery
                .Where(o => o.CreatedAt.Date == today)
                .Select(o => new { o.TotalAmount, o.DiscountAmount })
                .ToListAsync(cancellationToken);

            var dailySales = dailyOrders.Sum(o => o.TotalAmount - o.DiscountAmount);
            var dailyOrdersCount = dailyOrders.Count;

            // محاسبه فروش هفتگی
            var weeklyOrders = await ordersQuery
                .Where(o => o.CreatedAt.Date >= weekStart)
                .Select(o => new { o.TotalAmount, o.DiscountAmount })
                .ToListAsync(cancellationToken);

            var weeklySales = weeklyOrders.Sum(o => o.TotalAmount - o.DiscountAmount);
            var weeklyOrdersCount = weeklyOrders.Count;

            // محاسبه فروش ماهانه
            var monthlyOrders = await ordersQuery
                .Where(o => o.CreatedAt.Date >= monthStart)
                .Select(o => new { o.TotalAmount, o.DiscountAmount })
                .ToListAsync(cancellationToken);

            var monthlySales = monthlyOrders.Sum(o => o.TotalAmount - o.DiscountAmount);
            var monthlyOrdersCount = monthlyOrders.Count;

            // میانگین ارزش سفارش
            var averageOrderValue = monthlyOrdersCount > 0 ? monthlySales / monthlyOrdersCount : 0;

            var result = new SystemSummaryDto
            {
                TotalUsers = totalUsers,
                TotalExperts = totalExperts,
                TotalActiveUsers = totalActiveUsers,
                ActiveBaskets = activeBaskets,
                DailySales = dailySales,
                DailyOrders = dailyOrdersCount,
                WeeklySales = weeklySales,
                WeeklyOrders = weeklyOrdersCount,
                MonthlySales = monthlySales,
                MonthlyOrders = monthlyOrdersCount,
                AverageOrderValue = averageOrderValue
            };

            return ServiceResult.Ok(result);
        }
    }

    public record GetSalesReportQuery() : IQuery<ServiceResult<SalesReportDto>>;

    public class GetSalesReportQueryHandler(
        IRepository<Order> orderRepository)
        : IQueryHandler<GetSalesReportQuery, ServiceResult<SalesReportDto>>
    {
        public async Task<ServiceResult<SalesReportDto>> Handle(GetSalesReportQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var ordersQuery = orderRepository.Table
                .Where(o => o.PaymentStatus == PaymentStatus.Paid &&
                           o.Status != OrderStatus.Cancelled);

            // محاسبه فروش روزانه
            var dailyData = await ordersQuery
                .Where(o => o.CreatedAt.Date == today)
                .Select(o => new { o.TotalAmount, o.DiscountAmount })
                .ToListAsync(cancellationToken);

            // محاسبه فروش هفتگی
            var weeklyData = await ordersQuery
                .Where(o => o.CreatedAt.Date >= weekStart)
                .Select(o => new { o.TotalAmount, o.DiscountAmount })
                .ToListAsync(cancellationToken);

            // محاسبه فروش ماهانه
            var monthlyData = await ordersQuery
                .Where(o => o.CreatedAt.Date >= monthStart)
                .Select(o => new { o.TotalAmount, o.DiscountAmount })
                .ToListAsync(cancellationToken);

            var result = new SalesReportDto
            {
                DailySales = dailyData.Sum(o => o.TotalAmount - o.DiscountAmount),
                DailyOrders = dailyData.Count,
                WeeklySales = weeklyData.Sum(o => o.TotalAmount - o.DiscountAmount),
                WeeklyOrders = weeklyData.Count,
                MonthlySales = monthlyData.Sum(o => o.TotalAmount - o.DiscountAmount),
                MonthlyOrders = monthlyData.Count
            };

            return ServiceResult.Ok(result);
        }
    }

    public record GetUsersReportQuery() : IQuery<ServiceResult<UsersReportDto>>;

    public class GetUsersReportQueryHandler(
        IRepository<Entities.Users.User> userRepository)
        : IQueryHandler<GetUsersReportQuery, ServiceResult<UsersReportDto>>
    {
        public async Task<ServiceResult<UsersReportDto>> Handle(GetUsersReportQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var result = new UsersReportDto
            {
                TotalUsers = await userRepository.Table
                    .CountAsync(u => u is User && u.IsActive, cancellationToken),

                TotalExperts = await userRepository.Table
                    .CountAsync(u => u is Expert && u.IsActive, cancellationToken),

                NewUsersThisWeek = await userRepository.Table
                    .CountAsync(u => u.CreateAt >= weekStart && u.CreateAt < today.AddDays(1), cancellationToken),

                NewUsersThisMonth = await userRepository.Table
                    .CountAsync(u => u.CreateAt >= monthStart && u.CreateAt < today.AddDays(1), cancellationToken),

                ActiveUsersToday = await userRepository.Table
                    .CountAsync(u => u.LastLoginDate.HasValue &&
                                   u.LastLoginDate.Value.Date == today, cancellationToken)
            };

            return ServiceResult.Ok(result);
        }
    }

    public record GetWeeklySalesTrendQuery() : IQuery<ServiceResult<List<DailySalesTrendDto>>>;

    public class GetWeeklySalesTrendQueryHandler(
        IRepository<Order> orderRepository)
        : IQueryHandler<GetWeeklySalesTrendQuery, ServiceResult<List<DailySalesTrendDto>>>
    {
        public async Task<ServiceResult<List<DailySalesTrendDto>>> Handle(GetWeeklySalesTrendQuery request, CancellationToken cancellationToken)
        {
            var weekStart = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek);

            // دریافت داده‌ها و گروه‌بندی در حافظه
            var dailyData = await orderRepository.Table
                .Where(o => o.PaymentStatus == PaymentStatus.Paid &&
                           o.Status != OrderStatus.Cancelled &&
                           o.CreatedAt >= weekStart)
                .Select(o => new
                {
                    Date = o.CreatedAt.Date,
                    TotalAmount = o.TotalAmount,
                    DiscountAmount = o.DiscountAmount
                })
                .ToListAsync(cancellationToken);

            var dailyTrends = dailyData
                .GroupBy(o => o.Date)
                .Select(g => new DailySalesTrendDto
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    SalesAmount = g.Sum(o => o.TotalAmount - o.DiscountAmount),
                    OrdersCount = g.Count(),
                    AverageOrderValue = g.Count() > 0 ? g.Average(o => o.TotalAmount - o.DiscountAmount) : 0
                })
                .OrderBy(x => x.Date)
                .ToList();

            return ServiceResult.Ok(dailyTrends);
        }
    }

    public record GetTopProductsQuery(TopProductsRequestDto Request) : IQuery<ServiceResult<List<TopProductDto>>>;

    public class GetTopProductsQueryHandler(
        IRepository<Order> orderRepository)
        : IQueryHandler<GetTopProductsQuery, ServiceResult<List<TopProductDto>>>
    {
        public async Task<ServiceResult<List<TopProductDto>>> Handle(GetTopProductsQuery request, CancellationToken cancellationToken)
        {
            var fromDate = request.Request.FromDate ?? DateTime.UtcNow.AddMonths(-1);
            var toDate = request.Request.ToDate ?? DateTime.UtcNow;

            var topProducts = await orderRepository.Table
                .Where(o => o.PaymentStatus == PaymentStatus.Paid &&
                           o.Status != OrderStatus.Cancelled &&
                           o.CreatedAt >= fromDate &&
                           o.CreatedAt <= toDate)
                .SelectMany(o => o.Items)
                .GroupBy(oi => new { oi.ObjectId, oi.ItemType })
                .Select(g => new TopProductDto
                {
                    ProductId = g.Key.ObjectId,
                    ProductType = g.Key.ItemType.ToString(),
                    SalesCount = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
                })
                .OrderByDescending(x => x.SalesCount)
                .Take(request.Request.TopCount)
                .ToListAsync(cancellationToken);

            return ServiceResult.Ok(topProducts);
        }
    }
}