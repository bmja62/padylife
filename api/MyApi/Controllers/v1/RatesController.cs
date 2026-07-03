using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Application.Rates.Commands;
using Application.Rates.Queries;
using Asp.Versioning;
using Entities.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.CommentServices.DTOs;
using Services.Services.RateServices.DTOs;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر ستاره
    /// </summary>
    [ApiVersion("1")]
    public class RatesController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : BaseController
    {

        /// <summary>
        /// ساخت ستاره
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> Create([FromBody] CreateRateDTO input) =>
           (await commandDispatcher
            .SendAsync(new CreateRateCommand(input)))
            .ToApiResult();

        /// <summary>
        /// ویرایش ستاره
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="entityId"></param>
        /// <param name="entityType"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> Update(long userId, long entityId, EntityType entityType, [FromBody] UpdateRateDTO input) =>
           (await commandDispatcher
            .SendAsync(new UpdateRateCommand(userId, entityId, entityType, input)))
            .ToApiResult();

        /// <summary>
        /// دریافت میانگین ستاره
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> GetAverageRating(long entityId, EntityType entityType) =>
           (await queryDispatcher
            .SendAsync(new GetAverageRatingQuery(entityId, entityType)))
            .ToApiResult();

        /// <summary>
        /// ایا قبلا ستاره داده ام؟
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> IsRateExist(long entityId, EntityType entityType) =>
           (await queryDispatcher
            .SendAsync(new IsRateExistQuery(entityId, entityType)))
            .ToApiResult();
    }
}
