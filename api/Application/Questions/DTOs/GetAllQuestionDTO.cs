namespace Application.Questions.DTOs
{
    public class GetAllQuestionDTO
    {
        public long Id { get; internal set; }
        public long QuestionCategoryId { get; internal set; }
        public string QuestionCategoryName { get; internal set; }
        public string Text { get; internal set; }
        public List<GetAllQuestionQuestionOptionDTO> Options { get; internal set; }
        public string DisplayText { get; internal set; }
    }
}
