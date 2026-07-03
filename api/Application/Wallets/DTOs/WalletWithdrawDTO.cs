using System.ComponentModel.DataAnnotations;

namespace Application.Wallets.DTOs
{
    public class WalletWithdrawDTO
    {
        public long WalletId { get; set; }
        public string Description { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "نمی توانید مبلغ منفی ارسال کنید")]
        public decimal Credit { get; set; }
    }
    public class WalletDepositDTO
    {
        public long WalletId { get; set; }
        public string Description { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "نمی توانید مبلغ منفی ارسال کنید")]
        public decimal Credit { get; set; }
    }

    public class ChargeWalletByCouponDTO
    {
        public string CouponCode { get; set; }
    }
}
