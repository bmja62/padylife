using Entities.Wallets;

namespace Application.Wallets.DTOs
{
    public class WalletTransactionDTO
    {
        public long Id { get; internal set; }
        public long WalletId { get; internal set; }
        public decimal Amount { get; internal set; }
        public string Description { get; internal set; }
        public DateTime CreatedAt { get; internal set; }
        public UserWalletInfoDTO CreatedByUser { get; internal set; }
        public WalletTransactionOperation Operation { get; internal set; }
    }
}
