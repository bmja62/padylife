namespace Application.Payments.DTOs
{
    public class PaymentDTO
    {
        public long UserId { get; set; }
        public PaymentUserDTO User { get; set; }
        public long? OrderId { get; set; }
        public decimal Amount { get; set; }
        public bool IsPayed { get; set; }
        public DateTime? PayedAt { get; set; }
        public DateTime CreatedAt { get; internal set; }
        public bool WalletCharge { get; set; }
    }

    public class PaymentUserDTO
    {
        public long Id { get; internal set; }
        public string FullName { get; internal set; }
        public string PhoneNumber { get; internal set; }
    }
}
