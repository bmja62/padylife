using Entities.Wallets;

namespace Services.Services.WalletsServices
{
    public interface IWalletService
    {
        Task<Wallet> GetOrCreateByUserId(long userId);
        Task<Wallet> GetById(long id);
    }
}
