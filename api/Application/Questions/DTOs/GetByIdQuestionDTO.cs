namespace Application.Questions.DTOs
{
    public class GetByIdQuestionDTO
    {
        public GetByIdQuestionDTO(long id, long questionCategoryId, string questionCategoryName, string text, string displayText, List<GetAllQuestionQuestionOptionDTO> options)
        {
            Id = id;
            QuestionCategoryId = questionCategoryId;
            QuestionCategoryName = questionCategoryName;
            Text = text;
            Options = options;
            DisplayText = displayText;
        }

        public long Id { get; internal set; }
        public long QuestionCategoryId { get; internal set; }
        public string QuestionCategoryName { get; internal set; }
        public string Text { get; internal set; }
        public List<GetAllQuestionQuestionOptionDTO> Options { get; internal set; }
        public string DisplayText { get; internal set; }

        public static GetByIdQuestionDTO CreateDefault(long id, long questionCategoryId, string questionCategoryName, string text, string displayText, List<GetAllQuestionQuestionOptionDTO> options)
        => new(id, questionCategoryId, questionCategoryName, text,displayText, options);
    }
}
