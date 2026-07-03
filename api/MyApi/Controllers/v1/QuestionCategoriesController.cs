using Asp.Versioning;
using AutoMapper;
using Data.Contracts;
using Entities.Questions;
using PadyLife.Api.Models.DTOs;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// دسته بندی سوالات
    /// </summary>
    [ApiVersion("1")]
    public class QuestionCategoriesController(IRepository<QuestionCategory> questionCategoryRepository, IMapper mapper) : CrudController<QuestionCategoryDTO, QuestionCategoryDTO, QuestionCategory, long>(questionCategoryRepository, mapper)
    {
    }
}
