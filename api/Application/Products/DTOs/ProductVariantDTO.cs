namespace Application.Products.DTOs
{
    public class ProductVariantDTO : UpdateProductVariantDTO
    {
        public GetProductImageDTO ProductVariantImages { get; internal set; }
    }

    public class GetProductVariantDTO
    {
        public long Id { get; internal set; }
        public string SKU { get; internal set; }
        public decimal Price { get; internal set; }
        public int StockQuantity { get; internal set; }
        public GetProductImageDTO ProductVariantImages { get; internal set; }
        public List<GetVariantAttributeValueDTO> AttributeValues { get; internal set; }
        public Dictionary<string, string> Attributes { get; set; }
        public int BasketQuantity { get; internal set; }
    }

}