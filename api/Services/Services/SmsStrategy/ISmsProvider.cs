
namespace Services.Services.SmsStrategy
{
    public interface ISmsProvider
    {
        string Name { get; } // "kavenegar" یا "smsir"
        Task SendOtpAsync(string phoneNumber, string otp, CancellationToken ct = default);
        Task SendNotificationAsync(string phoneNumber, string subject, string description, CancellationToken ct = default);
        Task FiveTokenAsync(string phoneNumber, string Token1, string Token2, string Token3, string Token4, string Token5, CancellationToken ct = default);
        Task SendPaymentSuccess(string phoneNumber, string itemName);
        Task SendForgetPassword(string phoneNumber, string resetCode);
        Task SendOTPVerificationCode(string phoneNumber, string resetCode);
    }
}
