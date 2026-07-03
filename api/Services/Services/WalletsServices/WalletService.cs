using Common;
using Data.Contracts;
using Entities.Wallets;
using Microsoft.EntityFrameworkCore;

namespace Services.Services.WalletsServices
{
    public class WalletService
        (IRepository<Wallet> repository) :
        IWalletService, IScopedDependency
    {
        public async Task<Wallet> GetById(long id)
        {
            var wallet = await repository.Table.Where(x => x.Id == id).FirstOrDefaultAsync();
            return wallet;
        }

        public async Task<Wallet> GetOrCreateByUserId(long userId)
        {
            var wallet = await repository.Table.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            if (wallet is null)
            {
                wallet = new Wallet(userId);
                await repository.AddAsync(wallet, CancellationToken.None, true);
            }
            return wallet;
        }
    }
}
