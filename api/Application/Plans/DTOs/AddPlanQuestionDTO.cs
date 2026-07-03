namespace Application.Plans.DTOs
{
    public class AddPlanQuestionDTO
    {
        public long PlanId { get; set; }
        public long QuestionId { get; set; }
        public bool IsMainQuestion { get; set; }

    }
}
