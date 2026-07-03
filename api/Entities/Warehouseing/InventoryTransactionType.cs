namespace Entities.Warehouseing
{
    public enum InventoryTransactionType
    {
        Purchase,       // خرید
        Sale,           // فروش
        Return,         // مرجوعی
        Adjustment,     // تنظیم دستی
        TransferIn,     // انتقال ورودی
        TransferOut,    // انتقال خروجی
        Damage,         // ضایعات
        Expired,        // انقضا
        Reservation,    // رزرو
        Release         // آزادسازی رزرو
    }
}
