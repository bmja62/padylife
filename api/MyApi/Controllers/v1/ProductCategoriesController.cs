using Asp.Versioning;
using AutoMapper;
using Data.Contracts;
using Entities.Products;
using PadyLife.Api.Models.DTOs;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// دسته بندی محصولات
    /// </summary>
    /// <param name="productCategoryRepository"></param>
    /// <param name="mapper"></param>
    [ApiVersion("1")]
    public class ProductCategoriesController(IRepository<ProductCategory> productCategoryRepository, IMapper mapper) : CrudController<ProductCategoryDTO, ProductCategoryDTO, ProductCategory, long>(productCategoryRepository, mapper)
    {
    }
}
