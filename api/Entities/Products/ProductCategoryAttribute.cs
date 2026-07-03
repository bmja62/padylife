using Entities.Common;

namespace Entities.Products
{
    public class ProductCategoryAttribute : IEntity
    {
        public long CategoryId { get; set; }
        public long AttributeId { get; set; }
        public bool IsRequired { get; set; }
        public bool IsVariant { get; set; } // مشخص می‌کند آیا این ویژگی برای ایجاد variants استفاده می‌شود

        // Navigation properties
        public ProductCategory Category { get; set; }
        public ProductAttribute Attribute { get; set; }
    }
}
