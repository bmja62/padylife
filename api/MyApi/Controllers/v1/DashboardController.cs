using Application.Cqrs.Queris;
using Application.Dashboard.Queries;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر گزارش‌های جامع سیستم
    /// </summary>
    [ApiVersion("1")]
    public class DashboardController : BaseController
    {
        private readonly IQueryDispatcher _queryDispatcher;

        public DashboardController(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
        }

        /// <summary>
        /// دریافت آمار کلی سیستم
        /// </summary>
        /// <returns></returns>
        [HttpGet("summary")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ApiResult> GetSystemSummary() =>
            (await _queryDispatcher
                .SendAsync(new GetSystemSummaryQuery()))
            .ToApiResult();

        /// <summary>
        /// دریافت گزارش فروش روزانه، هفتگی و ماهانه
        /// </summary>
        /// <returns></returns>
        [HttpGet("sales-report")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ApiResult> GetSalesReport() =>
            (await _queryDispatcher
                .SendAsync(new GetSalesReportQuery()))
            .ToApiResult();

        /// <summary>
        /// دریافت آمار کاربران و متخصصین
        /// </summary>
        /// <returns></returns>
        [HttpGet("users-report")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ApiResult> GetUsersReport() =>
            (await _queryDispatcher
                .SendAsync(new GetUsersReportQuery()))
            .ToApiResult();

        /// <summary>
        /// دریافت روند فروش هفته جاری
        /// </summary>
        /// <returns></returns>
        [HttpGet("weekly-trend")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ApiResult> GetWeeklySalesTrend() =>
            (await _queryDispatcher
                .SendAsync(new GetWeeklySalesTrendQuery()))
            .ToApiResult();

        /// <summary>
        /// دریافت محصولات پرفروش
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("top-products")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ApiResult> GetTopProducts([FromBody] GetTopProductsQuery query) =>
            (await _queryDispatcher
                .SendAsync(query))
            .ToApiResult();
    }
}
