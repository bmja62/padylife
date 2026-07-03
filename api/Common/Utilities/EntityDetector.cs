using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public static class EntityDetector
    {
        private static readonly Dictionary<string, string> _urlPatterns = new()
        {
            { @"/plans/(\d+)", "Plan" },
            { @"/products/(\d+)", "Product" },
            { @"/users/(\d+)", "User" },
            { @"/categories/(\d+)", "Category" },
            { @"/articles/(\d+)", "Article" },
            { @"/blog/(\d+)", "Blog" },
            { @"/courses/(\d+)", "Course" },
            { @"/orders/(\d+)", "Order" },
            { @"/invoices/(\d+)", "Invoice" },
            { @"/dashboard", "Dashboard" },
            { @"/profile", "Profile" },
            { @"/settings", "Settings" },
            { @"/admin/", "Admin" },
            { @"/api/", "Api" }
        };

        private static readonly Dictionary<string, string> _sectionPatterns = new()
        {
            { @"/admin/", "Admin" },
            { @"/dashboard", "Dashboard" },
            { @"/api/", "Api" },
            { @"/auth/", "Auth" },
            { @"/public/", "Public" },
            { @"/private/", "Private" }
        };

        public static EntityDetectionResult DetectFromUrl(string pageUrl)
        {
            if (string.IsNullOrEmpty(pageUrl))
                return new EntityDetectionResult();

            var result = new EntityDetectionResult();

            // تشخیص موجودیت از الگوهای URL
            foreach (var pattern in _urlPatterns)
            {
                var match = System.Text.RegularExpressions.Regex.Match(pageUrl, pattern.Key);
                if (match.Success)
                {
                    result.EntityType = pattern.Value;

                    // استخراج ID اگر وجود دارد
                    if (match.Groups.Count > 1 && long.TryParse(match.Groups[1].Value, out long entityId))
                    {
                        result.EntityId = entityId;
                    }
                    break;
                }
            }

            // تشخیص بخش
            foreach (var pattern in _sectionPatterns)
            {
                if (pageUrl.Contains(pattern.Key))
                {
                    result.Section = pattern.Value;
                    break;
                }
            }

            // اگر موجودیت تشخیص داده نشد، از ساختار URL استفاده کن
            if (string.IsNullOrEmpty(result.EntityType))
            {
                result.EntityType = ExtractEntityFromUrlStructure(pageUrl);
            }

            return result;
        }

        private static string ExtractEntityFromUrlStructure(string url)
        {
            var segments = url.Trim('/').Split('/');

            if (segments.Length > 0)
            {
                var firstSegment = segments[0].ToLower();

                return firstSegment switch
                {
                    "plans" or "plan" => "Plan",
                    "products" or "product" => "Product",
                    "users" or "user" => "User",
                    "categories" or "category" => "Category",
                    "articles" or "article" => "Article",
                    "blog" => "Blog",
                    "courses" or "course" => "Course",
                    "orders" or "order" => "Order",
                    "invoices" or "invoice" => "Invoice",
                    "dashboard" => "Dashboard",
                    "profile" => "Profile",
                    "settings" => "Settings",
                    "admin" => "Admin",
                    "api" => "Api",
                    _ => "Page" // پیش‌فرض
                };
            }

            return "Home"; // صفحه اصلی
        }
    }

    public class EntityDetectionResult
    {
        public string EntityType { get; set; } = "Page";
        public long? EntityId { get; set; }
        public string Section { get; set; } = "Public";
    }
}
