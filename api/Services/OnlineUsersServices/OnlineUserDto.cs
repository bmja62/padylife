
namespace Services.Services.OnlineUsersServices
{
    public class OnlineUserDto
    {
        public string UserId { get; internal set; }
        public string CurrentPage { get; internal set; }
        public string PageTitle { get; internal set; }
        public string UserAgent { get; internal set; }
        public string IpAddress { get; internal set; }
        public DateTime ConnectedAt { get; internal set; }
        public DateTime LastActivity { get; internal set; }
        public string ConnectionId { get; internal set; }
        public bool IsActive { get; internal set; }
    }


}