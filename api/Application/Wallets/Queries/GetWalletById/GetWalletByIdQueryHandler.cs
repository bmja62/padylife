using Application.Cqrs.Queris;
using Application.Wallets.DTOs;
using Data.Contracts;
using Entities.Wallets;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Wallets.Queries.GetWalletById
{
    public class GetWalletByIdQueryHandler
        (IRepository<Wallet> repository)
        : IQueryHandler<GetWalletByIdQuery, ServiceResult<WalletDTO>>
    {
        public async Task<ServiceResult<WalletDTO>> Handle(GetWalletByIdQuery request, CancellationToken cancellationToken)
        {
            var wallet = await repository.Table
                .Where(x => x.Id == request.WalletId)
                .Select(x => new WalletDTO
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    User = new UserWalletInfoDTO
                    {
                        Id = x.User.Id,
                        FullName = x.User.FullName,
                        PhoneNumber = x.User.PhoneNumber
                    },
                    Credit = x.Credit,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).FirstOrDefaultAsync(cancellationToken);

            if (wallet is null)
                return ServiceResult.BadRequest<WalletDTO>("کیف پول یافت نشد");

            return ServiceResult.Ok(wallet);
        }
    }
}
