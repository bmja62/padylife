namespace Application.Reports.DTOs
{
    public class GetUserActivityForAllQuestionsReportDTO
    {
        public long UserId { get; set; }
        public int TotalExercises { get; set; }
        public int TotalSteps { get; set; }
        public int CompletedSteps { get; set; }
        public double CompletionPercentage { get; set; }
        public double AverageCompletionPercentage { get; set; }
        public int ComparisonWithOthers { get; set; } // 1 = بالاتر از میانگین, 0 = برابر, -1 = پایین‌تر
        public List<ExerciseStatDTO> ExerciseStats { get; set; }
    }

    public class ExerciseStatDTO
    {
        public long ExerciseId { get; set; }
        public string ExerciseTitle { get; set; }
        public int TotalSteps { get; set; }
        public int CompletedSteps { get; set; }
        public double CompletionPercentage { get; set; }
    }

    public class UserAnswerDto
    {
        public long UserId { get; set; }
        public long ExcersieId { get; set; }
        public long StepId { get; set; }
        public long? SelectedStepOptionId { get; set; }
        public List<UserSelectedChoiceDto> SelectedChoices { get; set; }
    }

    public class UserSelectedChoiceDto
    {
        public long OptionChoiceId { get; set; }
        public long StepOptionId { get; set; }
    }
    public class OtherUserAnswerDto
    {
        public long UserId { get; set; }
        public long ExcersieId { get; set; }
        public long StepId { get; set; }
        public long? SelectedStepOptionId { get; set; }
        public List<OtherUserSelectedChoiceDto> SelectedChoices { get; set; }
    }

    public class OtherUserSelectedChoiceDto
    {
        public long OptionChoiceId { get; set; }
        public long StepOptionId { get; set; }
    }
}
