using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Application.Excersies.Commands;
using Application.Excersies.DTOs;
using Application.Excersies.Queries;
using Asp.Versioning;
using Common.GridResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر تمرینات
    /// </summary>
    [ApiVersion("1")]
    public class ExcersiesController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : BaseController
    {
        /// <summary>
        /// ساخت تمرین
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> Create([FromBody] CreateExcersiesCommandDTO input) =>
           (await commandDispatcher
            .SendAsync(new CreateExcersiesCommand(input)))
            .ToApiResult();

        /// <summary>
        /// ساخت تمرین
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        public async Task<ApiResult> Update([FromBody] UpdateExcersiesCommandDTO input) =>
           (await commandDispatcher
            .SendAsync(new UpdateExcersiesCommand(input)))
            .ToApiResult();

        /// <summary>
        /// افزودن مرحله به تمرین
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> AddStepToExcersie([FromBody] AddOrRemoveStepToOrFromExcersiesCommandDTO input) =>
           (await commandDispatcher
            .SendAsync(new AddStepToExcersiesCommand(input)))
            .ToApiResult();

        /// <summary>
        /// حذف مرحله از تمرین
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> RemoveStepFromExcersie([FromBody] AddOrRemoveStepToOrFromExcersiesCommandDTO input) =>
           (await commandDispatcher
            .SendAsync(new RemoveStepToExcersiesCommand(input)))
            .ToApiResult();


        /// <summary>
        /// دریافت تمرینات
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult> GetAll([FromQuery] GlobalGrid input) =>
           (await queryDispatcher
            .SendAsync(new GetAllExcersieQuery(input)))
            .ToApiResult();

        /// <summary>
        /// دریافت تمرین
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult> Get([FromQuery] long id) =>
           (await queryDispatcher
            .SendAsync(new GetExcersieQuery(id)))
            .ToApiResult();



        /// <summary>
        /// دریافت تمرین
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        public async Task<ApiResult> Delete([FromQuery] long id) =>
           (await commandDispatcher
            .SendAsync(new DeleteExcersieCommand(id)))
            .ToApiResult();

        //Handle in CreateUserPlanAnswer Method
        ///// <summary>
        ///// اتصال تمرینات به کاربر
        ///// </summary>
        ///// <param name="questionLinkedId"></param>
        ///// <param name="userPlanId"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Authorize]
        //public async Task<ApiResult> AssginExcersieToUser([FromQuery] long questionLinkedId , long userPlanId) =>
        //   (await commandDispatcher
        //    .SendAsync(new AssginExcersieToUserCommand(questionLinkedId, userPlanId)))
        //    .ToApiResult();
    }
}
