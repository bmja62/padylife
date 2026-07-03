using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Application.DailyFeelings.Commands.CreateDailyFeeling;
using Application.DailyFeelings.DTOs;
using Application.DailyFeelings.Queries.GetAllDailyFeeling;
using Asp.Versioning;
using Common.GridResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر احساسات روزانه
    /// </summary>
    [ApiVersion("1")]
    public class DailyFeelingsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : BaseController
    {
        /// <summary>
        /// ساخت احساسات
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> Create([FromBody] CreateDailyFeelingCommandDTO input) =>
           (await commandDispatcher
            .SendAsync(new CreateDailyFeelingCommand(input)))
            .ToApiResult();


        [HttpGet]
        [Authorize]
        public async Task<ApiResult<GlobalGridResult<GetAllDailyFeelingDTO>>> GetAll([FromQuery] GetAllDailyFeelingQueryDTO input) =>
           (await queryDispatcher
            .SendAsync(new GetAllDailyFeelingQuery(input)))
            .ToApiResult();



    }
}
