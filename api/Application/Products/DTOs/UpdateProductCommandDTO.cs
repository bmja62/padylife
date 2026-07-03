namespace Application.Products.DTOs
{
    public class UpdateProductCommandDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public long CategoryId { get; set; }
        public List<ProductAttributeValueDTO> AttributeValues { get; set; }
        public List<UpdateProductVariantDTO> Variants { get; set; }
    }
}