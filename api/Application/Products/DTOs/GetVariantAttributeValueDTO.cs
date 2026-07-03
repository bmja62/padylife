namespace Application.Products.DTOs
{
    public class GetVariantAttributeValueDTO
    {
        public long VariantId { get; internal set; }
        public long AttributeId { get; set; }
        public string AttributeName { get; set; }
        public string Value { get; set; }
    }
}