namespace Application.Plans.DTOs
{
    public class CreateUserPlanAnswerDTO
    {
        public long UserPlanId { get; set; }
        public long PlanQuestionId { get; set; }
        public long SelectedQuestionOptionId { get; set; }

    }
}
