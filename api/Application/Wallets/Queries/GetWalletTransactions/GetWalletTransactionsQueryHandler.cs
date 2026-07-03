using Application.Cqrs.Queris;
using Application.Wallets.DTOs;
using Common.GridResults;
using Data.Contracts;
using Entities.Wallets;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Wallets.Queries.GetWalletTransactions
{
    public class GetWalletTransactionsQueryHandler(
        IRepository<WalletTransaction> repository)
        : IQueryHandler<GetWalletTransactionsQuery, ServiceResult<GlobalGridResult<WalletTransactionDTO>>>
    {
        public async Task<ServiceResult<GlobalGridResult<WalletTransactionDTO>>> Handle(GetWalletTransactionsQuery request, CancellationToken cancellationToken)
        {
            var query = repository.Table.Where(x => x.WalletId == request.WalletId);

            var data = await query.OrderByDescending(x => x.Id)
                .Select(x => new WalletTransactionDTO
                {
                    Id = x.Id,
                    WalletId = x.WalletId,
                    Amount = x.Amount,
                    Description = x.Description,
                    CreatedAt = x.CreatedAt,
                    Operation = x.Operation,
                    CreatedByUser = x.CreatedByUser == null ? null : new UserWalletInfoDTO
                    {
                        Id = x.CreatedByUser.Id,
                        FullName = x.CreatedByUser.FullName,
                        PhoneNumber = x.CreatedByUser.PhoneNumber

                    },
                })
                .Skip((request.PageNumber.Value - 1) * request.Count.Value)
                .Take(request.Count.Value)
                .ToListAsync(cancellationToken);

            var totalCount = await query.CountAsync(cancellationToken);

            return ServiceResult.Ok(new GlobalGridResult<WalletTransactionDTO>
            {
                Data = data,
                TotalCount = totalCount
            });
        }
    }
}
