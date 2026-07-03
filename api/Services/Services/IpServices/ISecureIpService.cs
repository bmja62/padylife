using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.IpServices
{
    public interface ISecureIpService
    {
        string HashIp(string ip);
        string AnonymizeIp(string ip);
        string ProcessIpForStorage(string ip);
        string ProcessIpForTracking(string ip);
        bool IsLocalIp(string ip);
    }

    public class SecureIpService : ISecureIpService, IScopedDependency
    {
        private readonly string _ipHashSalt;
        private readonly ILogger<SecureIpService> _logger;

        public SecureIpService(IConfiguration configuration, ILogger<SecureIpService> logger)
        {
            _ipHashSalt = configuration.GetValue<string>("IpHashSalt") ?? "your-default-secret-salt-change-in-production";
            _logger = logger;
        }

        public string HashIp(string ip)
        {
            if (string.IsNullOrEmpty(ip) || ip == "Unknown")
                return "Unknown";

            try
            {
                using var sha256 = SHA256.Create();
                var bytes = Encoding.UTF8.GetBytes(ip + _ipHashSalt);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error hashing IP: {IP}", ip);
                return "Unknown";
            }
        }

        public string AnonymizeIp(string ip)
        {
            if (string.IsNullOrEmpty(ip) || ip == "Unknown")
                return "Unknown";

            try
            {
                // برای IPv4: 192.168.1.100 -> 192.168.1.0
                if (ip.Contains('.') && !ip.Contains(':'))
                {
                    var parts = ip.Split('.');
                    if (parts.Length == 4)
                        return $"{parts[0]}.{parts[1]}.{parts[2]}.0";
                }

                // برای IPv6: 2001:0db8:85a3:0000:0000:8a2e:0370:7334 -> 2001:0db8:85a3::
                if (ip.Contains(':'))
                {
                    var parts = ip.Split(':');
                    if (parts.Length >= 4)
                        return $"{parts[0]}:{parts[1]}:{parts[2]}::";
                }

                return ip;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error anonymizing IP: {IP}", ip);
                return "Unknown";
            }
        }

        public string ProcessIpForStorage(string ip)
        {
            // برای ذخیره در دیتابیس: ناشناس‌سازی
            return AnonymizeIp(ip);
        }

        public string ProcessIpForTracking(string ip)
        {
            // برای ردیابی: هش کردن
            return HashIp(ip);
        }

        public bool IsLocalIp(string ip)
        {
            if (string.IsNullOrEmpty(ip) || ip == "Unknown")
                return false;

            try
            {
                // بررسی IP های local
                var localRanges = new[]
                {
                    "127.0.0.1",
                    "10.",
                    "192.168.",
                    "172.16.",
                    "172.17.",
                    "172.18.",
                    "172.19.",
                    "172.20.",
                    "172.21.",
                    "172.22.",
                    "172.23.",
                    "172.24.",
                    "172.25.",
                    "172.26.",
                    "172.27.",
                    "172.28.",
                    "172.29.",
                    "172.30.",
                    "172.31.",
                    "::1"
                };

                return localRanges.Any(range => ip.StartsWith(range));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking local IP: {IP}", ip);
                return false;
            }
        }
    }
}
