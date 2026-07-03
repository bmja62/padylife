using Common.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Services.Services.NotifyServices;
using Services.Services.UserServices;

namespace Services.Hubs
{

    public class NotifyHub : Hub
    {
        readonly IUserSecurity _userSecurity;
        private readonly INotifyService _notifyService;
        private readonly INotifyHubService _notifyHubService;
        private readonly IHttpContextAccessor _contextAccessor;
        public NotifyHub(IUserSecurity userSecurity, INotifyService notifyService, INotifyHubService notifyHubService, IHttpContextAccessor contextAccessor)
        {
            _userSecurity = userSecurity;
            _notifyService = notifyService;
            _notifyHubService = notifyHubService;
            _contextAccessor = contextAccessor;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.Identity.GetUserId<long>();

            if (userId > 0)
                await _notifyHubService.CreateNotifyConnection(userId, Context.ConnectionId);

            var data = await _notifyService.GetUserData(userId);


            await Clients.Client(Context.ConnectionId).SendAsync("UserData", data);
            await base.OnConnectedAsync();
        }

        private long GetLoginUserId()
        {
            if (_userSecurity.UserId != null)
                return _userSecurity.UserId.Value;
            return -1;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _notifyHubService.DeleteNotifyConnectionByConnectionId(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }


}
