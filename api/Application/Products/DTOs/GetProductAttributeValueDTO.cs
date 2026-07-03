namespace Application.Products.DTOs
{
    public class GetProductAttributeValueDTO
    {
        public long AttributeId { get; set; }
        public string AttributeName { get; set; }
        public string Value { get; set; }
    }
}