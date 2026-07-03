using Common.Utilities;
using Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Visits
{
    public class PageVisit : BaseEntity<long>
    {
        public string PageUrl { get; set; }
        public string PageTitle { get; set; }

        // IP اصلی
        public string UserIp { get; set; }

        // IP ناشناس شده برای ذخیره امن
        public string ProcessedUserIp { get; set; }

        // IP هش شده برای ردیابی یکتا
        public string HashedUserIp { get; set; }

        // تشخیص IP داخلی
        public bool IsLocalIp { get; set; }

        public string UserAgent { get; set; }
        public string SessionId { get; set; }

        // وضعیت کاربر
        public long? UserId { get; set; }
        public bool IsLoggedIn { get; set; }

        public DateTime VisitDate { get; set; } = DateTime.UtcNow;
        public bool IsUniqueVisit { get; set; } = true;

        // تشخیص خودکار موجودیت
        public string DetectedEntityType { get; set; }
        public long? DetectedEntityId { get; set; }
        public string DetectedSection { get; set; }

        // اطلاعات اضافی
        public string Referrer { get; set; }
        public string Country { get; set; }
        public string Browser { get; set; }
        public string Platform { get; set; }

        // اطلاعات جغرافیایی (در صورت نیاز)
        public string City { get; set; }
        public string Region { get; set; }

        // اطلاعات دستگاه
        public string DeviceType { get; set; } // Mobile, Desktop, Tablet
        public bool IsBot { get; set; }

        // متدهای هوشمند
        public void AutoDetectEntity()
        {
            var detection = EntityDetector.DetectFromUrl(PageUrl);
            DetectedEntityType = detection.EntityType;
            DetectedEntityId = detection.EntityId;
            DetectedSection = detection.Section;
        }

        // متد برای تشخیص اطلاعات دستگاه (ساده شده)
        // در کلاس PageVisit
        public void DetectDeviceInfo()
        {
            if (string.IsNullOrEmpty(UserAgent))
            {
                Browser = "Unknown";
                Platform = "Unknown";
                DeviceType = "Unknown";
                IsBot = false;
                return;
            }

            var userAgent = UserAgent.ToLower();

            // تشخیص دقیق‌تر مرورگر
            if (userAgent.Contains("chrome") && !userAgent.Contains("edg") && !userAgent.Contains("opr"))
                Browser = "Chrome";
            else if (userAgent.Contains("firefox"))
                Browser = "Firefox";
            else if (userAgent.Contains("safari") && !userAgent.Contains("chrome"))
                Browser = "Safari";
            else if (userAgent.Contains("edg"))
                Browser = "Edge";
            else if (userAgent.Contains("opr") || userAgent.Contains("opera"))
                Browser = "Opera";
            else if (userAgent.Contains("msie") || userAgent.Contains("trident"))
                Browser = "Internet Explorer";
            else
                Browser = "Other";

            // تشخیص دقیق‌تر سیستم عامل
            if (userAgent.Contains("windows nt 10"))
                Platform = "Windows 10/11";
            else if (userAgent.Contains("windows nt 6.3"))
                Platform = "Windows 8.1";
            else if (userAgent.Contains("windows nt 6.2"))
                Platform = "Windows 8";
            else if (userAgent.Contains("windows nt 6.1"))
                Platform = "Windows 7";
            else if (userAgent.Contains("windows nt 6.0"))
                Platform = "Windows Vista";
            else if (userAgent.Contains("windows nt 5.1") || userAgent.Contains("windows xp"))
                Platform = "Windows XP";
            else if (userAgent.Contains("mac os x"))
                Platform = "macOS";
            else if (userAgent.Contains("linux"))
                Platform = "Linux";
            else if (userAgent.Contains("android"))
                Platform = "Android";
            else if (userAgent.Contains("iphone") || userAgent.Contains("ipad"))
                Platform = "iOS";
            else
                Platform = "Other";

            // تشخیص دقیق‌تر نوع دستگاه
            if (userAgent.Contains("mobile") || userAgent.Contains("android") || userAgent.Contains("iphone"))
                DeviceType = "Mobile";
            else if (userAgent.Contains("tablet") || userAgent.Contains("ipad") || (userAgent.Contains("android") && !userAgent.Contains("mobile")))
                DeviceType = "Tablet";
            else
                DeviceType = "Desktop";

            // تشخیص ربات‌ها
            IsBot = userAgent.Contains("bot") ||
                   userAgent.Contains("crawler") ||
                   userAgent.Contains("spider") ||
                   userAgent.Contains("monitoring") ||
                   userAgent.Contains("analytics") ||
                   userAgent.Contains("slurp") ||
                   userAgent.Contains("bingbot") ||
                   userAgent.Contains("yandex") ||
                   userAgent.Contains("duckduckbot");
        }
    }
}