using Common;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Services.Hubs;
using Services.Services.GeoLocationServices;
using System.Collections.Concurrent;

namespace Services.Services.OnlineUsersServices
{
    public interface IOnlineUsersService
    {
        Task<int> GetOnlineUsersCountAsync();
        Task<List<OnlineUserDto>> GetOnlineUsersAsync();
        Task<List<OnlineUserDto>> GetActiveUsersAsync(TimeSpan? timeWindow = null);
        Task<OnlineUserStatsDto> GetOnlineUsersStatsAsync();
        Task UpdateUserActivityAsync(string userId, string pageUrl, string pageTitle, string userAgent = null, string ipAddress = null);
        Task UserConnectedAsync(string connectionId, string userId, string userAgent, string ipAddress);
        Task UserDisconnectedAsync(string connectionId);
    }

    public class OnlineUsersService : IOnlineUsersService, IScopedDependency
    {
        // ذخیره static برای کاربران آنلاین
        private static readonly ConcurrentDictionary<string, OnlineUserDto> _onlineUsers = new();
        private readonly IHubContext<OnlineUsersHub, IOnlineUsersHub> _hubContext;
        private readonly IGeoLocationService _geoLocationService;
        private readonly ILogger<OnlineUsersService> _logger;

        public OnlineUsersService(
            IHubContext<OnlineUsersHub, IOnlineUsersHub> hubContext,
            IGeoLocationService geoLocationService,
            ILogger<OnlineUsersService> logger)
        {
            _hubContext = hubContext;
            _geoLocationService = geoLocationService;
            _logger = logger;
        }

        public Task<int> GetOnlineUsersCountAsync()
        {
            try
            {
                var count = _onlineUsers.Values
                    .Count(u => u.LastActivity > DateTime.UtcNow.AddMinutes(-5));
                return Task.FromResult(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting online users count");
                return Task.FromResult(0);
            }
        }

        public Task<List<OnlineUserDto>> GetOnlineUsersAsync()
        {
            try
            {
                var users = _onlineUsers.Values
                    .Where(u => u.LastActivity > DateTime.UtcNow.AddMinutes(-5))
                    .ToList();
                return Task.FromResult(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting online users");
                return Task.FromResult(new List<OnlineUserDto>());
            }
        }

        public async Task<List<OnlineUserDto>> GetActiveUsersAsync(TimeSpan? timeWindow = null)
        {
            var window = timeWindow ?? TimeSpan.FromMinutes(5);
            var users = await GetOnlineUsersAsync();
            return users.Where(u => u.LastActivity > DateTime.UtcNow.Subtract(window)).ToList();
        }

        public async Task UpdateUserActivityAsync(string userId, string pageUrl, string pageTitle, string userAgent = null, string ipAddress = null)
        {
            try
            {
                var userKey = userId ?? Guid.NewGuid().ToString();

                if (_onlineUsers.TryGetValue(userKey, out var user))
                {
                    user.LastActivity = DateTime.UtcNow;
                    user.CurrentPage = pageUrl;
                    user.PageTitle = pageTitle;

                    if (!string.IsNullOrEmpty(userAgent))
                        user.UserAgent = userAgent;
                    if (!string.IsNullOrEmpty(ipAddress))
                        user.IpAddress = ipAddress;
                }
                else
                {
                    var newUser = new OnlineUserDto
                    {
                        UserId = userKey,
                        CurrentPage = pageUrl,
                        PageTitle = pageTitle,
                        UserAgent = userAgent,
                        IpAddress = ipAddress,
                        ConnectedAt = DateTime.UtcNow,
                        LastActivity = DateTime.UtcNow
                    };
                    _onlineUsers[userKey] = newUser;
                }

                // اطلاع‌رسانی به کلاینت‌های متصل
                await _hubContext.Clients.All.UpdateUserActivity(userId, pageUrl, pageTitle);

                _logger.LogDebug("User activity updated: {UserId} on {PageUrl}", userId, pageUrl);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update user activity for {UserId}", userId);
            }
        }

        public async Task UserConnectedAsync(string connectionId, string userId, string userAgent, string ipAddress)
        {
            try
            {
                var userKey = userId ?? connectionId;

                var onlineUser = new OnlineUserDto
                {
                    ConnectionId = connectionId,
                    UserId = userKey,
                    UserAgent = userAgent,
                    IpAddress = ipAddress,
                    ConnectedAt = DateTime.UtcNow,
                    LastActivity = DateTime.UtcNow,
                    CurrentPage = "/",
                    PageTitle = "Home"
                };

                _onlineUsers[userKey] = onlineUser;

                // اطلاع‌رسانی به کلاینت‌ها
                await _hubContext.Clients.All.UserConnected(userId, $"User {userId} connected");

                _logger.LogInformation("User connected: {UserId}, Connection: {ConnectionId}", userId, connectionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UserConnectedAsync for {UserId}", userId);
            }
        }

        public async Task UserDisconnectedAsync(string connectionId)
        {
            try
            {
                var user = _onlineUsers.Values.FirstOrDefault(u => u.ConnectionId == connectionId);
                if (user != null)
                {
                    _onlineUsers.TryRemove(user.UserId, out _);
                    await _hubContext.Clients.All.UserDisconnected(user.UserId);
                    _logger.LogInformation("User disconnected: {UserId}", user.UserId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UserDisconnectedAsync for connection {ConnectionId}", connectionId);
            }
        }

        public async Task<OnlineUserStatsDto> GetOnlineUsersStatsAsync()
        {
            try
            {
                var users = await GetOnlineUsersAsync();
                var activeUsers = users.Where(u => u.IsActive).ToList();

                // گروه‌بندی بر اساس صفحات
                var pageStats = activeUsers
                    .Where(u => !string.IsNullOrEmpty(u.CurrentPage))
                    .GroupBy(u => u.CurrentPage)
                    .Select(g => new PageStatDto
                    {
                        PageUrl = g.Key,
                        Visitors = g.Count(),
                        PageTitle = g.First().PageTitle ?? "Unknown"
                    })
                    .OrderByDescending(p => p.Visitors)
                    .Take(10)
                    .ToList();

                // تحلیل جغرافیایی
                var locationTasks = activeUsers
                    .Where(u => !string.IsNullOrEmpty(u.IpAddress))
                    .Select(async u => await _geoLocationService.GetLocationFromIpAsync(u.IpAddress))
                    .ToList();

                var locations = await Task.WhenAll(locationTasks);
                var countryStats = locations
                    .Where(l => l != null && !string.IsNullOrEmpty(l.Country))
                    .GroupBy(l => l.Country)
                    .Select(g => new LocationStatDto
                    {
                        Country = g.Key,
                        Visitors = g.Count(),
                        CountryCode = g.First().CountryCode
                    })
                    .OrderByDescending(c => c.Visitors)
                    .ToList();

                return new OnlineUserStatsDto
                {
                    TotalOnline = users.Count,
                    TotalActive = activeUsers.Count,
                    Pages = pageStats,
                    Countries = countryStats,
                    RecordedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting online users stats");
                return new OnlineUserStatsDto
                {
                    TotalOnline = 0,
                    TotalActive = 0,
                    Pages = new List<PageStatDto>(),
                    Countries = new List<LocationStatDto>(),
                    RecordedAt = DateTime.UtcNow
                };
            }
        }
    }

  

  
}