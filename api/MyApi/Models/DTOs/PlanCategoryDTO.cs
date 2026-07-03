using Entities.Plans;
using WebFramework.Api;

namespace PadyLife.Api.Models.DTOs
{
    public class PlanCategoryDTO : BaseDto<PlanCategoryDTO, PlanCategory, long>
    {
        public string Name { get; set; }
    }
}
