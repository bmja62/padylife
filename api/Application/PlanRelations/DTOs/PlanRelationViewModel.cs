namespace Application.PlanRelations.DTOs
{
    public class PlanRelationViewModel
    {
        public long TargetPlanId { get; set; }
        public string TargetTitle { get; set; }
        public bool HasPlan { get; set; }

        public int Order { get; set; }
        public decimal? FinalPrice { get; internal set; }
    }

}
