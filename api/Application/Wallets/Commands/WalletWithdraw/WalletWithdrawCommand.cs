using Application.Cqrs.Commands;
using Services;

namespace Application.Wallets.Commands.WalletWithdraw
{
    public class WalletWithdrawCommand(long walletId, string description, decimal credit) : ICommand<ServiceResult>
    {
        public long WalletId { get; } = walletId;
        public string Description { get; } = description;
        public decimal Credit { get; } = credit;
    }
}
