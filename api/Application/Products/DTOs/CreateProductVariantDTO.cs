namespace Application.Products.DTOs
{
    public class CreateProductVariantDTO
    {
        public string SKU { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public List<VariantAttributeValueDTO> AttributeValues { get; set; }
    }
}