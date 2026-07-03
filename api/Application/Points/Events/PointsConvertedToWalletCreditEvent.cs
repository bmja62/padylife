using Common.Roles;
using Data.Contracts;
using Entities.Common.Events;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services.Services.NotificationServices;
using System.Text;

namespace Application.Points.Events
{
    public class PointsConvertedToWalletCreditEvent : IDomainEvent
    {
        public PointsConvertedToWalletCreditEvent(long userId, int pointsToConvert, decimal moneyValue, string description)
        {
            UserId = userId;
            PointsToConvert = pointsToConvert;
            MoneyValue = moneyValue;
            Description = description;
        }

        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public long UserId { get; }
        public int PointsToConvert { get; }
        public decimal MoneyValue { get; }
        public string Description { get; }
    }

    /// <summary>
    /// اکشن ارسال نوتیفیکیشن برای افزایش امتیاز کاربر
    /// </summary>
    /// <param name="sendSystemNotification">سرویس ارسال نوتیفیکیشن</param>
    /// <param name="userRepository">ریپازیتوری کاربران</param>
    public class PointsConvertedToWalletCreditNotificationAction(
        ISendSystemNotification sendSystemNotification,
        IRepository<User> userRepository) : IDomainEventAction<PointsConvertedToWalletCreditEvent>
    {
        private readonly ISendSystemNotification _sendSystemNotification = sendSystemNotification;
        private readonly IRepository<User> _userRepository = userRepository;

        public async Task ExecuteAsync(PointsConvertedToWalletCreditEvent @event, CancellationToken cancellationToken)
        {
            StringBuilder description = new StringBuilder();

            description.Append($"کاربر عزیز به شناسه #{@event.UserId}");
            description.AppendLine();
            description.Append($"درخواست شما برای تبدیل تعداد {@event.PointsToConvert} امتیاز به {@event.MoneyValue} اعتبار کیف پول انجام گردید");
            description.AppendLine();
            description.Append($"توضیحات تکمیلی:");
            description.AppendLine();
            description.Append(@event.Description);

            // ارسال نوتیفیکیشن به کاربر
            await _sendSystemNotification.SendNotification(
                await GetAdminUserId(cancellationToken),
                subject: "تبدیل امتیاز به اعتبار کیف پول",
                description: description.ToString(),
                false,
                new List<long> { @event.UserId },
                true
            );


            var adminId = await GetAdminUserId(cancellationToken);
            if (adminId > 0)
            {
                var user = await _userRepository.GetByIdAsync(cancellationToken, @event.UserId);
                await _sendSystemNotification.SendNotification(
                    adminId,
                    subject: "تبدیل امتیاز به اعتبار کیف پول",
                    description: description.ToString(),
                    false,
                    new List<long> { adminId, @event.UserId },
                    true
                );
            }

        }
        private async Task<long> GetAdminUserId(CancellationToken cancellationToken)
        {
            return await _userRepository.Table
                .Where(u => u.UserRoles.Any(r => r.Role.Name == UserRoleNames.Admin))
                .Select(u => u.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }

}
