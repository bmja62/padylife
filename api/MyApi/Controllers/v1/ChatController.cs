using Application.Chats.Dto;
using Application.Chats.Queries.GetAllChatForAdmin;
using Application.Chats.Queries.GetChatById;
using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Asp.Versioning;
using Common.GridResults;
using Common.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Hubs.DTOs;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// چت ها
    /// </summary>
    /// <param name="commandDispatcher"></param>
    /// <param name="queryDispatcher"></param>
    [ApiVersion("1")]
    public class ChatController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : BaseController
    {

        /// <summary>
        /// لیست تمام چت ها
        /// </summary>
        /// <param name="Dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = $"{UserRoleNames.Admin}")]
        [AllowAnonymous]
        public async Task<ApiResult<GlobalGridResult<LoadChatsForAdminViewModel>>> GetAllChatForAdmin([FromQuery] GetAllChatRequestDto Dto) =>
    (await queryDispatcher.SendAsync(new GetAllChatForAdminQuery(Dto)))
    .ToApiResult();

        /// <summary>
        /// پیام های چت انتخابی
        /// </summary>
        /// <param name="Dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = $"{UserRoleNames.Admin}")]
        [AllowAnonymous]
        public async Task<ApiResult<GlobalGridResult<ChatMessageViewModel>>> GetChatByIdQueryForAdmin([FromQuery] ChatByIdRequestDto Dto) =>
(await queryDispatcher.SendAsync(new GetChatByIdQuery(Dto)))
.ToApiResult();


    }
}
