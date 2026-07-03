using Application.Cqrs.Queris;
using Application.Wallets.DTOs;
using Application.Wallets.Queries.GetWalletById;
using Services;
using Services.Services.WalletsServices;

namespace Application.Wallets.Queries.GetWalletByUserId
{
    public class GetWalletByUserIdQueryHandler
        (IWalletService walletService,
        IQueryDispatcher queryDispatcher
        )
        : IQueryHandler<GetWalletByUserIdQuery, ServiceResult<WalletDTO>>
    {
        public async Task<ServiceResult<WalletDTO>> Handle(GetWalletByUserIdQuery request, CancellationToken cancellationToken)
        {
            var wallet = await walletService.GetOrCreateByUserId(request.UserId);

            return await queryDispatcher.SendAsync(new GetWalletByIdQuery(wallet.Id), cancellationToken);
        }
    }
}
