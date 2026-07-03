using Common.GridResults;

namespace Application.Plans.DTOs
{
    public sealed class GetPlanAnswersRequest : GlobalGrid
    {
        public long PlanId { get; init; }
        public bool? OnlyCompleted { get; init; }
        public bool? ForAdmin { get; set; }
    }
}
