using Application.Calenders.Command;
using Application.Calenders.Dtos;
using Application.Calenders.Query;
using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر تقویم
    /// </summary>
    /// <param name="commandDispatcher"></param>
    /// <param name="queryDispatcher"></param>
    [ApiVersion("1")]
    public class CalendarManagerController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : BaseController
    {
        /// <summary>
        /// دریافت گرید ماه شمسی (۵ هفته/۳۵ روز) به همراه مناسبت‌ها و رویدادهای کاربر (در صورت لاگین بودن)
        /// </summary>
        /// <param name="input">پارامترهای سال/ماه شمسی</param>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResult<GetMonthCalendarResponseDto>> GetMonth([FromQuery] GetMonthCalendarRequestDto input) =>
            (await queryDispatcher
                .SendAsync(new GetMonthCalendarQuery(input)))
            .ToApiResult();

        /// <summary>
        /// دریافت رویدادهای کاربر در یک روز مشخص
        /// </summary>
        /// <param name="date">تاریخ میلادی (فقط بخش Date خوانده می‌شود)</param>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<List<CalendarEventResponseDto>>> GetEventsByDay([FromQuery] DateTime date) =>
            (await queryDispatcher
                .SendAsync(new GetCalendarEventsByDayQuery(date)))
            .ToApiResult();

        /// <summary>
        /// ایجاد رویداد جدید برای کاربر جاری
        /// </summary>
        /// <param name="input">اطلاعات رویداد</param>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult<CalendarEventResponseDto>> CreateEvent([FromBody] CreateCalendarEventDto input) =>
            (await commandDispatcher
                .SendAsync(new CreateCalendarEventCommand(input)))
            .ToApiResult();

        /// <summary>
        /// ویرایش رویداد کاربر جاری
        /// </summary>
        /// <param name="input">اطلاعات ویرایش رویداد</param>
        [HttpPut]
        [Authorize]
        public async Task<ApiResult<CalendarEventResponseDto>> UpdateEvent([FromBody] UpdateCalendarEventDto input) =>
            (await commandDispatcher
                .SendAsync(new UpdateCalendarEventCommand(input)))
            .ToApiResult();

        /// <summary>
        /// حذف نرم (Soft Delete) رویداد کاربر جاری
        /// </summary>
        /// <param name="id">شناسه‌ی رویداد</param>
        [HttpDelete]
        [Authorize]
        public async Task<ApiResult> DeleteEvent([FromQuery] long id) =>
            (await commandDispatcher
                .SendAsync(new DeleteCalendarEventCommand(id)))
            .ToApiResult();
    }

}
