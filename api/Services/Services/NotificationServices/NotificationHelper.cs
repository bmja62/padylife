using Entities.Notifications;

namespace Services.Services.NotificationServices
{
    public static class NotificationHelper
    {
        public static Notification CreateNotification(long senderId, string subject, string description, NotificationType notificationType, List<long> validUserIds, bool? isFromSystem)
        {
            Notification notification = Notification.CreateDefault(senderId, subject, description, notificationType, isFromSystem);
            var notificationReceivers = validUserIds.Select(t => new NotificationReceiver
            {
                ReceiverId = t,
                IsRead = false,
                IsDeleted = false,
            }).ToList();
            notification.AddNotificationReceiver(notificationReceivers);
            return notification;
        }
    }

}
