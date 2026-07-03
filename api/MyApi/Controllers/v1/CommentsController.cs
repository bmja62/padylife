using Application.Comments.Commands;
using Application.Comments.DTOs;
using Application.Comments.Queries;
using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Asp.Versioning;
using Common.GridResults;
using Entities.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.CommentServices.DTOs;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر کامنت
    /// </summary>
    [ApiVersion("1")]
    public class CommentsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : BaseController
    {
        /// <summary>
        /// ثبت کامنت
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> Create([FromBody] CreateCommentDTO input) =>
            (await commandDispatcher.SendAsync(new CreateCommentCommand(input))).ToApiResult();

        /// <summary>
        /// ریپلای به کامنت
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> Reply([FromBody] CreateCommentDTO input) =>
            (await commandDispatcher.SendAsync(new ReplyCommentCommand(input))).ToApiResult();

        /// <summary>
        /// حذف کامنت
        /// </summary>
        [HttpDelete("{commentId}")]
        [Authorize]
        public async Task<ApiResult> Delete(long commentId) =>
            (await commandDispatcher.SendAsync(new DeleteCommentCommand(commentId))).ToApiResult();

        /// <summary>
        /// ویرایش کامنت
        /// </summary>
        [HttpPut]
        [Authorize]
        public async Task<ApiResult> Edit([FromBody] EditCommentDTO input) =>
            (await commandDispatcher.SendAsync(new EditCommentCommand(input))).ToApiResult();

        /// <summary>
        /// تایید کامنت توسط ادمین
        /// </summary>
        [HttpPut("{commentId}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ApiResult> Approve(long commentId) =>
            (await commandDispatcher.SendAsync(new ApproveCommentCommand(commentId))).ToApiResult();


        /// <summary>
        /// واکنش به کامنت
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> ReactToComment(long commentId, [FromBody] ReactionDTO input) =>
            (await commandDispatcher.SendAsync(new ReactToCommentCommand(commentId, input.type))).ToApiResult();

        /// <summary>
        /// دریافت لیست کامنت‌های یک انتیتی
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<GlobalGridResult<GetCommentDTO>>> GetEntityComments([FromQuery] long? entityId, [FromQuery] bool? isApproved, [FromQuery] EntityType entityType, [FromQuery] GlobalGrid globalGrid) =>
            (await queryDispatcher.SendAsync(new GetEntityCommentsQuery(entityId, isApproved, entityType, globalGrid))).ToApiResult();


        /// <summary>
        /// دریافت لیست کامنت‌های یک انتیتی
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="isApproved"></param>
        /// <param name="entityType"></param>
        /// <param name="globalGrid"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ApiResult<GlobalGridResult<GetCommentDTO>>> GetEntityCommentsForAdmin([FromQuery] long? entityId, [FromQuery] bool? isApproved, [FromQuery] EntityType? entityType, [FromQuery] GlobalGrid globalGrid) =>
          (await queryDispatcher.SendAsync(new GetEntityCommentsQuery(entityId, isApproved, entityType, globalGrid))).ToApiResult();
    }
}
