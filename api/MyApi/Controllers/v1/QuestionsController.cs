using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Application.Questions.Commands;
using Application.Questions.DTOs;
using Application.Questions.Queries;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر بانک سوالات
    /// </summary>
    [ApiVersion("1")]
    public class QuestionsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : BaseController
    {
        /// <summary>
        /// ساخت سوال
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> Create([FromBody] CreateQuestionCommandDTO input) =>
           (await commandDispatcher
            .SendAsync(new CreateQuestionCommand(input)))
            .ToApiResult();

        /// <summary>
        /// لیست سوالات
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult> GetAll([FromQuery] GetAllFilterDTO input) =>
           (await queryDispatcher
            .SendAsync(new GetAllQuestionQuery(input)))
            .ToApiResult();

        /// <summary>
        /// دریافت سوال
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult> GetBy([FromQuery] long id) =>
           (await queryDispatcher
            .SendAsync(new GetByIdQuestionQuery(id)))
            .ToApiResult();

        /// <summary>
        /// ویرایش سوال
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        public async Task<ApiResult> Update([FromQuery] long id, [FromBody] UpdateQuestionDTO input) =>
           (await commandDispatcher
            .SendAsync(new UpdateQuestionCommand(id, input)))
            .ToApiResult();

        /// <summary>
        /// حذف سوال
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        public async Task<ApiResult> Delete(long id) =>
           (await commandDispatcher
            .SendAsync(new DeleteQuestionCommand(id)))
            .ToApiResult();


        /// <summary>
        /// افزودن پاسخ به سوال
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> AddQuestionOptionToQuestion(AddQuestionOptionToQuestionDTO input) =>
           (await commandDispatcher
            .SendAsync(new AddQuestionOptionToQuestionCommand(input)))
            .ToApiResult();

        /// <summary>
        /// افزودن پاسخ به سوال
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        public async Task<ApiResult> RemoveQuestionOptionToQuestion(long optionId) =>
           (await commandDispatcher
            .SendAsync(new RemoveQuestionOptionToQuestionCommand(optionId)))
            .ToApiResult();

        /// <summary>
        /// ویرایش اتصالات پاسخ یک سوال
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> UpdateQuestionOptionLinkeds(UpdateQuestionOptionLinkedsDTO input) =>
           (await commandDispatcher
            .SendAsync(new UpdateQuestionOptionLinkedsCommand(input)))
            .ToApiResult();


        /// <summary>
        /// ویرایش اتصالات پاسخ یک سوال
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> UpdateQuestionOption(UpdateQuestionOptionDTO input) =>
           (await commandDispatcher
            .SendAsync(new UpdateQuestionOptionCommand(input)))
            .ToApiResult();


    }
}
