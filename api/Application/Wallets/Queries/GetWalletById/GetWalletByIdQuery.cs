using Application.Cqrs.Queris;
using Application.Wallets.DTOs;
using Services;

namespace Application.Wallets.Queries.GetWalletById
{
    public class GetWalletByIdQuery(long walletId) : IQuery<ServiceResult<WalletDTO>>
    {
        public long WalletId { get; } = walletId;
    }
}
