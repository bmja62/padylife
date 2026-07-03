using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Application.StepOptions.Commands;
using Application.StepOptions.DTOs;
using Application.StepOptions.Queries;
using Asp.Versioning;
using Common.GridResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر گزینه‌های گام تمرین
    /// </summary>
    [ApiVersion("1")]
    public class StepOptionController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : BaseController
    {
        /// <summary>
        /// ساخت پاسخ ویدیویی
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> CreateVideo([FromBody] CreateVideoStepOptionCommandDTO input)
        {
            var result = await commandDispatcher.SendAsync(new CreateVideoStepOptionCommand(input));
            return result.ToApiResult();
        }
        /// <summary>
        /// ساخت پاسخ چند گزینه ای
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> CreateMultipleChoice([FromBody] CreateMultipleChoiceStepOptionCommandDTO input)
        {
            var result = await commandDispatcher.SendAsync(new CreateMultipleChoiceStepOptionCommand(input));
            return result.ToApiResult();
        }

        /// <summary>
        /// ساخت پاسخ متنی
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> CreateText([FromBody] CreateTextStepOptionCommandDTO input)
        {
            var result = await commandDispatcher.SendAsync(new CreateTextStepOptionCommand(input));
            return result.ToApiResult();
        }

        /// <summary>
        /// ساخت پاسخ تصویری
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> CreateImage([FromBody] CreateImageStepOptionCommandDTO input)
        {
            var result = await commandDispatcher.SendAsync(new CreateImageStepOptionCommand(input));
            return result.ToApiResult();
        }

        /// <summary>
        /// ساخت تکلیف
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> CreateTask([FromBody] CreateTaskStepOptionCommandDTO input)
        {
            var result = await commandDispatcher.SendAsync(new CreateTaskStepOptionCommand(input));
            return result.ToApiResult();
        }

        /// <summary>
        /// ساخت تعامل
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> CreateAction([FromBody] CreateActionStepOptionCommandDTO input)
        {
            var result = await commandDispatcher.SendAsync(new CreateActionStepOptionCommand(input));
            return result.ToApiResult();
        }


        /// <summary>
        /// دریافت همه گزینه‌های یک مرحله
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<GlobalGridResult<GetStepOptionDTO>>> GetAll([FromQuery] GetAllStepOptionsQueryDTO input)
        {
            var result = await queryDispatcher.SendAsync(new GetAllStepOptionsQuery(input));
            return result.ToApiResult();
        }

        /// <summary>
        /// دریافت همه گزینه‌های یک مرحله
        /// </summary>
        [HttpDelete]
        [Authorize]
        public async Task<ApiResult> Delete(long id, bool confrim)
        {
            var result = await commandDispatcher.SendAsync(new DeleteStepOptionsCommand(id, confrim));
            return result.ToApiResult();
        }
    }
}
