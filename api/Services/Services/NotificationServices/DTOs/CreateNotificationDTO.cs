using Entities.Notifications;

namespace Services.Services.NotificationServices.DTOs
{
    public class CreateNotificationDTO
    {
        public long SenderId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public bool Allusers { get; set; }
        public List<long> ReciverIds { get; set; } = null;
        public NotificationType NotificationType { get; set; }
        public bool IsFromSystem { get; set; } = false;
    }


}
