namespace Application.Products.DTOs
{
    public class ProductAttributeValueDTO
    {
        public long AttributeId { get; set; }
        public string Value { get; set; }
    }

    public class AddProductAttributeValueDTO
    {
        public long ProductId { get; set; }
        public long AttributeId { get; set; }
        public string Value { get; set; }
    }

    public class RemoveProductAttributeValueDTO
    {
        public long ProductId { get; set; }
        public long AttributeId { get; set; }
    }
}