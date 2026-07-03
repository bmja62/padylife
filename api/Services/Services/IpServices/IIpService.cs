using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.IpServices
{
    public interface IIpService
    {
        string GetClientIp();
        string GetClientIpForStorage();
        string GetClientIpForTracking();
        string GetRawClientIp();
        bool IsValidIp(string ip);
        string SanitizeIp(string ip);
    }

    public class IpService : IIpService , IScopedDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<IpService> _logger;
        private readonly ISecureIpService _secureIpService;

        public IpService(
            IHttpContextAccessor httpContextAccessor,
            ILogger<IpService> logger,
            ISecureIpService secureIpService)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _secureIpService = secureIpService;
        }

        public string GetClientIp()
        {
            try
            {
                var context = _httpContextAccessor.HttpContext;
                if (context == null) return "Unknown";

                var ip = GetIpFromHeaders(context);
                if (!string.IsNullOrEmpty(ip) && ip != "Unknown")
                    return ip;

                return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client IP");
                return "Unknown";
            }
        }

        public string GetClientIpForStorage()
        {
            // برای ذخیره در دیتابیس: ناشناس‌سازی شده
            var ip = GetClientIp();
            return _secureIpService.ProcessIpForStorage(ip);
        }

        public string GetClientIpForTracking()
        {
            // برای ردیابی: هش شده
            var ip = GetClientIp();
            return _secureIpService.ProcessIpForTracking(ip);
        }

        public string GetRawClientIp()
        {
            // IP اصلی بدون پردازش
            return GetClientIp();
        }

        private string GetIpFromHeaders(HttpContext context)
        {
            var headers = new[] { "X-Forwarded-For", "X-Real-IP", "CF-Connecting-IP", "X-Cluster-Client-IP" };

            foreach (var header in headers)
            {
                if (context.Request.Headers.TryGetValue(header, out var value))
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        var ip = value.ToString().Split(',')[0].Trim();
                        if (IsValidIp(ip))
                        {
                            _logger.LogDebug("IP found from header {Header}: {IP}", header, ip);
                            return ip;
                        }
                    }
                }
            }

            return null;
        }

        public bool IsValidIp(string ip)
        {
            if (string.IsNullOrEmpty(ip) || ip == "Unknown")
                return false;

            return System.Net.IPAddress.TryParse(ip, out _);
        }

        public string SanitizeIp(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return "Unknown";

            // حذف پورت اگر وجود دارد
            var parts = ip.Split(':');
            if (parts.Length == 2 && parts[0].Contains('.')) // IPv4 با پورت
                return parts[0];

            return ip;
        }
    }
}
