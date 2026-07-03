using Entities.Common;
using Entities.Users;

namespace Entities.Notifications
{
    public class Notification : BaseEntity<long>
    {
        private Notification()
        {

        }

        public Notification(long senderId, string subject, string description, NotificationType notificationType, bool? isFromSystem)
        {
            SenderId = senderId;
            Subject = subject;
            Description = description;
            NotificationTypes = notificationType;
            IsFromSystem = isFromSystem;
        }
        public string Subject { get; set; }
        public string Description { get; set; }
        public long SenderId { get; set; }
        public bool? IsFromSystem { get; set; } = false;
        public User Sender { get; set; }
        public NotificationType NotificationTypes { get; set; }
        public List<NotificationReceiver> NotificationReceivers { get; set; } = new();


        public static Notification CreateDefault(long senderId, string subject, string description, NotificationType notificationType, bool? isFromSystem)
            => new(senderId, subject, description, notificationType, isFromSystem);
        public void AddNotificationReceiver(List<NotificationReceiver> notificationReceivers) => NotificationReceivers = notificationReceivers;
    }
}
