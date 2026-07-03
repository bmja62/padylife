using Entities.Excersies;
using WebFramework.Api;

namespace PadyLife.Api.Models.DTOs
{
    public class ExerciseCategoryDTO : BaseDto<ExerciseCategoryDTO, ExerciseCategory, long>
    {
        public string Name { get; set; }
    }
}
