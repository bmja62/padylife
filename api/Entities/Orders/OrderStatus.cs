using System.ComponentModel.DataAnnotations;

namespace Entities.Orders
{
    public enum OrderStatus
    {
        [Display(Name = "در انتظار")]
        Pending,
        [Display(Name = "در حال پردازش")]
        Processing,
        [Display(Name = "بسته بندی و ارسال")]
        Shipped,
        [Display(Name = "ارسال شده")]
        Delivered,
        [Display(Name = "کنسل شده")]
        Cancelled
    }

}
