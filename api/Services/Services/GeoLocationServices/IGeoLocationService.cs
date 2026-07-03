using Common;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Services.GeoLocationServices
{
    public interface IGeoLocationService
    {
        Task<GeoLocationResult> GetLocationFromIpAsync(string ipAddress);
        Task<GeoLocationResult> GetLocationFromIpAsync(string ipAddress, string userAgent);
    }

    public class GeoLocationService : IGeoLocationService, IScopedDependency
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<GeoLocationService> _logger;
        private readonly string[] _freeGeoIpApis = {
            "http://ip-api.com/json/{IP}",
            "https://api.ipbase.com/v1/json/{IP}",
            "https://ipapi.co/{IP}/json/"
        };

        public GeoLocationService(
            IHttpClientFactory httpClientFactory,
            IMemoryCache memoryCache,
            ILogger<GeoLocationService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<GeoLocationResult> GetLocationFromIpAsync(string ipAddress)
        {
            return await GetLocationFromIpAsync(ipAddress, null);
        }

        public async Task<GeoLocationResult> GetLocationFromIpAsync(string ipAddress, string userAgent)
        {
            if (string.IsNullOrEmpty(ipAddress) || ipAddress == "Unknown")
                return new GeoLocationResult { IP = ipAddress };

            // بررسی کش
            var cacheKey = $"GeoLocation_{ipAddress}";
            if (_memoryCache.TryGetValue(cacheKey, out GeoLocationResult cachedResult))
            {
                _logger.LogDebug("Geo location found in cache for IP: {IP}", ipAddress);
                return cachedResult;
            }

            // بررسی IPهای داخلی
            if (IsPrivateIp(ipAddress))
            {
                var localResult = new GeoLocationResult
                {
                    IP = ipAddress,
                    Country = "Iran",
                    CountryCode = "IR",
                    Region = "Tehran",
                    City = "Tehran",
                    IsLocal = true
                };

                _memoryCache.Set(cacheKey, localResult, TimeSpan.FromHours(24));
                return localResult;
            }

            // استفاده از سرویس‌های مختلف
            foreach (var apiUrl in _freeGeoIpApis)
            {
                try
                {
                    var result = await FetchFromApi(apiUrl, ipAddress, userAgent);
                    if (result != null && !string.IsNullOrEmpty(result.Country))
                    {
                        _memoryCache.Set(cacheKey, result, TimeSpan.FromHours(24));
                        _logger.LogInformation("Geo location found for IP {IP}: {Country}, {City}",
                            ipAddress, result.Country, result.City);
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to get location from API: {ApiUrl}", apiUrl);
                }
            }

            _logger.LogWarning("Could not determine location for IP: {IP}", ipAddress);
            return new GeoLocationResult { IP = ipAddress };
        }

        private async Task<GeoLocationResult> FetchFromApi(string apiUrl, string ipAddress, string userAgent)
        {
            var url = apiUrl.Replace("{IP}", ipAddress);
            var client = _httpClientFactory.CreateClient();

            if (!string.IsNullOrEmpty(userAgent))
            {
                client.DefaultRequestHeaders.Add("User-Agent", userAgent);
            }

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return ParseGeoResponse(json, apiUrl);
            }

            return null;
        }

        private GeoLocationResult ParseGeoResponse(string json, string apiUrl)
        {
            try
            {
                using var document = JsonDocument.Parse(json);
                var root = document.RootElement;

                if (apiUrl.Contains("ip-api.com"))
                {
                    if (root.GetProperty("status").GetString() == "success")
                    {
                        return new GeoLocationResult
                        {
                            Country = root.GetProperty("country").GetString(),
                            CountryCode = root.GetProperty("countryCode").GetString(),
                            Region = root.GetProperty("regionName").GetString(),
                            City = root.GetProperty("city").GetString(),
                            Latitude = root.GetProperty("lat").GetDouble(),
                            Longitude = root.GetProperty("lon").GetDouble(),
                            ISP = root.GetProperty("isp").GetString(),
                            Organization = root.GetProperty("org").GetString()
                        };
                    }
                }
                else if (apiUrl.Contains("ipapi.co"))
                {
                    return new GeoLocationResult
                    {
                        Country = root.GetProperty("country_name").GetString(),
                        CountryCode = root.GetProperty("country_code").GetString(),
                        Region = root.GetProperty("region").GetString(),
                        City = root.GetProperty("city").GetString(),
                        Latitude = root.GetProperty("latitude").GetDouble(),
                        Longitude = root.GetProperty("longitude").GetDouble(),
                        ISP = root.GetProperty("org").GetString()
                    };
                }
                else if (apiUrl.Contains("ipbase.com"))
                {
                    return new GeoLocationResult
                    {
                        Country = root.GetProperty("country_name").GetString(),
                        CountryCode = root.GetProperty("country_code").GetString(),
                        Region = root.GetProperty("region_name").GetString(),
                        City = root.GetProperty("city").GetString(),
                        Latitude = root.GetProperty("latitude").GetDouble(),
                        Longitude = root.GetProperty("longitude").GetDouble()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse geo response from {ApiUrl}", apiUrl);
            }

            return null;
        }

        private bool IsPrivateIp(string ipAddress)
        {
            try
            {
                var ip = IPAddress.Parse(ipAddress);
                var bytes = ip.GetAddressBytes();

                // IPv4 private ranges
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return (bytes[0] == 10) ||
                           (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) ||
                           (bytes[0] == 192 && bytes[1] == 168) ||
                           (bytes[0] == 127);
                }

                // IPv6 local
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    return ip.IsIPv6LinkLocal || ip.IsIPv6SiteLocal;
                }
            }
            catch
            {
                // اگر IP معتبر نیست
            }

            return false;
        }
    }

    public class GeoLocationResult
    {
        public string IP { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string ISP { get; set; }
        public string Organization { get; set; }
        public bool IsLocal { get; set; }
    }
}
