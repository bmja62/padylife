using Entities.Common;
using Entities.Users;

namespace Entities.Notifications
{
    public class NotificationReceiver : BaseEntity<long>
    {
        public Notification Notification { get; set; }
        public long NotificationId { get; set; }
        public long ReceiverId { get; set; }
        public User Receiver { get; set; }
        public bool IsRead { get; set; } = false;

        public void SetIsRead()
        {
            IsRead = true;
        }
    }
}
