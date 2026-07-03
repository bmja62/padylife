using Application.Baskets.Extentions;
using Common.Roles;
using Data.Contracts;
using Entities.Baskets;
using Entities.Common.Events;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services.Services.NotificationServices;

namespace Application.Baskets.Events
{
    public class ExpireBasketsEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public class ExpireBasketsEventHandler(IRepository<User> userRepository, IRepository<Basket> repository, ISendSystemNotification sendSystemNotification) : IDomainEventHandler<ExpireBasketsEvent>
    {
        public async Task HandleAsync(ExpireBasketsEvent @event, CancellationToken cancellationToken)
        {
            var expiredBaskets = await repository.Table.WhereExpired().ToListAsync();
            if (expiredBaskets != null && expiredBaskets.Any() && expiredBaskets.Count > 0)
            {
                expiredBaskets.ForEach(item =>
                {
                    item.ExpireBasket();
                });
                await repository.UpdateRangeAsync(expiredBaskets, CancellationToken.None);

                var notificationUsers = expiredBaskets.Select(t => t.UserId).ToList();
                if (notificationUsers != null && notificationUsers.Count > 0)
                {
                    var adminId = await userRepository.Table.Include(t => t.UserRoles).ThenInclude(t => t.Role).Where(t => t.UserRoles.Any(t => t.Role.Name == $"{UserRoleNames.Admin}")).Select(t => t.Id).FirstOrDefaultAsync(); ;
                    if (adminId > 0)
                    {

                        await sendSystemNotification.SendNotification(
                            adminId,
                            BasketNotificationMessage.ExpireSubject,
                            BasketNotificationMessage.ExpireDescription,
                            false,
                            notificationUsers,
                            true);
                    }
                }

            }
        }
    }
}
