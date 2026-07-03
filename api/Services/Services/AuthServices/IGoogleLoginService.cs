using Common;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using Services.Services.AuthServices.DTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.AuthServices
{
    /// <summary>
    /// سرویس احراز هویت از طریق گوگل
    /// </summary>
    public interface IGoogleLoginService
    {
        /// <summary>
        /// احراز هویت کاربر با استفاده از Credential دریافتی از Google One Tap
        /// </summary>
        Task<ServiceResult<GoogleUserInfoDto>> VerifyGoogleTokenAsync(string credential);
    }
   

    public class GoogleLoginService : IGoogleLoginService ,IScopedDependency
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GoogleLoginService> _logger;

        public GoogleLoginService(IConfiguration configuration, ILogger<GoogleLoginService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<ServiceResult<GoogleUserInfoDto>> VerifyGoogleTokenAsync(string credential)
        {
            var clientId = _configuration["SiteSettings:GoogleSettings:ClientId"];
            if (string.IsNullOrEmpty(clientId))
                return ServiceResult.BadRequest<GoogleUserInfoDto>("ClientId در تنظیمات یافت نشد");

            _logger.LogInformation("[VerifyGoogleToken] ClientId استفاده شده: {ClientId}", clientId);

            GoogleJsonWebSignature.Payload? payload = null;

            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                };

                payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);
                _logger.LogInformation("[VerifyGoogleToken] توکن با موفقیت توسط گوگل تأیید شد (Online Mode)");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden || ex.InnerException is SocketException)
            {
                _logger.LogWarning("[VerifyGoogleToken] گوگل در دسترس نیست - استفاده از حالت آفلاین");
                payload = ValidateLocally(credential, clientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[VerifyGoogleToken] خطا در تماس با گوگل - استفاده از حالت آفلاین");
                payload = ValidateLocally(credential, clientId);
            }

            if (payload == null)
                return ServiceResult<GoogleUserInfoDto>.BadRequest<GoogleUserInfoDto>("توکن گوگل معتبر نیست");

            var result = new GoogleUserInfoDto
            {
                Email = payload.Email,
                Name = payload.Name,
                Picture = payload.Picture,
                GivenName = payload.GivenName,
                FamilyName = payload.FamilyName
            };

            return ServiceResult<GoogleUserInfoDto>.Ok(result);
        }

        private GoogleJsonWebSignature.Payload? ValidateLocally(string token, string clientId)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(token))
                {
                    _logger.LogWarning("[ValidateLocally] فرمت توکن نامعتبر است");
                    return null;
                }

                var jwt = handler.ReadJwtToken(token);

                var payload = new GoogleJsonWebSignature.Payload
                {
                    Issuer = jwt.Claims.FirstOrDefault(c => c.Type == "iss")?.Value,
                    Audience = jwt.Claims.FirstOrDefault(c => c.Type == "aud")?.Value,
                    Subject = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value,
                    Email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value,
                    EmailVerified = bool.TryParse(jwt.Claims.FirstOrDefault(c => c.Type == "email_verified")?.Value, out var verified) && verified,
                    Name = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value,
                    Picture = jwt.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,
                    GivenName = jwt.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value,
                    FamilyName = jwt.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value
                };

                if (payload.Issuer != "https://accounts.google.com" && payload.Issuer != "accounts.google.com")
                    return null;

                // اگر Audience ممکنه آرایه باشه
                var audiences = payload.Audience switch
                {
                    string s => new[] { s },
                    string[] arr => arr,
                    object o => new[] { o.ToString() },
                    _ => Array.Empty<string>()
                };

                if (!audiences.Any(a => a.Trim() == clientId.Trim()))
                {
                    _logger.LogWarning("[VerifyGoogleToken] Audience نامعتبر است: {Audience}", string.Join(",", audiences));
                    return null;
                }

                if (jwt.ValidTo < DateTime.UtcNow)
                    return null;

                if (!payload.EmailVerified)
                    return null;

                _logger.LogInformation("[ValidateLocally] توکن به‌صورت محلی معتبر است ✅");
                return payload;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ValidateLocally] خطا در بررسی محلی توکن");
                return null;
            }
        }
    }
}

