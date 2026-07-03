using Application.Cqrs.Queris;
using Application.Wallets.DTOs;
using Services;

namespace Application.Wallets.Queries.GetWalletByUserId
{
    public class GetWalletByUserIdQuery(long userId) : IQuery<ServiceResult<WalletDTO>>
    {
        public long UserId { get; } = userId;
    }
}
