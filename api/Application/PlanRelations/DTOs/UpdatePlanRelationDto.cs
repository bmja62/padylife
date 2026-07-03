namespace Application.PlanRelations.DTOs
{
    public class UpdatePlanRelationDto
    {
        public long SourcePlanId { get; set; }
        public long TargetPlanId { get; set; }
        public int Order { get; set; }
    }

}
