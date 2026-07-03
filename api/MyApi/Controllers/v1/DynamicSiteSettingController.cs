using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Application.DynamicSiteSettings.Command.CreateDynamicSiteSetting;
using Application.DynamicSiteSettings.Command.DeleteDynamicSiteSetting;
using Application.DynamicSiteSettings.Command.UpdateDynamicSiteSetting;
using Application.DynamicSiteSettings.DTO;
using Application.DynamicSiteSettings.Query.GetAllDynamicSiteSettingsByType;
using Application.DynamicSiteSettings.Query.GetDynamicSiteSettingById;
using Application.DynamicSiteSettings.Query.GetDynamicSiteSettingByTypeAndKey;
using Asp.Versioning;
using Common.GridResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر تنظیمات داینامیک سایت
    /// </summary>
    [ApiVersion("1")]
    public class DynamicSiteSettingController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : BaseController
    {
        /// <summary>
        /// ایجاد تنظیم داینامیک
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult<DynamicSiteSettingResponseDTO>> Create([FromBody] CreateDynamicSiteSettingDTO input) =>
            (await commandDispatcher.SendAsync(new CreateDynamicSiteSettingCommand(input))).ToApiResult();

        /// <summary>
        /// ویرایش تنظیم داینامیک
        /// </summary>
        [HttpPut]
        [Authorize]
        public async Task<ApiResult<DynamicSiteSettingResponseDTO>> Update([FromBody] UpdateDynamicSiteSettingDTO input) =>
            (await commandDispatcher.SendAsync(new UpdateDynamicSiteSettingCommand(input))).ToApiResult();

        /// <summary>
        /// حذف تنظیم داینامیک
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ApiResult> Delete(long id) =>
            (await commandDispatcher.SendAsync(new DeleteDynamicSiteSettingCommand(id))).ToApiResult();

        /// <summary>
        /// دریافت با شناسه
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ApiResult<DynamicSiteSettingResponseDTO>> GetById([FromQuery] long id) =>
            (await queryDispatcher.SendAsync(new GetDynamicSiteSettingByIdQuery(id))).ToApiResult();

        /// <summary>
        /// دریافت با نوع و کلید
        /// </summary>
        [HttpGet("by-type-key")]
        [AllowAnonymous]
        public async Task<ApiResult<DynamicSiteSettingResponseDTO>> GetByTypeAndKey([FromQuery] string type, string key) =>
            (await queryDispatcher.SendAsync(new GetDynamicSiteSettingByTypeAndKeyQuery(type, key))).ToApiResult();

        /// <summary>
        /// دریافت همه تنظیمات یک نوع
        /// </summary>
        [HttpGet("by-type/{type}")]
        [AllowAnonymous]
        public async Task<ApiResult<GlobalGridResult<DynamicSiteSettingResponseDTO>>> GetAllByType([FromQuery] GetAllDynamicSiteSettingsByTypeRequestDto Input) =>
            (await queryDispatcher.SendAsync(new GetAllDynamicSiteSettingsByTypeQuery(Input))).ToApiResult();
    }

}
