using Entities.Questions;
using WebFramework.Api;

namespace PadyLife.Api.Models.DTOs
{
    public class QuestionCategoryDTO : BaseDto<QuestionCategoryDTO, QuestionCategory, long>
    {
        public string Name { get; set; }
    }
}
