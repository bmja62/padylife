using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Application.Points.Commands;
using Application.Points.DTOs;
using Application.Points.Queries;
using Asp.Versioning;
using Common.GridResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر امتیازات
    /// </summary>
    [ApiVersion("1")]
    public class PointsController : BaseController
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public PointsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        /// <summary>
        /// دریافت امتیاز کاربر
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        [Authorize]
        public async Task<ApiResult> GetUserPoints(long userId) =>
            (await _queryDispatcher
                .SendAsync(new GetUserPointsQuery(userId)))
            .ToApiResult();

        /// <summary>
        /// افزایش امتیاز
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("earn")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ApiResult> EarnPoints([FromBody] EarnPointsRequest request) =>
            (await _commandDispatcher
                .SendAsync(new EarnPointsCommand(request.UserId, request.Amount, request.Reason, request.ReferenceId, request.ReferenceType)))
            .ToApiResult();

        /// <summary>
        /// مصرف امتیاز
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("consume")]
        [Authorize]
        public async Task<ApiResult> ConsumePoints([FromBody] ConsumePointsRequest request) =>
            (await _commandDispatcher
                .SendAsync(new ConsumePointsCommand(request.UserId, request.Amount, request.Reason, request.ReferenceId, request.ReferenceType)))
            .ToApiResult();

        /// <summary>
        /// تبدیل امتیاز به پول
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("convert")]
        [Authorize]
        public async Task<ApiResult> ConvertPointsToWalletCredit([FromBody] ConvertPointsToWalletCreditRequest request) =>
            (await _commandDispatcher
                  .SendAsync(new ConvertPointsToWalletCreditCommand(
                    request.UserId,
                    request.PointsToConvert,
                    request.Description)))
                    .ToApiResult();


        /// <summary>
        ///  گزارش حرفه‌ای امتیاز کاربر + مقایسه
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult> PointsReport([FromQuery] PointsReportQuery request) =>
            (await _queryDispatcher
                  .SendAsync(request))
                    .ToApiResult();

        /// <summary>
        ///  گزارش هفتگی ، ماهانه ، سالیانه
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult> MyPointsSummary([FromQuery] MyPointsSummaryQuery request) =>
            (await _queryDispatcher
                  .SendAsync(request))
                    .ToApiResult();
    }




}
