using System.ComponentModel.DataAnnotations;

namespace Entities.Orders
{
    public enum PaymentStatus
    {
        [Display(Name = "پرداخت نشده")]
        Unpaid,
        [Display(Name = "پرداخت شده")]
        Paid,
        [Display(Name = "تراکنش ناموفق")]
        Failed,
        [Display(Name = "باز پرداخت شده")]
        Refunded,
        [Display(Name = "کیف پول")]
        Wallet,
        [Display(Name = "تخفیف کامل")]
        FullDiscount,
        [Display(Name = "پلن رایگان")]
        FreePlan
    }

}
