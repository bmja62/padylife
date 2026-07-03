using Microsoft.AspNetCore.Http;

namespace Application.Products.DTOs
{
    public class ProductVariantImageDTO
    {
        public IFormFile MainImage { get; set; }
        public List<IFormFile> Gallery { get; set; }
    }
}
