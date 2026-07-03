using Entities.Common;

namespace Entities.Baskets
{
    public class BasketHistory : BaseEntity<long>
    {
        public long BasketId { get; set; }
        public string Title { get; set; }       // عنوان لاگ (مثلاً "تغییر قیمت"، "محصول حذف شد"، "افزودن تخفیف")
        public string Description { get; set; } // توضیحات جزئی
        public DateTime CreatedAt { get; set; }

        public Basket Basket { get; set; }
    }



}
