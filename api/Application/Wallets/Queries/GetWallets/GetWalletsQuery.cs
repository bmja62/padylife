using Application.Cqrs.Queris;
using Application.Wallets.DTOs;
using Common.GridResults;
using Services;

namespace Application.Wallets.Queries.GetWallets
{
    public class GetWalletsQuery : GlobalGrid, IQuery<ServiceResult<GlobalGridResult<WalletDTO>>>
    {
        public GetWalletsQuery(int pageNumber, int count, string userFullName, string roleName)
        {
            PageNumber = pageNumber;
            Count = count;
            UserFullName = userFullName;
            RoleName = roleName;
        }

        public string UserFullName { get; }
        public string RoleName { get; }
    }
}
