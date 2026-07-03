namespace Application.Plans.DTOs
{
    public class AddPlanQuestionToPlanCommandDTO
    {
        public long PlanId { get; set; }
        public List<NestedPlanQuestionDTO> NestedPlanQuestions { get; set; }
    }

    public class NestedPlanQuestionDTO
    {
        public long QuestionId { get; set; }
        public bool IsMainQuestion { get; set; }
    }

}
