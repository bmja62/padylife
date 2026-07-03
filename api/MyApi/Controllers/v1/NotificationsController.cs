using Asp.Versioning;
using Common.GridResults;
using Entities.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Services.NotificationServices;
using Services.Services.NotificationServices.DTOs;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// نوتیفیکیشن
    /// </summary>
    [ApiVersion("1")]
    public class NotificationsController : BaseController
    {
        private readonly ISendSMSNotification sendSMSNotification;
        private readonly ISendEmailNotification sendEmailNotification;
        private readonly ISendSystemNotification sendSystemNotification;

        public NotificationsController
            (
            ISendSMSNotification sendSMSNotification,
            ISendEmailNotification sendEmailNotification,
            ISendSystemNotification sendSystemNotification
            )
        {
            this.sendSMSNotification = sendSMSNotification;
            this.sendEmailNotification = sendEmailNotification;
            this.sendSystemNotification = sendSystemNotification;
        }

        /// <summary>
        /// ارسال پیام
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult<object>> SendNotification([FromBody] CreateNotificationDTO dto)
        {
            switch (dto.NotificationType)
            {
                case NotificationType.SystemNotification:
                    {
                        // Using SendSystemNotification Notification
                        return await sendSystemNotification.SendNotification(dto.SenderId, dto.Subject, dto.Description, dto.Allusers, dto.ReciverIds, dto.IsFromSystem);
                    }
                case NotificationType.Email:
                    {
                        // Using SendEmailNotification Notification
                        return await sendEmailNotification.SendNotification(dto.SenderId, dto.Subject, dto.Description, dto.Allusers, dto.ReciverIds, dto.IsFromSystem);
                    }

                case NotificationType.SMS:
                    {
                        // Using SendSMSNotification Notification
                        return await sendSMSNotification.SendNotification(dto.SenderId, dto.Subject, dto.Description, dto.Allusers, dto.ReciverIds, dto.IsFromSystem);
                    }
            }
            return ServiceResult.Fail<object>("مشکلی پیش آمده");
        }

        /// <summary>
        /// دریافت پیام های سیستمی
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="globalGrid"></param>
        /// <param name="isFromSystem"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ServiceResult<object>> GetSystemNotificationByUserId([FromQuery] long? userId, [FromQuery] GlobalGrid globalGrid, bool? isFromSystem) =>
             await sendSystemNotification.GetUserSystemNotifications(userId, globalGrid.PageNumber, globalGrid.Count, isFromSystem);

        /// <summary>
        /// دریافت پیام های سیستمی برای UI
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ServiceResult<object>> GetUserSystemNotificationsForUI([FromQuery] long userId, int? pageNumber = 1, int? count = 10) =>
           await sendSystemNotification.GetUserSystemNotificationsForUI(userId, pageNumber, count);


        /// <summary>
        /// تایید به عنوان خوانده شده
        /// </summary>
        /// <param name="markAsRead"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ServiceResult<object>> MarkAsRead([FromBody] MarkAsReadDTO markAsRead) =>
            await sendSystemNotification.MarkAsRead(markAsRead.UserId, markAsRead.NotificationId);
    }
}
