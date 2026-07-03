namespace Services.Services.SmsStrategy.SmsServices
{
    public interface ISmsService
    {
        Task SendOTP(string phoneNumber, string otp);
        Task SendSMSNotification(string phoneNumber, string subject, string description);
        Task FiveTokenAsync(string phoneNumber, string Token1, string Token2, string Token3, string Token4, string Token5, CancellationToken ct = default);
        Task SendForgotPassword(string phoneNumber, string resetCode);
        Task SendOTPVerificationCode(string phoneNumber, string verificationCode);
        Task SendPaymentSuccess(string phoneNumber, string itemName);
    }
}
