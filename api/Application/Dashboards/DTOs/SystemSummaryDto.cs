using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dashboards.DTOs
{
    public class SystemSummaryDto
    {
        public int TotalUsers { get; set; }
        public int TotalExperts { get; set; }
        public int TotalActiveUsers { get; set; }
        public int ActiveBaskets { get; set; }
        public decimal DailySales { get; set; }
        public int DailyOrders { get; set; }
        public decimal WeeklySales { get; set; }
        public int WeeklyOrders { get; set; }
        public decimal MonthlySales { get; set; }
        public int MonthlyOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    public class SalesReportDto
    {
        public decimal DailySales { get; set; }
        public int DailyOrders { get; set; }
        public decimal WeeklySales { get; set; }
        public int WeeklyOrders { get; set; }
        public decimal MonthlySales { get; set; }
        public int MonthlyOrders { get; set; }
    }

    public class UsersReportDto
    {
        public int TotalUsers { get; set; }
        public int TotalExperts { get; set; }
        public int NewUsersThisWeek { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int ActiveUsersToday { get; set; }
    }

    public class DailySalesTrendDto
    {
        public string Date { get; set; }
        public decimal SalesAmount { get; set; }
        public int OrdersCount { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    public class TopProductDto
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public int SalesCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class TopProductsRequestDto
    {
        public int TopCount { get; set; } = 10;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
