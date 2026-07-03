using Application.Cqrs.Commands;
using Common.Utilities;
using Data.Contracts;
using Entities.Wallets;
using Microsoft.AspNetCore.Http;
using Services;
using Services.Services.WalletsServices;

namespace Application.Wallets.Commands.WalletWithdraw
{
    public class WalletWithdrawCommandHandler(
        IRepository<Wallet> repository,
        IWalletService walletService,
        IHttpContextAccessor httpContextAccessor)
        : ICommandHandler<WalletWithdrawCommand, ServiceResult>
    {

        public async Task<ServiceResult> Handle(WalletWithdrawCommand request, CancellationToken cancellationToken)
        {

            var userId = httpContextAccessor.HttpContext?.User.Identity.GetUserId<long>();
            var wallet = await walletService.GetById(request.WalletId);
            if (wallet is null)
                return ServiceResult.BadRequest("کیف پول یافت نشد");

            if (wallet.Credit < request.Credit)
                return ServiceResult.BadRequest("مبلغ وارد شده بیشتر از موجودی کیف پول است");


            wallet.Withdraw(request.Credit, userId == 0 ? null : userId, request.Description);
            await repository.UpdateAsync(wallet, cancellationToken);
            return ServiceResult.Ok();
        }
    }
}
