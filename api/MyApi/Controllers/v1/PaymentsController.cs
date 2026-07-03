using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Application.Payments.Commands.PayByWallet;
using Application.Payments.Commands.Verify;
using Application.Payments.DTOs;
using Application.Payments.Queries.GetPaymentLink;
using Application.Payments.Queries.GetPayments;
using Asp.Versioning;
using Common.GridResults;
using Common.Roles;
using Common.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.PaymentServices.DTOs;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1;

/// <summary>
/// پرداخت ها
/// </summary>
/// <param name="commandDispatcher"></param>
/// <param name="queryDispatcher"></param>

[ApiVersion("1")]
public class PaymentsController(
    ICommandDispatcher commandDispatcher,
    IQueryDispatcher queryDispatcher,
    IConfiguration configuration
    ) : BaseController
{
    /// <summary>
    /// لیست پرداخت ها
    /// </summary>
    /// <param name="input"></param>
    /// <param name="isPayed"></param>
    /// <param name="userFullName"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Roles = $"{UserRoleNames.Admin}")]
    public async Task<ApiResult<GlobalGridResult<PaymentDTO>>> GetPayments(
        [FromQuery] GlobalGrid input, bool? isPayed,
        string userFullName, DateTime? from, DateTime? to, long? userId) =>
        (await queryDispatcher.SendAsync(new GetPaymentsQuery(input.PageNumber.Value, input.Count.Value, isPayed, from, to, userFullName, userId))).ToApiResult();

    /// <summary>
    /// لیست پرداخت ها
    /// </summary>
    /// <param name="input"></param>
    /// <param name="isPayed"></param>
    /// <param name="userFullName"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    [HttpGet("[action]")]
    public async Task<ApiResult<GlobalGridResult<PaymentDTO>>> GetMyPayments(
        [FromQuery] GlobalGrid input, bool? isPayed,
        string userFullName, DateTime? from, DateTime? to) =>
        (await queryDispatcher.SendAsync(new GetPaymentsQuery(input.PageNumber.Value, input.Count.Value, true, from, to, userFullName, User.Identity.GetUserId<long>()))).ToApiResult();
    

    [HttpGet("[action]")]
    public async Task<ApiResult<PaymentInvoiceDTO>> GetPaymentLink(
        [FromQuery] long OrderId) =>
        (await commandDispatcher.SendAsync(new GetPaymentLinkQuery(OrderId))).ToApiResult(); 
    
    /// <summary>
    /// پرداخت با کیف پول
    /// </summary>
    /// <param name="OrderId"></param>
    /// <returns></returns>
    [HttpPost("[action]")]
    public async Task<ApiResult<WalletPaymentResultDTO>> PayWithWallet(
        [FromQuery] long OrderId) =>
        (await commandDispatcher.SendAsync(new PayWithWalletCommand(OrderId))).ToApiResult();
    /// <summary>
    /// وریفای پرداخت
    /// </summary>
    /// <param name="trackId"></param>
    /// <returns></returns>
    [HttpGet("Verify")]
    [AllowAnonymous]
    public async Task<IActionResult> Verify([FromQuery] long trackId)
    {
        var clientUrl = configuration["FrontendUrl"];

        var verifyResult = await commandDispatcher.SendAsync(new VerifyCommand(trackId));
        if (!verifyResult.IsSuccess)
        {
            var redirentTo = $"{clientUrl}/dashboard/payments/failed";
            return Redirect(redirentTo);
        }
        else if (verifyResult.Data.WalletCharge)
        {
            var redirentTo = $"{clientUrl}/dashboard/user/points";
            return Redirect(redirentTo);
        }
        else
        {
            var redirentTo = $"{clientUrl}/dashboard/payments/success";
            return Redirect(redirentTo);
        }   
        throw new NotImplementedException();
    }
}
