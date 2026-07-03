namespace Application.Wallets.DTOs
{
    public class WalletDTO
    {
        public long Id { get; internal set; }
        public long UserId { get; internal set; }
        public UserWalletInfoDTO User { get; internal set; }
        public decimal Credit { get; internal set; }
        public DateTime CreatedAt { get; internal set; }
        public DateTime? UpdatedAt { get; internal set; }
    }
    public class UserWalletInfoDTO
    {
        public long Id { get; internal set; }
        public string FullName { get; internal set; }
        public string PhoneNumber { get; internal set; }
    }
}
