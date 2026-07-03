using System.ComponentModel.DataAnnotations;

namespace Entities.Products
{
    public enum ProductType
    {
        [Display(Name = "ساده")]
        Simple = 1,  // محصول ساده (مثل آینه)
        [Display(Name = "متغییر")]
        Variant = 2  // محصول متغیر (مثل کفش سایزبندی)
    }
}
