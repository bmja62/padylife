using Entities.Products;

namespace Application.Products.DTOs
{
    // مدل‌های مرحله‌بندی شده ساخت محصول
    public class CreateProductBasicInfoDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long CategoryId { get; set; }
        public ProductType Type { get; set; } // Simple یا Variant
    }
    public class CreateProductBasicInfoResponse
    {
        public long Id { get; set; }
    }
}
