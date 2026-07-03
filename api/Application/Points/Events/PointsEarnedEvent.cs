using Common.Roles;
using Data.Contracts;
using Entities.Common.Events;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services.Services.NotificationServices;

namespace Application.Points.Events
{
    public class PointsEarnedEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public long UserId { get; }
        public int Points { get; }
        public string Reason { get; }
        public long? ReferenceId { get; }

        public PointsEarnedEvent(long userId, int points, string reason, long? referenceId)
        {
            UserId = userId;
            Points = points;
            Reason = reason;
            ReferenceId = referenceId;
        }
    }

    /// <summary>
    /// اکشن ارسال نوتیفیکیشن برای افزایش امتیاز کاربر
    /// </summary>
    /// <param name="sendSystemNotification">سرویس ارسال نوتیفیکیشن</param>
    /// <param name="userRepository">ریپازیتوری کاربران</param>
    public class PointsEarnedNotificationAction(
        ISendSystemNotification sendSystemNotification,
        IRepository<User> userRepository) : IDomainEventAction<PointsEarnedEvent>
    {
        private readonly ISendSystemNotification _sendSystemNotification = sendSystemNotification;
        private readonly IRepository<User> _userRepository = userRepository;

        public async Task ExecuteAsync(PointsEarnedEvent @event, CancellationToken cancellationToken)
        {
            // ارسال نوتیفیکیشن به کاربر
            await _sendSystemNotification.SendNotification(
                await GetAdminUserId(cancellationToken),
                subject: "امتیاز جدید دریافت کردید",
                description: $"شما {@event.Points} امتیاز بابت {@event.Reason} دریافت کردید. مجموع امتیازات شما: {await GetUserTotalPoints(@event.UserId, cancellationToken)}",
                false,
                new List<long> { @event.UserId },
                true
            );

            // ارسال نوتیفیکیشن به ادمین (در صورت نیاز)
            if (@event.Points >= 1000) // فقط برای امتیازات بالا به ادمین اطلاع دهیم
            {
                var adminId = await GetAdminUserId(cancellationToken);
                if (adminId > 0)
                {
                    var user = await _userRepository.GetByIdAsync(cancellationToken, @event.UserId);
                    await _sendSystemNotification.SendNotification(
                        adminId,
                        subject: "دریافت امتیاز توسط کاربر",
                        description: $"کاربر {user?.FullName} ({@event.UserId}) - {@event.Points} امتیاز بابت {@event.Reason} دریافت کرد.",
                        false,
                        new List<long> { adminId, @event.UserId },
                        true
                    );
                }
            }
        }

        private async Task<long> GetAdminUserId(CancellationToken cancellationToken)
        {
            return await _userRepository.Table
                .Where(u => u.UserRoles.Any(r => r.Role.Name == UserRoleNames.Admin))
                .Select(u => u.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        private async Task<int> GetUserTotalPoints(long userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Table
                .Include(u => u.UserPoints)
                .ThenInclude(up => up.PointTransactions)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            return user?.UserPoints?.AvailablePoints ?? 0;
        }
    }
}

