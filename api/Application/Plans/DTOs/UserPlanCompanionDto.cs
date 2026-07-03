
namespace Application.Plans.DTOs
{
    public class UserPlanCompanionDto
    {
        public long Id { get; set; }
        public long CompanionUserId { get; set; }
        public string CompanionFullName { get; set; }
        public string CompanionAvatar { get; set; }
        public DateTime JoinedAt { get; set; }
    }

}
