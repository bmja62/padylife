namespace Application.Plans.DTOs
{
    public class CreateUserPlanExpertCommandDTO
    {
        public long UserPlanId { get; set; }
        public long ExpertId { get; set; }
        public decimal Price { get; set; }
        public string Specialization { get; set; }
    }
}
