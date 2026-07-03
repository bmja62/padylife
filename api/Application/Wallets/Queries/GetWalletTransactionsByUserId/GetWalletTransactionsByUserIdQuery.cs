using Application.Cqrs.Queris;
using Application.Wallets.DTOs;
using Common.GridResults;
using Services;

namespace Application.Wallets.Queries.GetWalletTransactionsByUserId
{
    public class GetWalletTransactionsByUserIdQuery : GlobalGrid, IQuery<ServiceResult<GlobalGridResult<WalletTransactionDTO>>>
    {
        public GetWalletTransactionsByUserIdQuery(int pageNumber, int count, long userId)
        {
            PageNumber = pageNumber;
            Count = count;
            UserId = userId;
        }
        public long UserId { get; }
    }
}
