using Entities.Products;

namespace Application.Products.DTOs
{
    public class GetProductByIdDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public ProductType Type { get; internal set; }
        public GetProductImageDTO ProductImages { get; internal set; }
        public List<GetProductAttributeValueDTO> Attributes { get; set; }
        public List<GetProductVariantDTO> Variants { get; set; }
        public ProductUserInfoDTO UserInfo { get; set; }
        public AvailableOptionsDTO AvailableOptions { get; set; }
        public int BasketQuantity { get; internal set; }
    }

    public class AvailableOptionsDTO
    {
        public List<string> Colors { get; set; }
        public List<string> Sizes { get; set; }
    }
}