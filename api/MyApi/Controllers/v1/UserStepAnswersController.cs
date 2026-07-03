using Application.Cqrs.Commands;
using Application.UserStepAnswers.Commands;
using Application.UserStepAnswers.DTOs;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر ثبت پاسخ‌های کاربران
    /// </summary>
    [ApiVersion("1")]
    public class UserStepAnswersController(ICommandDispatcher commandDispatcher) : BaseController
    {
        /// <summary>
        /// ثبت پاسخ کاربر به یک StepOption
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> SubmitAnswer([FromBody] CreateUserStepAnswerCommandDTO input)
        {
            var result = await commandDispatcher.SendAsync(new CreateUserStepAnswerCommand(input));
            return result.ToApiResult();
        }

        /// <summary>
        /// ثبت پاسخ کاربر به یک StepOption
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> HasSubmitAnswer([FromBody] CreateUserStepAnswerCommandDTO input)
        {
            var result = await commandDispatcher.SendAsync(new HasSubmitAnswerCommand(input));
            return result.ToApiResult();
        }
    }
}
