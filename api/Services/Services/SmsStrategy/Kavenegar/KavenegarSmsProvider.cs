using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Services.Services.KavenegarServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Services.Services.SmsStrategy.Kavenegar
{
    public class KavenegarSmsProvider : ISmsProvider
    {
        public string Name => "kavenegar";

        private readonly IKavenegarService _kavenegar;
        private readonly KavenegarOptions _opt;
        private readonly IConfiguration _configuration;
        public KavenegarSmsProvider(IKavenegarService kavenegar, IConfiguration configuration,
                                    IOptions<KavenegarOptions> options)
        {
            _kavenegar = kavenegar;
            _opt = options.Value;
            _configuration = configuration;
        }

        public async Task SendOtpAsync(string phoneNumber, string otp, CancellationToken ct = default)
        {
          var  templateId = (_configuration["SiteSettings:Kavenegar:OtpLogin"]);
            await _kavenegar.Lookup(phoneNumber, otp, templateId, ct);
        }

        public async Task SendNotificationAsync(string phoneNumber, string subject, string description, CancellationToken ct = default)
        {
            var text = string.IsNullOrWhiteSpace(subject)
                ? description
                : $"{subject}\n{description}";
            await _kavenegar.Send(phoneNumber, text, _opt.DefaultSender, ct);
        }

        public async Task FiveTokenAsync(string phoneNumber, string Token1, string Token2, string Token3, string Token4, string Token5, CancellationToken ct = default)
        {
            var templateId = (_configuration["SiteSettings:Kavenegar:FiveToken"]);
            await _kavenegar.SendWithTemplate(
                   phoneNumber,
                   templateId,
            ct,
            Token1, Token2, Token3, Token4, Token5
            );
        }

        public Task SendPaymentSuccess(string phoneNumber, string itemName)
        {
            throw new NotImplementedException();
        }

        public Task SendForgetPassword(string phoneNumber, string resetCode)
        {
            throw new NotImplementedException();
        }

        public Task SendOTPVerificationCode(string phoneNumber, string resetCode)
        {
            throw new NotImplementedException();
        }
    }

}
