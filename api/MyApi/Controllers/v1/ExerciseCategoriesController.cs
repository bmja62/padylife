using Asp.Versioning;
using AutoMapper;
using Data.Contracts;
using Entities.Excersies;
using PadyLife.Api.Models.DTOs;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// دسته بندی تمرینات
    /// </summary>
    [ApiVersion("1")]
    public class ExerciseCategoriesController(IRepository<ExerciseCategory> exerciseCategoryRepository, IMapper mapper) : CrudController<ExerciseCategoryDTO, ExerciseCategoryDTO, ExerciseCategory, long>(exerciseCategoryRepository, mapper)
    {


    }
}
