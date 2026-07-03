using Microsoft.AspNetCore.Http;

namespace Application.Products.DTOs
{
    public class CreateProductImageDTO
    {
        public IFormFile MainImage { get; set; }
        public List<IFormFile> Gallery { get; set; }
    }
}