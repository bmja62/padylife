namespace Application.PlanRelations.DTOs
{
    public class PlanRelationsResultDto
    {
        public long PlanId { get; set; }
        public string Title { get; set; }

        public List<PlanRelationViewModel> NextPlans { get; set; } = new();
        public List<PlanRelationViewModel> PreviousPlans { get; set; } = new();
        public decimal? FinalPrice { get; internal set; }
    }

}
