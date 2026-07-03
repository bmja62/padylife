using Microsoft.AspNetCore.Http;

namespace Application.Products.DTOs
{
    public class AddProductMediaDTO
    {
        public IFormFile MainImage { get; set; }
        public List<IFormFile> GalleryImages { get; set; }
    }
}
