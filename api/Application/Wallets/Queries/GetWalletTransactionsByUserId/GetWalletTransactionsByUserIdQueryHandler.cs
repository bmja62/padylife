using Application.Cqrs.Queris;
using Application.Wallets.DTOs;
using Application.Wallets.Queries.GetWalletTransactions;
using Common.GridResults;
using Services;
using Services.Services.WalletsServices;

namespace Application.Wallets.Queries.GetWalletTransactionsByUserId
{

    public class GetWalletTransactionsByUserIdQueryHandler
        (IWalletService walletService,
        IQueryDispatcher queryDispatcher
        )
        : IQueryHandler<GetWalletTransactionsByUserIdQuery, ServiceResult<GlobalGridResult<WalletTransactionDTO>>>
    {
        public async Task<ServiceResult<GlobalGridResult<WalletTransactionDTO>>> Handle(GetWalletTransactionsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var wallet = await walletService.GetOrCreateByUserId(request.UserId);

            return await queryDispatcher.SendAsync(new GetWalletTransactionsQuery(request.PageNumber.Value, request.Count.Value, wallet.Id), cancellationToken);
        }
    }
}
