using System.ComponentModel.DataAnnotations;

namespace Entities.Notifications
{
    public enum NotificationType
    {
        [Display(Name = "پیام درون سیستمی")]
        SystemNotification = 0,
        [Display(Name = "پیامک")]
        SMS = 1,
        [Display(Name = "ایمیل")]
        Email = 2,
    }
}
