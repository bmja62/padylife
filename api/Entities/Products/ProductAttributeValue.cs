using Entities.Common;

namespace Entities.Products
{
    public class ProductAttributeValue : IEntity
    {
        public long ProductId { get; set; }
        public long AttributeId { get; set; }
        public string Value { get; set; }

        // Navigation properties
        public Product Product { get; set; }
        public ProductAttribute Attribute { get; set; }
    }
}
