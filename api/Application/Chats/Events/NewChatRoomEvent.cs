using Application.Chats.Extentions;
using Common.Roles;
using Data.Contracts;
using Entities.Common.Events;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services.Services.NotificationServices;

namespace Application.Chats.Events
{
    public class NewChatRoomEvent(long[] userIds) : IDomainEvent
    {
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
        public long[] UserIds { get; } = userIds;
    }

    /// <summary>
    /// اکشن ارسال نوتیفیکیشن داخلی برای رویداد سفارش جدید
    /// </summary>
    /// <param name="sendSystemNotification"></param>
    /// <param name="userRepository"></param>
    public class SendNewChatRoomNotificationAction(ISendSystemNotification sendSystemNotification, IRepository<User> userRepository) : IDomainEventAction<NewChatRoomEvent>
    {
        private readonly ISendSystemNotification _sendSystemNotification = sendSystemNotification;
        private readonly IRepository<User> _userRepository = userRepository;

        public async Task ExecuteAsync(NewChatRoomEvent @event, CancellationToken cancellationToken)
        {
            var adminId = await _userRepository.Table
                .Where(u => u.UserRoles.Any(r => r.Role.Name == UserRoleNames.Admin))
                .Select(u => u.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (adminId > 0)
            {
                await _sendSystemNotification.SendNotification(
                    adminId,
                    ChatNotificationMessages.NewChatSubject,
                    ChatNotificationMessages.NewChatDescription,
                    false,
                    @event.UserIds.ToList(),
                    true
                );
            }
        }
    }
}
