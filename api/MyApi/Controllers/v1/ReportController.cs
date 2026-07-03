using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Application.Plans.Queries;
using Application.Reports.DTOs;
using Application.Reports.Queries;
using Asp.Versioning;
using Common.GridResults;
using Common.Roles;
using Common.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر گزارشات
    /// </summary>
    /// <param name="queryDispatcher"></param>
    /// <param name="accessor"></param>
    [ApiVersion("1")]
    public class ReportController(IQueryDispatcher queryDispatcher, IHttpContextAccessor accessor) : BaseController
    {
        /// <summary>
        /// نمودار پلن های فرد
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<List<UserPlanStatsChartDTO>>> GetMyPlanChartStats()
        {
            var userId = accessor.HttpContext.User.Identity.GetUserId<long>();
            return (await queryDispatcher.SendAsync(new GetUserPlanStatsChartQuery(userId))).ToApiResult();
        }
        /// <summary>
        /// نمودار پیشرفت
        /// </summary>
        /// <param name="periods"></param>
        /// <param name="grouping"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<List<PerformanceChartPointDTO>>> GetPerformanceChart(
            [FromQuery] int periods = 6,
            [FromQuery] PerformanceGroupingType grouping = PerformanceGroupingType.Weekly)
        {
            var userId = accessor.HttpContext.User.Identity.GetUserId<long>();
            return (await queryDispatcher.SendAsync(new GetPerformanceChartQuery(userId, periods, grouping))).ToApiResult();
        }

        /// <summary>
        /// نمودار پیشرفت گروهی
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = $"{UserRoleNames.Admin}")]
        public async Task<ApiResult<List<GroupPerformanceChartSeriesDTO>>> GetGroupPerformanceChart([FromBody] GetGroupPerformanceChartQuery input)
        {
            return (await queryDispatcher.SendAsync(input)).ToApiResult();
        }

        /// <summary>
        /// گزارش پیشرفت تیمی
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult<List<TeamPerformanceChartSeriesDTO>>> GetTeamPerformanceChart([FromBody] GetTeamPerformanceChartQuery input)
        {
            return (await queryDispatcher.SendAsync(input)).ToApiResult();
        }

        /// <summary>
        /// ساخت نمودار بر اساس برترین پلن
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult<List<TeamPerformanceChartSeriesDTO>>> AutoGeneratePlanTeamChart([FromBody] AutoGeneratePlanTeamChartQuery input)
        {
            return (await queryDispatcher.SendAsync(input)).ToApiResult();
        }

        /// <summary>
        /// ساخت تیم رنکینگ اتومات
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = $"{UserRoleNames.Admin}")]
        public async Task<ApiResult<List<TeamRankDTO>>> GenerateTeamRankings([FromBody] GenerateTeamRankingsQuery input)
        {
            return (await queryDispatcher.SendAsync(input)).ToApiResult();
        }

        /// <summary>
        /// گزارش پیشرفت دوره
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult<CourseProgressReportDTO>> GetCourseProgressReport([FromBody] GetCourseProgressReportQuery input)
        {
            return (await queryDispatcher.SendAsync(input)).ToApiResult();
        }


        /// <summary>
        /// گزارش تعهد به برنامه
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult<WeeklyCommitmentReportDTO>> GetWeeklyCommitmentReport([FromBody] GetWeeklyCommitmentReportQuery input)
        {
            return (await queryDispatcher.SendAsync(input)).ToApiResult();
        }
        /// <summary>
        /// گزارش هفتگی احساسات
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult<WeeklyFeelingsReportDTO>> GetWeeklyFeelingsReport([FromBody] GetWeeklyFeelingsReportQuery input)
        {
            return (await queryDispatcher.SendAsync(input)).ToApiResult();
        }

        /// <summary>
        ///  گزارش فعالیت‌های کاربر
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult<UserActivityReportDTO>> GetUserActivityReport([FromBody] GetUserActivityReportQuery input)
        {
            return (await queryDispatcher.SendAsync(input)).ToApiResult();
        }

        /// <summary>
        ///  گزارش فعالیت‌های کاربر
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult<GetUserActivityForAllQuestionsReportDTO>> GetUserActivityForAllQuestionsReport([FromBody] GetUserActivityForAllQuestionsReportQuery input)
        {
            return (await queryDispatcher.SendAsync(input)).ToApiResult();
        }
        [HttpGet]
        [Authorize(Roles = $"{UserRoleNames.Specialist}")]

        public async Task<ApiResult<GlobalGridResult<GetReportPlanResponseDto>>> GetReportPlanForExpert([FromQuery] GetReportPlanRequestDto input)
        {
            return (await queryDispatcher.SendAsync(new GetReportPlanQuery(input))).ToApiResult();
        }
    }
}
