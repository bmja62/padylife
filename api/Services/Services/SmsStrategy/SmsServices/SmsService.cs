using Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Services.Services.SmsStrategy.SmsServices
{
    public class SmsService : ISmsService
    {
        private readonly Func<string, ISmsProvider> _resolve; 
        private readonly SmsSettings _settings;

        public SmsService(Func<string, ISmsProvider> resolve,
                          IOptions<SmsSettings> options)
        {
            _resolve = resolve;
            _settings = options.Value;

        }

        private ISmsProvider GetProvider() => _resolve(_settings.DefaultProvider);

        public Task SendOTP(string phoneNumber, string otp)
            => GetProvider().SendOtpAsync(phoneNumber, otp);

        public Task SendSMSNotification(string phoneNumber, string subject, string description)
            => GetProvider().SendNotificationAsync(phoneNumber, subject, description);

        public Task FiveTokenAsync(string phoneNumber, string Token1, string Token2, string Token3, string Token4, string Token5, CancellationToken ct = default)
              => GetProvider().FiveTokenAsync(phoneNumber, Token1, Token2, Token3, Token4, Token5);

        public Task SendForgotPassword(string phoneNumber, string resetCode) => GetProvider().SendForgetPassword(phoneNumber, resetCode);
        public Task SendOTPVerificationCode(string phoneNumber, string verifyCode) => GetProvider().SendOTPVerificationCode(phoneNumber, verifyCode);

        public Task SendPaymentSuccess(string phoneNumber, string itemName) => GetProvider().SendPaymentSuccess(phoneNumber, itemName);
    }

}
