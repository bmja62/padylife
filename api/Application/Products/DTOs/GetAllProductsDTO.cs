using Entities.Products;
using Services.Services.MediaServices.DTOs;

namespace Application.Products.DTOs
{
    public class GetAllProductsDTO
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
    }
    public class GetProductImageDTO
    {
        public GetProductImageDTO(GetObjectMediaDTO main, List<GetObjectMediaDTO> gallery)
        {
            Main = main;
            Gallery = gallery;
        }
        public GetObjectMediaDTO Main { get; set; }
        public List<GetObjectMediaDTO> Gallery { get; set; }

        internal static GetProductImageDTO Create(GetObjectMediaDTO main, List<GetObjectMediaDTO> gallery)
        => new(main, gallery);
    }
}