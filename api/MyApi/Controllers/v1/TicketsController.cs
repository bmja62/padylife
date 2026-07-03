using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Application.Tickets.Commands.AnswerToTicket;
using Application.Tickets.Commands.CloseTicket;
using Application.Tickets.Commands.CreateTicket;
using Application.Tickets.DTOs;
using Application.Tickets.Queries.GetTicketById;
using Application.Tickets.Queries.GetTicketCounts;
using Application.Tickets.Queries.GetTickets;
using Application.Tickets.Queries.GetUserTickets;
using Asp.Versioning;
using Common.GridResults;
using Common.Utilities;
using Entities.Tickets;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1;

/// <summary>
/// کنترلر تیکت ها
/// </summary>
/// <param name="commandDispatcher"></param>
/// <param name="queryDispatcher"></param>
[ApiVersion("1")]
public class TicketsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : BaseController
{
    /// <summary>
    /// ایجاد تیکت جدید
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost()]
    public async Task<ApiResult> Create(CreateTicketDTO input) =>
        (await commandDispatcher.SendAsync(new CreateTicketCommand(input.Title, input.Content, input.TicketType))).ToApiResult();

    /// <summary>
    /// پاسخ به تیکت
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("[action]")]
    public async Task<ApiResult> Answer(AnswerTicketDTO input) =>
        (await commandDispatcher.SendAsync(new AnswerToTicketCommand(input.Id, input.Content))).ToApiResult();

    /// <summary>
    /// بستن تیکت
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [HttpPost("[action]/{ticketId}")]
    public async Task<ApiResult> Close(long ticketId) =>
        (await commandDispatcher.SendAsync(new CloseTicketCommand(ticketId))).ToApiResult();


    /// <summary>
    /// جزئیات تیکت
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [HttpGet("{ticketId}")]
    public async Task<ApiResult<TicketDTO>> Get(long ticketId) =>
        (await queryDispatcher.SendAsync(new GetTicketByIdQuery(ticketId))).ToApiResult();

    /// <summary>
    /// لیست تیکت ها
    /// </summary>
    /// <param name="input"></param>
    /// <param name="type"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    [HttpGet()]
    public async Task<ApiResult<GlobalGridResult<TicketDTO>>> GetTickets([FromQuery] GlobalGrid input, TicketType? type, TicketStatus? status) =>
        (await queryDispatcher.SendAsync(new GetTicketsQuery(input.PageNumber.Value, input.Count.Value, type, status))).ToApiResult();

    /// <summary>
    /// لیست تیکت های من
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("[action]")]
    public async Task<ApiResult<GlobalGridResult<TicketDTO>>> GetMyTickets([FromQuery] GlobalGrid input) =>
        (await queryDispatcher.SendAsync(new GetUserTicketsQuery(input.PageNumber.Value, input.Count.Value, User.Identity.GetUserId<long>()))).ToApiResult();

    /// <summary>
    /// تعداد تیکت های پاسخ داده شده ی من
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("[action]")]
    public async Task<ApiResult<TicketCountDTO>> GetMyAnsweredTicketCount() =>
        (await queryDispatcher.SendAsync(new GetTicketCountsQuery(User.Identity.GetUserId<long>(), TicketStatus.WaitingForUser))).ToApiResult();

    /// <summary>
    /// تعداد تیکت های در انتظار پاسخ 
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("[action]")]
    public async Task<ApiResult<TicketCountDTO>> GetNotAnsweredTicketCount() =>
        (await queryDispatcher.SendAsync(new GetTicketCountsQuery(null, TicketStatus.WaitingForSupport))).ToApiResult();
}

