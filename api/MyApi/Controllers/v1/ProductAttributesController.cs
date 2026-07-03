using Asp.Versioning;
using AutoMapper;
using Data.Contracts;
using Entities.Products;
using PadyLife.Api.Models.DTOs;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// ویژگی های محصول
    /// </summary>
    /// <param name="productAttributeRepository"></param>
    /// <param name="mapper"></param>
    [ApiVersion("1")]
    public class ProductAttributesController(IRepository<ProductAttribute> productAttributeRepository, IMapper mapper) : CrudController<ProductAttributeDTO, ProductAttributeDTO, ProductAttribute, long>(productAttributeRepository, mapper)
    {
    }
}
