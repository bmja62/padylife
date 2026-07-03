using Data.Contracts;
using Entities.Notifications;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services.Services.EmailSenderServices;

namespace Services.Services.NotificationServices
{
    public class SendEmailNotification : ISendEmailNotification
    {
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<User> _userRepository;
        public SendEmailNotification(IEmailSender emailSender, IRepository<User> userRepository, IRepository<Notification> notificationRepository)
        {
            _emailSender = emailSender;
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<ServiceResult<object>> SendNotification(long senderId, string subject, string description, bool allUsers, List<long> users, bool? isFromSystem)
        {
            if (subject is null && subject?.Length >= 200)
                return ServiceResult.Fail<object>("موضوع یک فیلد ضروری است و مقدار آن نباید بیشتر از 200 کارکتر باشد.");

            if (description is null && description?.Length >= 2000)
                return ServiceResult.Fail<object>("توضیحات یک فیلد ضروری است و مقدار آن نباید بیشتر از 2000 کارکتر باشد.");

            var query = _userRepository.Table;
            if (users != null && !allUsers)
                query = query.Where(t => users.Contains(t.Id));

            var validUsers = await query.Select(t => new { t.Email, t.Id }).ToListAsync();
            if (validUsers is null)
                return ServiceResult.Fail<object>("لیست ارسالی خالی است.");

            var notification = NotificationHelper.CreateNotification(senderId, subject, description, NotificationType.Email, validUsers.Select(t => t.Id).ToList(), isFromSystem);
            var errors = new List<object>();
            validUsers.ForEach(async t =>
            {
                if (t.Email is not null)
                {
                    var result = await _emailSender.Send(t.Email, subject, description);
                    if (result.IsSuccess == false)
                        errors.Add($"ایمیل برای {t.Email} ارسال نشد");
                }
            });
            await _notificationRepository.AddAsync(notification, CancellationToken.None);
            return ServiceResult.Ok<object>(errors);
        }
    }

}
