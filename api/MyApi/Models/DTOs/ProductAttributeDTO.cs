using Entities.Products;
using WebFramework.Api;

namespace PadyLife.Api.Models.DTOs
{
    public class ProductAttributeDTO : BaseDto<ProductAttributeDTO, ProductAttribute, long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public AttributeType Type { get; set; }
    }
}
