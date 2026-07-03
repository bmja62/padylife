using Entities.Common;

namespace Entities.Products
{
    public class VariantAttributeValue : IEntity
    {
        public long VariantId { get; set; }
        public long AttributeId { get; set; }
        public string Value { get; set; }

        // Navigation properties
        public ProductVariant Variant { get; set; }
        public ProductAttribute Attribute { get; set; }
    }
}
