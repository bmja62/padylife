using Entities.Addresses.ECommerce.Entities;
using Entities.Common;
using Entities.Discounts;
using Entities.Payments;
using Entities.Users;

namespace Entities.Orders;

public class Order : BaseEntity<long>
{

    //FKs
    public long UserId { get; set; }
    public long? DiscountId { get; set; }
    public long? AddressId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
    public decimal TotalAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal FinalAmount => TotalAmount - DiscountAmount;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public Address Address { get; set; }
    public Discount Discount { get; set; }
    public User User { get; set; }

    public ICollection<Payment> Payments { get; set; }
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    // اضافه کردن آیتم به سفارش
    public void AddItem(long objectId, OrderItemType itemType, int quantity, decimal unitPrice)
    {
        var item = new OrderItem
        {
            ObjectId = objectId,
            ItemType = itemType,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
        _items.Add(item);
        RecalculateTotal();
    }

    // حذف آیتم از سفارش
    public void RemoveItem(long orderItemId)
    {
        var item = _items.SingleOrDefault(x => x.Id == orderItemId);
        if (item != null)
        {
            _items.Remove(item);
            RecalculateTotal();
        }
    }

    // تغییر تعداد یک آیتم
    public void UpdateItemQuantity(long orderItemId, int newQuantity)
    {
        var item = _items.SingleOrDefault(x => x.Id == orderItemId);
        if (item != null)
        {
            item.Quantity = newQuantity;
            RecalculateTotal();
        }
    }

    // محاسبه مجدد مبالغ سفارش
    private void RecalculateTotal()
    {
        TotalAmount = _items.Sum(x => x.Quantity * x.UnitPrice);
        ApplyDiscountIfAvailable();
    }

    private void ApplyDiscountIfAvailable()
    {
        DiscountAmount = 0;
        if (Discount != null)
        {
            if (Discount.StartDate.HasValue && Discount.StartDate > DateTime.UtcNow)
                return;

            if (Discount.EndDate.HasValue && Discount.EndDate < DateTime.UtcNow)
                return;

            if (Discount.DiscountPercentage > 0)
                DiscountAmount = (TotalAmount * Discount.DiscountPercentage) / 100;

            if (Discount.DiscountAmount > 0)
                DiscountAmount = Discount.DiscountAmount;

            if (Discount.IsSpecial)
                DiscountAmount *= 2;

            // اطمینان از اینکه تخفیف بیشتر از مبلغ کل نباشد
            DiscountAmount = Math.Min(DiscountAmount, TotalAmount);
        }
    }

    public void SetPayed()
    {

        Status = OrderStatus.Delivered;
        PaymentStatus = PaymentStatus.Paid;
        UpdatedAt = DateTime.Now;
    }

    public void SetPayedByWallet()
    {
        Status = OrderStatus.Delivered;
        PaymentStatus = PaymentStatus.Wallet;
        UpdatedAt = DateTime.Now;
    }

    public void SetPayedFullDiscount()
    {
        Status = OrderStatus.Delivered;
        PaymentStatus = PaymentStatus.FullDiscount;
        UpdatedAt = DateTime.Now;
    }

    public void SetFreePlan()
    {
        Status = OrderStatus.Delivered;
        PaymentStatus = PaymentStatus.FullDiscount;
        UpdatedAt = DateTime.Now;
    }
}
