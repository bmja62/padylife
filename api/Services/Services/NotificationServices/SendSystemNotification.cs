using Data.Contracts;
using Entities.Hubs;
using Entities.Notifications;
using Entities.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Services.Hubs;
using Services.Services.NotifyServices;

namespace Services.Services.NotificationServices
{
    public class SendSystemNotification : ISendSystemNotification
    {
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IRepository<NotificationReceiver> _notificationReceiverRepository;
        private readonly IRepository<NotifyConnection> _notifyConnectionRepository;
        private readonly IRepository<User> _userRepository;
        private readonly INotifyService _notifyService;
        private readonly IHubContext<NotifyHub> _hubContext;

        public SendSystemNotification
            (
            IRepository<Notification> notificationRepository,
            IRepository<User> userRepository,
            IRepository<NotificationReceiver> notificationReceiverRepository,
            IHubContext<NotifyHub> hubContext,
            IRepository<NotifyConnection> notifyConnectionRepository,
            INotifyService notifyService)
        {
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
            _notificationReceiverRepository = notificationReceiverRepository;
            _hubContext = hubContext;
            _notifyConnectionRepository = notifyConnectionRepository;
            _notifyService = notifyService;
        }

        public async Task<ServiceResult<object>> GetUserSystemNotifications(long? userId, int? pageNumber, int? count, bool? isFromSystem)
        {
            var subQuery = _notificationRepository.Table.SelectMany(t => t.NotificationReceivers);

            if (userId.HasValue && userId > 0)
                subQuery = subQuery.Where(t => t.ReceiverId == userId);

            var notificationIds = subQuery.Select(t => t.NotificationId);

            var query = _notificationRepository.Table.Where(t => t.NotificationTypes == NotificationType.SystemNotification).Where(t => notificationIds.Contains(t.Id));

            if (isFromSystem.HasValue)
                query = query.Where(t => t.IsFromSystem == isFromSystem);

            var result = await query.Select(t => new
            {
                t.Id,
                t.SenderId,
                t.Subject,
                t.Description,
                t.NotificationTypes,
                t.CreatedAt,
                IsRead = t.NotificationReceivers.Where(t => t.ReceiverId == userId && t.NotificationId == t.Id).Select(x => x.IsRead).FirstOrDefault()
            })
            .OrderBy(x => x.IsRead ? 0 : 999).ThenByDescending(t => t.Id)
            .Skip((pageNumber.Value - 1) * count.Value)
            .Take(count.Value)
            .ToListAsync();
            return ServiceResult.Ok<object>(new
            {
                Data = result,
                TotalCount = await query.CountAsync()
            });
        }

        public async Task<ServiceResult<object>> GetUserSystemNotificationsForUI(long userId, int? pageNumber, int? count)
        {
            int page = pageNumber ?? 1;
            int pageSize = count ?? 10;

            // ابتدا شناسه‌های مورد نیاز را بگیرید
            var receiverQuery = _notificationReceiverRepository.Table
                .Where(a => a.ReceiverId == userId);

            var unReadIds = await receiverQuery
                .Where(a => !a.IsRead)
                .Select(a => a.NotificationId)
                .ToListAsync();

            var readIds = await receiverQuery
                .Where(a => a.IsRead)
                .Select(a => a.NotificationId)
                .ToListAsync();

            // سپس اطلاعیه‌ها را بارگیری کنید
            var unReadNotifications = await _notificationRepository.Table
                .Where(a => unReadIds.Contains(a.Id))
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new
                {
                    t.Id,
                    t.SenderId,
                    t.Subject,
                    t.Description,
                    t.NotificationTypes,
                    t.CreatedAt
                })
                .ToListAsync();

            var readNotifications = await _notificationRepository.Table
                .Where(a => readIds.Contains(a.Id))
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new
                {
                    t.Id,
                    t.SenderId,
                    t.Subject,
                    t.Description,
                    t.NotificationTypes,
                    t.CreatedAt
                })
                .ToListAsync();

            var data = new
            {
                UnReadNotification = unReadNotifications,
                UnReadTotalCount = unReadIds.Count,
                ReadNotification = readNotifications,
                ReadTotalCount = readIds.Count,
            };

            await UpdateUserData(userId);

            return ServiceResult.Ok<object>(new
            {
                Data = data,
                TotalCount = await receiverQuery.CountAsync()
            });
        }

        private async Task UpdateUserData(long userId)
        {
            var connectionsIds = await _notifyConnectionRepository.Table.Where(a => a.UserId == userId).Select(a => a.ConnectionId).ToListAsync();
            if (connectionsIds != null && connectionsIds.Count > 0)
            {
                var userData = await _notifyService.GetUserData(userId);
                await _hubContext.Clients.Clients(connectionsIds).SendAsync("UserData", userData);
            }
        }

        private async Task UpdateUsersData(List<long> userIds)
        {
            var connectionsIds = await _notifyConnectionRepository.Table.Where(a => userIds.Contains(a.UserId)).Select(a => new { a.ConnectionId, a.UserId }).ToListAsync();
            if (connectionsIds != null && connectionsIds.Count > 0)
            {
                foreach (var connection in connectionsIds)
                {
                    var userData = await _notifyService.GetUserData(connection.UserId);
                    await _hubContext.Clients.Clients(connection.ConnectionId).SendAsync("UserData", userData);
                }

            }
        }

        public async Task<ServiceResult<object>> MarkAsRead(long userId, long notificationId)
        {
            var notificationReceiverInDb = await _notificationReceiverRepository.Table.Where(t => t.NotificationId == notificationId && t.ReceiverId == userId && t.IsRead == false).FirstOrDefaultAsync();
            if (notificationReceiverInDb is null)
                return ServiceResult.Ok<object>("نوتیفیکیشنی یافت نشد");

            notificationReceiverInDb.SetIsRead();

            await _notificationReceiverRepository.UpdateAsync(notificationReceiverInDb, CancellationToken.None);

            await UpdateUserData(userId);

            return ServiceResult.Ok<object>(notificationReceiverInDb);
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

            var ValidUserIds = await query.Select(t => t.Id).ToListAsync();
            if (ValidUserIds is null)
                return ServiceResult.Fail<object>("لیست ارسالی خالی است.");

            var notification = NotificationHelper.CreateNotification(senderId, subject, description, NotificationType.SystemNotification, ValidUserIds, isFromSystem);

            if (notification is null)
                return ServiceResult.Fail<object>("مشکلی پیش آمده");

            await _notificationRepository.AddAsync(notification, CancellationToken.None);

            var simpleNotification = new
            {
                notification.Id,
                notification.Subject,
                notification.Description,
                notification.NotificationTypes,
                notification.CreatedAt
            };
            if (allUsers)
            {
                var connectionsIds = await _notifyConnectionRepository.Table.Select(a => a.ConnectionId).ToListAsync();
                await _hubContext.Clients.Clients(connectionsIds).SendAsync("UserNotifications", simpleNotification);
                await UpdateUsersData(ValidUserIds);
            }
            else
            {
                var connectionsIds = await _notifyConnectionRepository.Table.Where(a => users.Contains(a.UserId)).Select(a => a.ConnectionId).ToListAsync();
                if (connectionsIds != null && connectionsIds.Count > 0)
                {
                    await _hubContext.Clients.Clients(connectionsIds).SendAsync("UserNotifications", simpleNotification);
                    await UpdateUsersData(ValidUserIds);
                }
            }

            return ServiceResult.Ok<object>(null);
        }


    }

}
