using Common;
using Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.Services.JwtServices
{
    public class JwtService : IJwtService, IScopedDependency
    {
        private readonly SiteSettings _siteSetting;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly ILogger<JwtService> _logger;

        public JwtService(
            IOptionsSnapshot<SiteSettings> settings,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ILogger<JwtService> logger)
        {
            _siteSetting = settings.Value;
            this.signInManager = signInManager;
            this.userManager = userManager;
            _logger = logger;
        }

        public async Task<AccessToken> GenerateAsync(User user)
        {
            var secretKey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.SecretKey);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

            var encryptionkey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.EncryptKey);
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionkey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            var claims = await GetUserClaimsAsync(user);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _siteSetting.JwtSettings.Issuer,
                Audience = _siteSetting.JwtSettings.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now.AddMinutes(_siteSetting.JwtSettings.NotBeforeMinutes),
                Expires = DateTime.Now.AddMinutes(_siteSetting.JwtSettings.ExpirationMinutes),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateJwtSecurityToken(descriptor);

            _logger.LogInformation("JWT token generated for user {UserId}", user.Id);

            return new AccessToken(securityToken);
        }

        private async Task<List<Claim>> GetUserClaimsAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim("LoginTime", DateTime.UtcNow.ToString("o")),
                new Claim("SessionId", Guid.NewGuid().ToString()) // ایجاد Session ID برای JWT
            };

            // اضافه کردن ایمیل اگر موجود باشد
            if (!string.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }

            // اضافه کردن شماره موبایل اگر موجود باشد
            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                claims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.MobilePhone, "09123456987")); // مقدار پیش‌فرض
            }

            // اضافه کردن نقش‌های کاربر
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                claims.Add(new Claim("role", role)); // اضافه کردن با key ساده
            }

            // اضافه کردن claims از ClaimsFactory
            var factoryClaims = await signInManager.ClaimsFactory.CreateAsync(user);
            claims.AddRange(factoryClaims.Claims);

            // اضافه کردن claims سفارشی
            claims.Add(new Claim("CustomClaim", "CustomValue"));
            claims.Add(new Claim("AppVersion", "1.0.0"));

            _logger.LogDebug("Generated {ClaimCount} claims for user {UserId}", claims.Count, user.Id);

            return claims;
        }

        /// <summary>
        /// بررسی اعتبار توکن JWT
        /// </summary>
        public ClaimsPrincipal ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var secretKey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.SecretKey);
                var encryptionKey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.EncryptKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _siteSetting.JwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _siteSetting.JwtSettings.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    TokenDecryptionKey = new SymmetricSecurityKey(encryptionKey),
                    ClockSkew = TimeSpan.Zero // عدم تحمل خطای زمانی
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "JWT token validation failed");
                throw;
            }
        }

        /// <summary>
        /// استخراج اطلاعات از توکن بدون اعتبارسنجی
        /// </summary>
        public ClaimsPrincipal ReadTokenWithoutValidation(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                if (tokenHandler.CanReadToken(token))
                {
                    var jwtToken = tokenHandler.ReadJwtToken(token);
                    var claims = jwtToken.Claims;

                    return new ClaimsPrincipal(new ClaimsIdentity(claims, "JWT"));
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read JWT token without validation");
                return null;
            }
        }

        /// <summary>
        /// دریافت زمان انقضای توکن
        /// </summary>
        public DateTime? GetTokenExpiration(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                return jwtToken.ValidTo;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get token expiration");
                return null;
            }
        }

        /// <summary>
        /// بررسی اینکه آیا توکن منقضی شده است
        /// </summary>
        public bool IsTokenExpired(string token)
        {
            var expiration = GetTokenExpiration(token);
            return expiration.HasValue && expiration.Value < DateTime.UtcNow;
        }

        /// <summary>
        /// تمدید توکن (ایجاد توکن جدید)
        /// </summary>
        public async Task<AccessToken> RefreshTokenAsync(User user)
        {
            _logger.LogInformation("Refreshing JWT token for user {UserId}", user.Id);
            return await GenerateAsync(user);
        }
    }
}