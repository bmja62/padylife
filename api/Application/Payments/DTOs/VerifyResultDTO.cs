namespace Application.Payments.DTOs
{
    public class VerifyResultDTO
    {
        public long? InsurancePaperId { get; set; }
        public bool WalletCharge { get; set; }
        public bool CouponPurchase { get; internal set; }
        public long OrderId { get; internal set; }
    }
}
