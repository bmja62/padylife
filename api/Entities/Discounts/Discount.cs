using Entities.Common;
using Entities.Orders;

namespace Entities.Discounts
{
    public class Discount : BaseEntity<long>
    {
        public string Code { get; set; }   // کد تخفیف
        public decimal DiscountAmount { get; set; }  // تخفیف به صورت عددی
        public decimal DiscountPercentage { get; set; } // تخفیف به صورت درصدی
        public DateTime? StartDate { get; set; } // تاریخ شروع تخفیف
        public DateTime? EndDate { get; set; }   // تاریخ پایان تخفیف
        public bool IsSpecial { get; set; }   // تخفیف ویژه (جشنواره)

        public ICollection<Order> Orders { get; set; }
    }

}
