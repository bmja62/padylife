using Common;
using Common.Utilities;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Services.Services.EmailSenderServices.DTOs;
using Services.Services.EmailTemplateService;

namespace Services.Services.EmailSenderServices
{
    public class EmailSender : IEmailSender, IScopedDependency
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailTemplateService _emailTemplateService;
        public EmailSender(IConfiguration configuration, IEmailTemplateService emailTemplateService)
        {
            _configuration = configuration;
            _emailTemplateService = emailTemplateService;
        }

        public async Task<ServiceResult> Send(string email, string subject, string body)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(body) && email.IsEmail())
            {
                var config = _configuration.GetSection("EmailConfig").Get<EmailConfigDTO>();

                MimeMessage message = new MimeMessage();
                MailboxAddress from = new MailboxAddress("Ferman", config.SenderEmail);
                message.From.Add(from);

                MailboxAddress to = new MailboxAddress(email, email);
                message.To.Add(to);

                message.Subject = subject;

                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = body;
                message.Body = bodyBuilder.ToMessageBody();

                SmtpClient client = new SmtpClient();

                client.Connect(config.HostIp, config.Port, config.IsSSL);
                client.Authenticate(config.Username, config.Password);

                await client.SendAsync(message);
                client.Disconnect(true);
                client.Dispose();

                return ServiceResult.Ok();
            }

            return ServiceResult.Fail("لطفا کلیه پارامتر ها را ارسال کنید");
        }


    }
}
