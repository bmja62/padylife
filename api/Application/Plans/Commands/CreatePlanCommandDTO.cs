namespace Application.Plans.Commands
{
    public class CreateOrUpdatePlanCommandDTO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public bool IsSignUpPlan { get; set; }
        public long PlanCategoryId { get; set; }
        public string Level { get; set; }
        public decimal? Price { get; set; }
    }
}
