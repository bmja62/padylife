namespace Application.Plans.DTOs
{
    public class PlanQuestionDTO
    {
        public PlanQuestionDTO(long planId, long questionId, string text, string displayText, bool isMain, List<PlanQuestionOptionDTO> questionOptions, List<ReadOnlyQuestionOptionsDTO> readOnlyQuestionOptions)
        {
            PlanQuestionId = planId;
            QuestionId = questionId;
            Text = text;
            DisplayText = displayText;
            IsMain = isMain;
            QuestionOptions = questionOptions;
            ReadOnlyQuestionOptions = readOnlyQuestionOptions;
        }

        public long PlanQuestionId { get; set; }
        public long QuestionId { get; set; }
        public string Text { get; set; }
        public string DisplayText { get; set; }
        public bool? IsMain { get; set; }
        public List<PlanQuestionOptionDTO> QuestionOptions { get; set; }
        public List<ReadOnlyQuestionOptionsDTO> ReadOnlyQuestionOptions { get; set; }

        internal static PlanQuestionDTO CreateDefault(long planId, long questionId, string text, string displayText, bool isMain, List<PlanQuestionOptionDTO> questionOptions, List<ReadOnlyQuestionOptionsDTO> readOnlyQuestionOptions)
        => new(planId, questionId, text, displayText, isMain, questionOptions, readOnlyQuestionOptions);
    }
}
