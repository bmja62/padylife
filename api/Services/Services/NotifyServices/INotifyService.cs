using Common;
using Data.Contracts;
using Entities.Notifications;
using Entities.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Services.Hubs;
using Services.Services.NotifyServices.DTOs;

namespace Services.Services.NotifyServices
{
    public interface INotifyService
    {
        Task<object> GetUserData(long userId);
        Task<object> GetUserNotifications(long userId, int? pageNumber, int? count);

    }

    public class NotifyService : INotifyService, IScopedDependency
    {
        private readonly IHubContext<NotifyHub> _hubContext;
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IRepository<NotificationReceiver> _notificationReceiverRepository;
        private readonly IRepository<User> _userRepository;


        public NotifyService
            (
            IHubContext<NotifyHub> hubContext,
            IRepository<Notification> notificationRepository,
            IRepository<User> userRepository
,
            IRepository<NotificationReceiver> notificationReceiverRepository)
        {
            _hubContext = hubContext;
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _notificationReceiverRepository = notificationReceiverRepository;
        }

        public async Task<object> GetUserData(long userId)
        {
            var notificationQuery = _notificationRepository.Table;
            var subQuery = notificationQuery.SelectMany(t => t.NotificationReceivers).Where(t => t.ReceiverId == userId).Select(t => t.NotificationId);
            // ابتدا شناسه‌های مورد نیاز را بگیرید
            var receiverQuery = _notificationReceiverRepository.Table
                .Where(a => a.ReceiverId == userId);

            var unReadIds = await receiverQuery
                .Where(a => !a.IsRead)
                .Select(a => a.NotificationId)
                .ToListAsync();

            var data = await _userRepository.Table.Where(t => t.Id == userId).Select(t => new GetByTokenDTO
            {
                UserId = t.Id,
                FullName = t.FullName,
                Email = t.Email,
                PhoneNumber = t.PhoneNumber,
                UnReadNotificationCount = unReadIds.Count,
            }).FirstOrDefaultAsync();

            return data;
        }

        public async Task<object> GetUserNotifications(long userId, int? pageNumber, int? count)
        {
            var notificationQuery = _notificationRepository.Table;
            var subQuery = notificationQuery.SelectMany(t => t.NotificationReceivers).Where(t => t.ReceiverId == userId).Select(t => t.NotificationId);
            var query = notificationQuery.Where(t => t.NotificationTypes == NotificationType.SystemNotification).Where(t => subQuery.Contains(t.Id));

            var unReadQuery = notificationQuery.Where(a => a.NotificationReceivers.Any(b => !b.IsRead) && a.NotificationTypes == NotificationType.SystemNotification && subQuery.Contains(a.Id));

            var result = await query.Select(t => new
            {
                UnReadCount = unReadQuery.Count(),
                UnReads = unReadQuery.OrderByDescending(a => a.CreatedAt).Select(t => new
                {
                    t.Id,
                    t.SenderId,
                    t.Subject,
                    t.Description,
                    t.NotificationTypes,
                    t.CreatedAt,
                }).Skip((pageNumber.Value - 1) * count.Value).Take(count.Value).ToList(),
                ReadCount = query.Except(unReadQuery).Count(),
                Reads = query.Except(unReadQuery).OrderByDescending(a => a.CreatedAt).Select(t => new
                {
                    t.Id,
                    t.SenderId,
                    t.Subject,
                    t.Description,
                    t.NotificationTypes,
                    t.CreatedAt,
                }).Skip((pageNumber.Value - 1) * count.Value).Take(count.Value).ToList(),
            })
            .FirstOrDefaultAsync();
            return ServiceResult.Ok<object>(new
            {
                Data = result,
                TotalCount = await query.CountAsync()
            });
        }


    }
}
