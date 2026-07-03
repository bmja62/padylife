using Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace Entities.Orders
{
    public class OrderItem : BaseEntity<long>
    {
        public long OrderId { get; set; }
        public long ObjectId { get; set; } // ID محصول یا واریانت
        public OrderItemType ItemType { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public Order Order { get; set; }
    }

    public enum OrderItemType
    {
        [Display(Name = "محصول ساده")]
        Product = 1,
        [Display(Name = "محصول متغییر")]
        Variant = 2,
        [Display(Name = "پلن")]
        Plan = 3,
        [Display(Name = "خدمات")]
        Service = 4,
        [Display(Name = "متخصص همراه")]
        ExpertPlanPrice
    }
}
