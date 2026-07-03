using Asp.Versioning;
using AutoMapper;
using Data.Contracts;
using Entities.Blogs;
using PadyLife.Api.Models.DTOs;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر دسته بندی بلاگ
    /// </summary>
    /// <param name="cityRepository"></param>
    /// <param name="mapper"></param>
    [ApiVersion("1")]
    public class BlogCategoriesController(IRepository<BlogCategory> cityRepository, IMapper mapper) : CrudController<BlogCategoryDTO, BlogCategoryDTO, BlogCategory, long>(cityRepository, mapper)
    {
    }
}
