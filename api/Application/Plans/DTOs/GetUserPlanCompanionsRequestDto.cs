using Common.GridResults;

namespace Application.Plans.DTOs
{
    public class GetUserPlanCompanionsRequestDto : GlobalGrid
    {
        public long UserPlanId { get; set; }
    }
}
