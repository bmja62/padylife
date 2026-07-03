using Common;
using IPE.SmsIrClient.Models.Requests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.Services.SmsStrategy;

namespace Services.Services.SmsStrategy.SmsIrs
{
    public class SmsIrProvider
        : ISmsProvider, IScopedDependency
    {
        private readonly ILogger<SmsIrProvider> _logger;
        private readonly IConfiguration _configuration;

        public string Name => "smsir";

        public SmsIrProvider(ILogger<SmsIrProvider> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        private async Task SendFast(string phoneNumber, VerifySendParameter[] parameters, int templateId)
        {
            try
            {
                var token = _configuration["SiteSettings:SmsIrSetting:ApiKey"];
                IPE.SmsIrClient.SmsIr smsIr = new(token);
                var result = await smsIr.VerifySendAsync(phoneNumber, templateId, parameters);
                _logger.LogInformation("Send sms -> PhoneNumber= {PhoneNumber} and  Status = {Status} and Message= {Message}", phoneNumber, result.Status, result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public async Task SendOtpAsync(string phoneNumber, string otp, CancellationToken ct = default)
        {
            _ = int.TryParse(_configuration["SiteSettings:SmsIrSetting:OtpLogin"], out int templateId);
            var parameters = new VerifySendParameter[]
               {
                    new("OTPCode",otp)
               };

            await SendFast(phoneNumber, parameters, templateId);
        }

        public async Task SendNotificationAsync(string phoneNumber, string subject, string description, CancellationToken ct = default)
        {
            _ = int.TryParse(_configuration["SiteSettings:SmsIrSetting:SendSMSNotification"], out int templateId);
            var parameters = new VerifySendParameter[]
            {
               new("subject", subject),
                new("desc", description)
            };

            await SendFast(phoneNumber, parameters, templateId);
        }

        public Task FiveTokenAsync(string phoneNumber, string Token1, string Token2, string Token3, string Token4, string Token5, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task SendPaymentSuccess(string phoneNumber, string itemName)
        {
            _ = int.TryParse(_configuration["SiteSettings:SmsIrSetting:PaymentSuccesspady"], out int templateId);
            var parameters = new VerifySendParameter[]
            {
               new("Name", itemName)
            };
            await SendFast(phoneNumber, parameters, templateId);
        }

        public async Task SendForgetPassword(string phoneNumber, string resetCode)
        {
            _ = int.TryParse(_configuration["SiteSettings:SmsIrSetting:ForgotPassword"], out int templateId);
            var parameters = new VerifySendParameter[]
            {
               new("VerificationCode", resetCode)
            };
            await SendFast(phoneNumber, parameters, templateId);
        }

        public async Task SendOTPVerificationCode(string phoneNumber, string verifyCode)
        {
            _ = int.TryParse(_configuration["SiteSettings:SmsIrSetting:VerificationCode"], out int templateId);
            var parameters = new VerifySendParameter[]
            {
               new("VerificationCode", verifyCode)
            };
            await SendFast(phoneNumber, parameters, templateId);
        }
    }
}
