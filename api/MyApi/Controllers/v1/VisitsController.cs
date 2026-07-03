using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Application.Visits;
using Application.Visits.Commands;
using Application.Visits.Queries;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Services.IpServices;
using System.Security.Claims;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر آمار
    /// </summary>
    [ApiVersion("1")]
    public class VisitTrackingController : BaseController
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly IIpService _ipService;
        private readonly ILogger<VisitTrackingController> _logger;

        public VisitTrackingController(
            ICommandDispatcher commandDispatcher,
            IQueryDispatcher queryDispatcher,
            ILogger<VisitTrackingController> logger,
            IIpService ipService)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _logger = logger;
            _ipService = ipService;
        }

        /// <summary>
        /// ردیابی بازدید صفحه
        /// </summary>
        [HttpPost("track")]
        [AllowAnonymous]
        public async Task<ApiResult> TrackVisit([FromBody] TrackVisitDTO dto)
        {
            try
            {
                var userIp = _ipService.GetClientIp();
                var userAgent = Request.Headers["User-Agent"].ToString();

                // مدیریت Session
                string sessionId;
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("VisitorId")))
                {
                    sessionId = Guid.NewGuid().ToString();
                    HttpContext.Session.SetString("VisitorId", sessionId);
                    _logger.LogInformation("New session created for tracking: {SessionId}", sessionId);
                }
                else
                {
                    sessionId = HttpContext.Session.GetString("VisitorId");
                }

                // دریافت UserId از JWT Token اگر کاربر لاگین باشد
                int? currentUserId = null;
                if (User.Identity.IsAuthenticated)
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(userIdClaim, out int userId))
                    {
                        currentUserId = userId;
                        // ذخیره در Session برای استفاده بعدی
                        HttpContext.Session.SetInt32("UserId", userId);
                    }
                }

                var command = new TrackVisitCommand(dto, userIp, userAgent, sessionId, currentUserId);

                return (await _commandDispatcher.SendAsync(command)).ToApiResult();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Session"))
            {
                // Fallback برای زمانی که Session پیکربندی نشده
                var userIp = _ipService.GetClientIp();
                var userAgent = Request.Headers["User-Agent"].ToString();
                var fallbackSessionId = Guid.NewGuid().ToString();

                int? currentUserId = null;
                if (User.Identity.IsAuthenticated)
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(userIdClaim, out int userId))
                    {
                        currentUserId = userId;
                    }
                }

                var command = new TrackVisitCommand(dto, userIp, userAgent, fallbackSessionId, currentUserId);
                return (await _commandDispatcher.SendAsync(command)).ToApiResult();
            }
        }
        /// <summary>
        /// دریافت آمار بازدید بر اساس فیلتر
        /// </summary>
        [HttpGet("stats")]
        public async Task<ApiResult<List<VisitStatsDTO>>> GetStats([FromQuery] VisitFilterDTO filter)
        {
            var query = new GetVisitStatsQuery(filter);
            return (await _queryDispatcher.SendAsync(query)).ToApiResult();
        }

        /// <summary>
        /// دریافت آمار بازدید بر اساس نوع موجودیت
        /// </summary>
        [HttpGet("stats/entity/{entityType}")]
        public async Task<ApiResult<List<VisitStatsDTO>>> GetStatsByEntityType(
            string entityType,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var filter = new VisitFilterDTO
            {
                EntityType = entityType,
                FromDate = fromDate,
                ToDate = toDate
            };
            var query = new GetVisitStatsQuery(filter);
            return (await _queryDispatcher.SendAsync(query)).ToApiResult();
        }

        /// <summary>
        /// دریافت آمار بازدید یک پلن خاص
        /// </summary>
        [HttpGet("stats/plan/{planId}")]
        public async Task<ApiResult<List<VisitStatsDTO>>> GetPlanStats(
            long planId,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var filter = new VisitFilterDTO
            {
                EntityType = "Plan",
                EntityId = planId,
                FromDate = fromDate,
                ToDate = toDate
            };
            var query = new GetVisitStatsQuery(filter);
            return (await _queryDispatcher.SendAsync(query)).ToApiResult();
        }

        /// <summary>
        /// دریافت آمار بازدید بر اساس بخش
        /// </summary>
        [HttpGet("stats/section/{section}")]
        public async Task<ApiResult<List<VisitStatsDTO>>> GetSectionStats(
            string section,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var filter = new VisitFilterDTO
            {
                Section = section,
                FromDate = fromDate,
                ToDate = toDate
            };
            var query = new GetVisitStatsQuery(filter);
            return (await _queryDispatcher.SendAsync(query)).ToApiResult();
        }

        /// <summary>
        /// دریافت پر بازدیدترین صفحات
        /// </summary>
        [HttpGet("stats/popular")]
        public async Task<ApiResult<List<PopularPageDTO>>> GetPopularPages(
            [FromQuery] int top = 10,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var query = new GetPopularPagesQuery(top, fromDate, toDate);
            return (await _queryDispatcher.SendAsync(query)).ToApiResult();
        }

        /// <summary>
        /// پردازش دستی آمار روزانه (برای ادمین)
        /// </summary>
        [HttpPost("process-daily-stats")]
        public async Task<ApiResult> ProcessDailyStats([FromBody] ProcessDailyStatsDTO dto)
        {
            var command = new ProcessDailyStatsCommand(dto.TargetDate);
            return (await _commandDispatcher.SendAsync(command)).ToApiResult();
        }

        // ========== ENDPOINT های جدید ==========

        /// <summary>
        /// دریافت آمار بازدید یک صفحه خاص
        /// </summary>
        [HttpGet("stats/page")]
        public async Task<ApiResult<PageVisitStatsDTO>> GetPageStats(
            [FromQuery] string pageUrl,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var query = new GetPageStatsQuery(pageUrl, fromDate, toDate);
            return (await _queryDispatcher.SendAsync(query)).ToApiResult();
        }

        /// <summary>
        /// دریافت آمار بازدید بر اساس بخش‌ها
        /// </summary>
        [HttpGet("stats/sections")]
        public async Task<ApiResult<List<SectionStatsDTO>>> GetAllSectionStats(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var query = new GetSectionStatsQuery(fromDate, toDate);
            return (await _queryDispatcher.SendAsync(query)).ToApiResult();
        }

        /// <summary>
        /// دریافت داده‌های نمودار روزانه
        /// </summary>
        [HttpGet("stats/chart/daily")]
        public async Task<ApiResult<List<DailyChartDataDTO>>> GetDailyChartData(
            [FromQuery] string entityType = null,
            [FromQuery] long? entityId = null,
            [FromQuery] int days = 30)
        {
            var query = new GetDailyChartDataQuery(entityType, entityId, days);
            return (await _queryDispatcher.SendAsync(query)).ToApiResult();
        }

        /// <summary>
        /// دریافت خلاصه آمار
        /// </summary>
        [HttpGet("stats/summary")]
        public async Task<ApiResult<VisitSummaryDTO>> GetVisitSummary(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var query = new GetVisitSummaryQuery(fromDate, toDate);
            return (await _queryDispatcher.SendAsync(query)).ToApiResult();
        }

        /// <summary>
        /// پردازش آمار برای بازه زمانی مشخص
        /// </summary>
        [HttpPost("process-range-stats")]
        public async Task<ApiResult> ProcessDateRangeStats([FromBody] ProcessDateRangeStatsDTO dto)
        {
            var command = new ProcessDateRangeStatsCommand(dto.FromDate, dto.ToDate);
            return (await _commandDispatcher.SendAsync(command)).ToApiResult();
        }

        /// <summary>
        /// پاک‌سازی داده‌های قدیمی
        /// </summary>
        [HttpPost("cleanup")]
        public async Task<ApiResult> CleanupOldData([FromBody] CleanupDataDTO dto)
        {
            var command = new CleanupOldDataCommand(dto.KeepDays);
            return (await _commandDispatcher.SendAsync(command)).ToApiResult();
        }

        /// <summary>
        /// دریافت آمار رشد بازدید
        /// </summary>
        [HttpGet("stats/growth/{entityType}")]
        public async Task<ApiResult<GrowthStatsDTO>> GetGrowthStats(
            string entityType,
            [FromQuery] long? entityId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var query = new GetGrowthStatsQuery(entityType, entityId, fromDate, toDate);
            return (await _queryDispatcher.SendAsync(query)).ToApiResult();
        }

        /// <summary>
        /// دریافت منابع بازدید یک صفحه
        /// </summary>
        [HttpGet("stats/sources")]
        public async Task<ApiResult<List<VisitSourceDTO>>> GetVisitSources(
            [FromQuery] string pageUrl,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var query = new GetVisitSourcesQuery(pageUrl, fromDate, toDate);
            return (await _queryDispatcher.SendAsync(query)).ToApiResult();
        }

        /// <summary>
        /// دریافت آمار بازدید واقعی (Real-time)
        /// </summary>
        [HttpGet("stats/realtime")]
        public async Task<ApiResult<RealtimeStatsDTO>> GetRealtimeStats(
            [FromQuery] int hours = 24)
        {
            var query = new GetRealtimeStatsQuery(hours);
            return (await _queryDispatcher.SendAsync(query)).ToApiResult();
        }

       
    }
}