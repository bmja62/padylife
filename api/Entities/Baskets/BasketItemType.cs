using System.ComponentModel.DataAnnotations;

namespace Entities.Baskets
{
    public enum BasketItemType
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
        ExpertPlanPrice = 5
    }



}
