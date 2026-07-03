using Asp.Versioning;
using AutoMapper;
using Data.Contracts;
using Entities.Plans;
using PadyLife.Api.Models.DTOs;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// دسته بندی پلن
    /// </summary>
    /// <param name="planCategoryRepository"></param>
    /// <param name="mapper"></param>
    [ApiVersion("1")]
    public class PlanCategoriesController(IRepository<PlanCategory> planCategoryRepository, IMapper mapper) : CrudController<PlanCategoryDTO, PlanCategoryDTO, PlanCategory, long>(planCategoryRepository, mapper)
    {
    }
}
