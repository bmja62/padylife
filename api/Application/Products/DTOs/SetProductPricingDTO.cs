namespace Application.Products.DTOs
{
    public class SetProductPricingDTO
    {
        public decimal BasePrice { get; set; }
        public List<ProductVariantPriceDTO> VariantPrices { get; set; }
    }

    public class ProductVariantPriceDTO
    {
        public string SKU { get; set; }
        public decimal Price { get; set; }
    }
}
