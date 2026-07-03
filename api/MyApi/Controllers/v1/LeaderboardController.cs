using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Application.Plans.Queries;
using Asp.Versioning;
using Common.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر رهبران
    /// </summary>
    /// <param name="queryDispatcher"></param>
    [ApiVersion("1")]
    public class LeaderboardController(IQueryDispatcher queryDispatcher) : BaseController
    {

        /// <summary>
        /// دریافت لیدربرد
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<LeaderboardReportDTO>> GetLeaderboard([FromQuery] GetLeaderboardQuery input)
        {
            return (await queryDispatcher.SendAsync(input)).ToApiResult();
        }
        /// <summary>
        /// دریافت لیدربرد کارشناس
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResult<ExpertLeaderboardReportDTO>> GetExpertLeaderboard([FromQuery] GetExpertLeaderboardQuery input)
        {
            return (await queryDispatcher.SendAsync(input)).ToApiResult();
        }

        /// <summary>
        /// دریافت لیدربرد کاربران در یک پلن خاص
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<List<PlanLeaderboardUserDTO>>> GetPlanLeaderboard([FromQuery] GetPlanLeaderboardQueryDTO input)
        {
            return (await queryDispatcher.SendAsync(new GetPlanLeaderboardQuery(input))).ToApiResult();
        }

        /// <summary>
        /// وضعیت پلن فرد
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("{planId}")]
        [Authorize]
        public async Task<ApiResult<PlanLeaderboardStatsDTO>> GetPlanLeaderboardStats(long planId)
        {
            var userId = User.Identity.GetUserId<long>();
            return (await queryDispatcher.SendAsync(new GetPlanLeaderboardStatsQuery(planId, userId))).ToApiResult();
        }

        /// <summary>
        /// لیدر بورد شخصی
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<List<UserPlanLeaderboardOverviewDTO>>> GetMyPlanLeaderboards()
        {
            var userId = User.Identity.GetUserId<long>();
            return (await queryDispatcher.SendAsync(new GetAllMyPlanLeaderboardsQuery(userId))).ToApiResult();
        }
    }
}
