namespace Services.Services.NotificationServices
{
    public interface ISendSystemNotification : INotificationStrategy
    {
        Task<ServiceResult<object>> GetUserSystemNotifications(long? userId, int? pageNumber, int? count, bool? isFromSystem);
        Task<ServiceResult<object>> GetUserSystemNotificationsForUI(long userId, int? pageNumber, int? count);
        Task<ServiceResult<object>> MarkAsRead(long userId, long notificationId);
    }

}
