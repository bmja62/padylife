using Entities.Common;

namespace Entities.Products
{
    public class ProductAttribute : BaseEntity<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public AttributeType Type { get; set; }

        // Navigation properties
        public ICollection<ProductCategoryAttribute> CategoryAttributes { get; set; }
        public ICollection<ProductAttributeValue> AttributeValues { get; set; }
        public ICollection<VariantAttributeValue> VariantAttributeValues { get; set; }
    }
}
