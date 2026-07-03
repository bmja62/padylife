namespace Services.Services.NotificationServices
{
    public class NotificationSender
    {
        private readonly INotificationStrategy _notificationStrategy;

        public NotificationSender(INotificationStrategy shippingStrategy)
        {
            _notificationStrategy = shippingStrategy;
        }

        public async Task<ServiceResult<object>> SendNotification(long senderId, string subject, string description, bool allUser, List<long> users)
        {
            return await _notificationStrategy.SendNotification(senderId, subject, description, allUser, users);
        }

    }


}
