using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Services.Services.OnlineUsersServices;
using System.Collections.Concurrent;

namespace Services.Hubs
{
    public interface IOnlineUsersHub
    {
        Task UserConnected(string userId, string userInfo);
        Task UserDisconnected(string userId);
        Task UpdateOnlineUsers(List<OnlineUserDto> users);
        Task UpdateUserActivity(string userId, string pageUrl, string pageTitle);
    }

    public class OnlineUsersHub : Hub<IOnlineUsersHub>
    {
        private readonly IOnlineUsersService _onlineUsersService;
        private readonly ILogger<OnlineUsersHub> _logger;

        public OnlineUsersHub(
            IOnlineUsersService onlineUsersService,
            ILogger<OnlineUsersHub> logger)
        {
            _onlineUsersService = onlineUsersService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext?.Request.Query["userId"].ToString() ?? "anonymous";
            var userAgent = httpContext?.Request.Headers["User-Agent"].ToString();
            var userIp = httpContext?.Connection.RemoteIpAddress?.ToString();

            await _onlineUsersService.UserConnectedAsync(Context.ConnectionId, userId, userAgent, userIp);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _onlineUsersService.UserDisconnectedAsync(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        // این متد توسط کلاینت فراخوانی می‌شود وقتی کاربر صفحه را تغییر می‌دهد
        public async Task UpdateUserActivity(string pageUrl, string pageTitle)
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext?.Request.Query["userId"].ToString() ?? "anonymous";

            await _onlineUsersService.UpdateUserActivityAsync(userId, pageUrl, pageTitle);
        }

        // متد برای دریافت لیست کاربران آنلاین
        public async Task<List<OnlineUserDto>> GetOnlineUsers()
        {
            return await _onlineUsersService.GetOnlineUsersAsync();
        }

        // متد برای دریافت آمار
        public async Task<OnlineUserStatsDto> GetOnlineStats()
        {
            return await _onlineUsersService.GetOnlineUsersStatsAsync();
        }
    }
}