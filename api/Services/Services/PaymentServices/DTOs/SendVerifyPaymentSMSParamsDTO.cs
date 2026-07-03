namespace Services.Services.PaymentServices.DTOs
{
    public class SendVerifyPaymentSMSParamsDTO
    {
        public string PhoneNumber { get; internal set; }
        public long? OrderId { get; internal set; }
    }
}
