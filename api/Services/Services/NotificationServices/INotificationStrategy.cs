namespace Services.Services.NotificationServices
{
    public interface INotificationStrategy
    {
        Task<ServiceResult<object>> SendNotification(long senderId, string subject, string description, bool allUsers, List<long> users, bool? isFromSystem = false);
    }

}
