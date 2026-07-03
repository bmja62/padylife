using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Application.Wallets.Commands.ChargeWalletByPayment;
using Application.Wallets.Commands.WalletDeposit;
using Application.Wallets.Commands.WalletWithdraw;
using Application.Wallets.DTOs;
using Application.Wallets.Queries.GetWalletById;
using Application.Wallets.Queries.GetWalletByUserId;
using Application.Wallets.Queries.GetWallets;
using Application.Wallets.Queries.GetWalletTransactions;
using Asp.Versioning;
using Common.GridResults;
using Common.Roles;
using Common.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.WalletsServices;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1;
/// <summary>
/// کنترلر کیف پول ها
/// </summary>
/// <param name="commandDispatcher"></param>
/// <param name="queryDispatcher"></param>
[ApiVersion("1")]
public class WalletsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, IWalletService walletService) : BaseController
{
    /// <summary>
    /// لیست کیف پول ها
    /// </summary>
    /// <param name="input"></param>
    /// <param name="userFullName"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.User}")]
    public async Task<ApiResult<GlobalGridResult<WalletDTO>>> GetWallets([FromQuery] GlobalGrid input, string userFullName, string roleName) =>
        (await queryDispatcher.SendAsync(new GetWalletsQuery(input.PageNumber.Value, input.Count.Value, userFullName, roleName))).ToApiResult();


    /// <summary>
    /// لیست تراکنش های کیف پول ها
    /// </summary>
    /// <param name="input"></param>
    /// <param name="walletId"></param>
    /// <returns></returns>
    [HttpGet("{walletId}")]
    [Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.User}")]
    public async Task<ApiResult<GlobalGridResult<WalletTransactionDTO>>> GetWalletTransactions([FromQuery] GlobalGrid input, long walletId) =>
        (await queryDispatcher.SendAsync(new GetWalletTransactionsQuery(input.PageNumber.Value, input.Count.Value, walletId))).ToApiResult();

    /// <summary>
    /// جزئیات کیف پول
    /// </summary>
    /// <param name="walletId"></param>
    /// <returns></returns>
    [HttpGet("{walletId}")]
    [Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.User}")]
    public async Task<ApiResult<WalletDTO>> GetWalletById(long walletId) =>
        (await queryDispatcher.SendAsync(new GetWalletByIdQuery(walletId))).ToApiResult();

    /// <summary>
    /// جزئیات کیف پول کاربر لاگین کرده
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ApiResult<WalletDTO>> GetMyWallet() =>
        (await queryDispatcher.SendAsync(new GetWalletByUserIdQuery(User.Identity.GetUserId<long>()))).ToApiResult();


    /// <summary>
    /// تاریخچه تراکنش های کیف پول کاربر لاگین کرده
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ApiResult<GlobalGridResult<WalletTransactionDTO>>> GetMyWalletTransactions([FromQuery] GlobalGrid input) =>
      (await queryDispatcher.SendAsync(new GetWalletTransactionsQuery(input.PageNumber.Value, input.Count.Value,
          (await walletService.GetOrCreateByUserId(User.Identity.GetUserId<long>())).Id
          ))).ToApiResult();

    /// <summary>
    /// برداشت از حساب
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.User}")]
    public async Task<ApiResult> Withdraw(WalletWithdrawDTO input) =>
        (await commandDispatcher.SendAsync(new WalletWithdrawCommand(input.WalletId, input.Description, input.Credit))).ToApiResult();

    /// <summary>
    /// واریز به حساب
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = $"{UserRoleNames.Admin},{UserRoleNames.User}")]
    public async Task<ApiResult> Deposit(WalletDepositDTO input) =>
        (await commandDispatcher.SendAsync(new WalletDepositCommand(input.WalletId, input.Description, input.Credit))).ToApiResult();


    ///// <summary>
    ///// شارژ کیف پول با کوپن
    ///// </summary>
    ///// <param name="input"></param>
    ///// <returns></returns>
    //[HttpPost]
    //public async Task<ApiResult> ChargeMyWalletByCoupon(ChargeWalletByCouponDTO input) =>
    //    (await commandDispatcher.SendAsync(new ChargeWalletByCouponCommand(User.Identity.GetUserId<long>(), input.CouponCode))).ToApiResult();


    /// <summary>
    /// لینک درگاه پرداخت برای افزایش موجودی کیف پول
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ApiResult> ChargeMyWallet(long amount) =>
        (await commandDispatcher.SendAsync(new ChargeWalletByPaymentCommand(User.Identity.GetUserId<long>(), amount))).ToApiResult();

}

