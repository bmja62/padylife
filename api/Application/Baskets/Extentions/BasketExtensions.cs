using Entities.Baskets;
using Entities.Discounts;
using Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Baskets.Extentions
{
    public static class BasketExtensions
    {
        public static IQueryable<Basket> WhereExpired(this IQueryable<Basket> query)
        {
            var expirationDate = DateTime.UtcNow.AddDays(-1);

            return query
                .Where(b => b.Status == BasketStatus.Active
                         && b.CreatedAt < expirationDate);
        }

        public static ServiceResult<Order> ConvertToOrder(this Basket basket, Discount discount = null)
        {
            // اعتبارسنجی سبد
            if (basket == null)
                return ServiceResult.BadRequest<Order>("سبد خرید نمی‌تواند خالی باشد");

            if (basket.IsExpired())
                return ServiceResult.BadRequest<Order>("سبد خرید منقضی شده و نمی‌تواند به سفارش تبدیل شود");

            if (basket.Status != BasketStatus.Active)
                return ServiceResult.BadRequest<Order>($"سبد خرید در وضعیت {basket.Status} است و نمی‌تواند به سفارش تبدیل شود");

            if (!basket.Items.Any())
                return ServiceResult.BadRequest<Order>("سبد خرید خالی است و نمی‌تواند به سفارش تبدیل شود");

            // اعتبارسنجی تخفیف
            if (discount != null)
            {
                var validationResult = ValidateDiscount(discount, basket.UserId, basket.ProductTotalPrice);
                if (!validationResult.IsValid)
                    return ServiceResult.BadRequest<Order>(validationResult.ErrorMessage);
            }

            // ایجاد سفارش جدید
            var order = new Order
            {
                UserId = basket.UserId,
                Discount = discount,
                CreatedAt = DateTime.UtcNow,
                AddressId = basket.AddressId,
            };

            // تبدیل آیتم‌های سبد به آیتم‌های سفارش
            foreach (var basketItem in basket.Items)
            {
                var itemType = basketItem.ItemType switch
                {
                    BasketItemType.Product => OrderItemType.Product,
                    BasketItemType.Variant => OrderItemType.Variant,
                    BasketItemType.Plan => OrderItemType.Plan,
                    BasketItemType.Service => OrderItemType.Service,
                    BasketItemType.ExpertPlanPrice => OrderItemType.ExpertPlanPrice,
                    _ => throw new ArgumentOutOfRangeException(nameof(basketItem.ItemType), $"Unexpected item type: {basketItem.ItemType}")
                };
                order.AddItem(basketItem.ObjectId, itemType, basketItem.Quantity, basketItem.UnitPrice);
            }

            // به‌روزرسانی وضعیت سبد
            basket.Status = BasketStatus.ConvertedToOrder;
            basket.LastUpdated = DateTime.UtcNow;
            basket.AddHistory("سبد به سفارش تبدیل شد", $"سبد خرید با موفقیت به سفارش #{order.Id} تبدیل شد.");

            return ServiceResult.Ok(order);
        }

        private static DiscountValidationResult ValidateDiscount(Discount discount, long userId, decimal totalAmount)
        {
            // بررسی تاریخ‌های تخفیف
            if (discount.StartDate.HasValue && discount.StartDate.Value > DateTime.UtcNow)
                return new DiscountValidationResult(false, "تخفیف هنوز شروع نشده است");

            if (discount.EndDate.HasValue && discount.EndDate.Value < DateTime.UtcNow)
                return new DiscountValidationResult(false, "تخفیف منقضی شده است");

            // بررسی حداقل شرایط استفاده از تخفیف
            if (discount.DiscountPercentage > 0 && discount.DiscountPercentage > 100)
                return new DiscountValidationResult(false, "درصد تخفیف نامعتبر است");

            if (discount.DiscountAmount > 0 && discount.DiscountAmount > totalAmount)
                return new DiscountValidationResult(false, "مبلغ تخفیف بیشتر از مبلغ کل سبد است");

            return new DiscountValidationResult(true);
        }

        private class DiscountValidationResult
        {
            public bool IsValid { get; }
            public string ErrorMessage { get; }

            public DiscountValidationResult(bool isValid, string errorMessage = null)
            {
                IsValid = isValid;
                ErrorMessage = errorMessage;
            }
        }
    }
}
