using Entities.Addresses.ECommerce.Entities;
using Entities.Common;
using Entities.Users;

namespace Entities.Baskets
{
    public class Basket : BaseEntity<long>
    {
        private Basket() { }
        public Basket(long userId)
        {
            UserId = userId;
            Items = new List<BasketItem>();
        }
        //FKs
        public long UserId { get; set; }
        public long? AddressId { get; set; }


        public decimal ProductTotalPrice => Items.Sum(i => i.UnitPrice * i.Quantity);
        public decimal DiscountAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal FinalPrice => (ProductTotalPrice - DiscountAmount) + ShippingCost;

        public DeliveryType? SelectedDeliveryType { get; set; }

        public BasketStatus Status { get; set; } = BasketStatus.Active;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;



        /// <summary>
        /// مدت زمان یک روز در نظر گرفته شده در صورت تغییر متد WhereExpired هم تغییر داده شود
        /// </summary>
        private static readonly TimeSpan ExpirationTime = TimeSpan.FromHours(24);

        //Navigations 
        public User User { get; set; }
        public Address Address { get; set; }
        public ICollection<BasketItem> Items { get; set; } = new List<BasketItem>();
        public ICollection<BasketHistory> HistoryLogs { get; set; } = new List<BasketHistory>();

        // ثبت لاگ تغییرات
        public void AddHistory(string title, string description)
        {
            HistoryLogs.Add(new BasketHistory
            {
                Title = title,
                Description = description,
                CreatedAt = DateTime.UtcNow
            });
        }

        // متد برای افزودن آیتم به سبد
        public void AddItem(BasketItem item)
        {
            LastUpdated = DateTime.UtcNow;
            Items.Add(item);
            AddHistory("آیتم جدید اضافه شد", $"آیتم {item.ObjectId} به سبد خرید اضافه شد.");
        }

        // متد برای تغییر تعداد آیتم موجود
        public void UpdateItemQuantity(long itemId, int newQuantity)
        {
            var item = Items.FirstOrDefault(i => i.ObjectId == itemId);
            if (item != null)
            {
                item.Quantity = newQuantity;
                AddHistory("تغییر تعداد آیتم", $"تعداد آیتم {itemId} به {newQuantity} تغییر کرد.");
            }
        }

        // متد برای حذف آیتم از سبد
        public void RemoveItem(long itemId)
        {
            var item = Items.FirstOrDefault(i => i.ObjectId == itemId);
            if (item != null)
            {
                Items.Remove(item);
                AddHistory("آیتم حذف شد", $"آیتم {itemId} از سبد خرید حذف شد.");
            }
        }

        // متد منقضی کردن سبد
        public void ExpireBasket()
        {
            Status = BasketStatus.Expired;
            LastUpdated = DateTime.UtcNow;
            AddHistory("سبد منقضی شد", $"بعلت عدم تعیین و تکلیف ، سبد شما منقضی شد.");
        }

        // متد بررسی منقضی شدن سبد
        public void CheckExpiration()
        {
            if (Status == BasketStatus.Active && DateTime.UtcNow - CreatedAt > ExpirationTime)
            {
                Status = BasketStatus.Expired;
                AddHistory("سبد منقضی شد", "سبد خرید به دلیل عدم تبدیل به سفارش منقضی شد.");
            }
        }

        public bool IsExpired()
        {
            if (Status == BasketStatus.Active && DateTime.UtcNow - CreatedAt > ExpirationTime)
            {
                return true;
            }
            return false;
        }

        public static Basket CreateByUserId(long userId) => new(userId);
    }



}
