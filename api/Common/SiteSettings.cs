namespace Common
{
    public class SiteSettings
    {
        public string ElmahPath { get; set; }
        public JwtSettings JwtSettings { get; set; }
        public IdentitySettings IdentitySettings { get; set; }
        public GoogleSettings GoogleSettings { get; set; }
        public ParbadSettings ParbadSettings { get; set; }
        public  KavenegarOptions KavenegarOptions { get; set; }
    }

    public  class KavenegarOptions
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; } = default!;
        public string DefaultSender { get; set; }
        public string HttpClientName { get; set; }
        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }
    public class IdentitySettings
    {
        public bool PasswordRequireDigit { get; set; }
        public int PasswordRequiredLength { get; set; }
        public bool PasswordRequireNonAlphanumeric { get; set; }
        public bool PasswordRequireUppercase { get; set; }
        public bool PasswordRequireLowercase { get; set; }
        public bool RequireUniqueEmail { get; set; }
    }
    public sealed class SmsSettings
    {
        public string DefaultProvider { get; set; } = "smsir";

        public List<string> FallbackOrder { get; set; }
    }
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string EncryptKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int NotBeforeMinutes { get; set; }
        public int ExpirationMinutes { get; set; }
    }
    public class GoogleSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
    public class ParbadSettings
    {
        public SadadSettings SadadSettings { get; set; }
    }
    public class SadadSettings
    {
        public string MerchantId { get; set; }
        public string TerminalId { get; set; }
        public string TerminalKey { get; set; }
    }
}
