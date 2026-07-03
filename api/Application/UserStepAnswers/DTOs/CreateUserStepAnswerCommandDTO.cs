namespace Application.UserStepAnswers.DTOs
{
    public class CreateUserStepAnswerCommandDTO
    {
        public long UserPlanId { get; set; }
        public long ExcersieId { get; set; }
        public long StepId { get; set; }
        public long SelectedStepOptionId { get; set; }

        // مقادیر اختیاری بر اساس نوع StepOption
        public string? Text { get; set; }
        public string? ImageUrl { get; set; }

        // برای MultipleChoice
        public List<long>? SelectedChoiceIds { get; set; }
    }
}
