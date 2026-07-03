using Common.Roles;
using Data.Contracts;
using Entities.Common.Events;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services.Services.NotificationServices;

namespace Application.Points.Events
{
    public class PointsConsumedEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public long UserId { get; }
        public int Points { get; }
        public string Reason { get; }
        public long? ReferenceId { get; }

        public PointsConsumedEvent(long userId, int points, string reason, long? referenceId)
        {
            UserId = userId;
            Points = points;
            Reason = reason;
            ReferenceId = referenceId;
        }
    }

    /// <summary>
    /// اکشن ارسال نوتیفیکیشن برای کاهش امتیاز کاربر
    /// </summary>
    /// <param name="sendSystemNotification">سرویس ارسال نوتیفیکیشن</param>
    /// <param name="userRepository">ریپازیتوری کاربران</param>
    public class PointsConsumedNotificationAction(
        ISendSystemNotification sendSystemNotification,
        IRepository<User> userRepository) : IDomainEventAction<PointsConsumedEvent>
    {
        private readonly ISendSystemNotification _sendSystemNotification = sendSystemNotification;
        private readonly IRepository<User> _userRepository = userRepository;

        public async Task ExecuteAsync(PointsConsumedEvent @event, CancellationToken cancellationToken)
        {
            // ارسال نوتیفیکیشن به کاربر
            await _sendSystemNotification.SendNotification(
                await GetAdminUserId(cancellationToken),
                subject: "امتیاز شما کسر شد",
                description: $"تعداد {@event.Points} امتیاز بابت {@event.Reason} از امتیازات شما کسر شد. مانده امتیازات شما: {await GetUserTotalPoints(@event.UserId, cancellationToken)}",
                false,
                new List<long> { @event.UserId },
                true
            );

            // ارسال نوتیفیکیشن به ادمین برای تراکنش‌های بزرگ
            if (@event.Points >= 5000) // فقط برای مصرف امتیازات بالا به ادمین اطلاع دهیم
            {
                var adminId = await GetAdminUserId(cancellationToken);
                if (adminId > 0)
                {
                    var user = await _userRepository.GetByIdAsync(cancellationToken, @event.UserId);
                    await _sendSystemNotification.SendNotification(
                        adminId,
                        subject: "کسر امتیاز توسط کاربر",
                        description: $"کاربر {user?.FullName} ({@event.UserId}) - {@event.Points} امتیاز بابت {@event.Reason} مصرف کرد.",
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
