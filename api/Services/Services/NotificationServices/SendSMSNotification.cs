using Data.Contracts;
using Entities.Notifications;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services.Services.SmsStrategy.SmsServices;

namespace Services.Services.NotificationServices
{
    public class SendSMSNotification : ISendSMSNotification
    {
        private readonly IRepository<Notification> _notificationRepository;
        private readonly ISmsService _smsService;
        private readonly IRepository<User> _userRepository;
        public SendSMSNotification(ISmsService smsSenderService, IRepository<User> userRepository, IRepository<Notification> notificationRepository)
        {
            _smsService = smsSenderService;
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<ServiceResult<object>> SendNotification(long senderId, string subject, string description, bool allUsers, List<long> users, bool? isFromSystem)
        {
            if (subject is null && subject?.Length > 25)
                return ServiceResult.Fail<object>("موضوع یک فیلد ضروری است و مقدار آن نباید بیشتر از 25 کارکتر باشد.");

            if (description is not null && description?.Length >= 25)
                return ServiceResult.Fail<object>("شما فقط قادر به استفاده از موضوع با حداکثر کارکتر 25 عدد میباشید..");

            var query = _userRepository.Table;
            if (users != null && !allUsers)
                query = query.Where(t => users.Contains(t.Id));

            var validUsers = await query.Select(t => new { t.PhoneNumber, t.Id }).ToListAsync();
            if (validUsers is null)
                return ServiceResult.Fail<object>("لیست ارسالی خالی است.");

            var notification = NotificationHelper.CreateNotification(senderId, subject, description, NotificationType.SMS, validUsers.Select(t => t.Id).ToList(), isFromSystem);
            var errors = new List<string>();
            foreach (var t in validUsers)
            {
                if (t.PhoneNumber?.Length > 0)
                {
                    await _smsService.SendSMSNotification(t.PhoneNumber, notification.Subject, notification.Description);
                }
            }
            await _notificationRepository.AddAsync(notification, CancellationToken.None);
            return ServiceResult.Ok<object>(errors);
        }
    }

}
