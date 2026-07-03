using Application.Products.DTOs;

namespace Application.Baskets.DTOs
{
    public class BasketItemDetailsDTO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; } // Product یا Variant
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;

        public string Brand { get; set; }


        // برای variant:
        public string VariantAttributes { get; set; }
        public GetProductImageDTO ImageUrl { get; internal set; }
    }
}
