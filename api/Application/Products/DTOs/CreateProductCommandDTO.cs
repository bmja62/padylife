using Entities.UploadedFiles;
using Microsoft.AspNetCore.Http;

namespace Application.Products.DTOs
{
    public class CreateProductCommandDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public long CategoryId { get; set; }
        public int? StockQuantity { get; set; } = null;
        public CreateProductImageDTO ProductImage { get; set; }
        public List<ProductAttributeValueDTO> AttributeValues { get; set; }
        public List<CreateProductVariantDTO> Variants { get; set; }
    }


    public class ChangeMainImageDTO
    {
        public long ObjectId { get; set; }
        public UploadType Type { get; set; }
        public IFormFile MainImage { get; set; }
    }
    public class AddImageToGalleryDTO
    {
        public long ObjectId { get; set; }
        public UploadType Type { get; set; }
        public IFormFile GalleryImage { get; set; }
    }
}