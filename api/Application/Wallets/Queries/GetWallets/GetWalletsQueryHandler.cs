using Application.Cqrs.Queris;
using Application.Wallets.DTOs;
using Common.GridResults;
using Common.Utilities;
using Data.Contracts;
using Entities.Wallets;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Wallets.Queries.GetWallets
{
    public class GetWalletsQueryHandler(
        IRepository<Wallet> repository)
        : IQueryHandler<GetWalletsQuery, ServiceResult<GlobalGridResult<WalletDTO>>>
    {
        public async Task<ServiceResult<GlobalGridResult<WalletDTO>>> Handle(GetWalletsQuery request, CancellationToken cancellationToken)
        {
            var query = repository.Table.AsQueryable();

            if (request.UserFullName.HasValue())
                query = query.Where(x =>
                (x.User.FullName).Contains(request.UserFullName) ||
                x.User.UserRoles.Any(a => a.Role.Name.Contains(request.UserFullName)) ||
                x.User.UserRoles.Any(a => a.Role.Description.Contains(request.UserFullName))

                );

            if (request.RoleName.HasValue())
                query = query.Where(x =>
                x.User.UserRoles.Any(a => a.Role.Name.Contains(request.RoleName)) ||
                x.User.UserRoles.Any(a => a.Role.Description.Contains(request.RoleName))
                );



            var data = await query
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
                })
                .Skip((request.PageNumber.Value - 1) * request.Count.Value)
                .Take(request.Count.Value)
                .ToListAsync();

            var totalCount = await query.CountAsync();

            return ServiceResult.Ok(new GlobalGridResult<WalletDTO>
            {
                Data = data,
                TotalCount = totalCount
            });
        }
    }
}
