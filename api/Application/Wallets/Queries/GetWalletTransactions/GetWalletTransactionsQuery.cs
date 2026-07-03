using Application.Cqrs.Queris;
using Application.Wallets.DTOs;
using Common.GridResults;
using Services;

namespace Application.Wallets.Queries.GetWalletTransactions
{
    public class GetWalletTransactionsQuery : GlobalGrid, IQuery<ServiceResult<GlobalGridResult<WalletTransactionDTO>>>
    {
        public GetWalletTransactionsQuery(int pageNumber, int count, long walletId)
        {
            PageNumber = pageNumber;
            Count = count;
            WalletId = walletId;
        }
        public long WalletId { get; }
    }
}
