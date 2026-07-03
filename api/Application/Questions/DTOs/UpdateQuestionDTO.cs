namespace Application.Questions.DTOs
{
    public class UpdateQuestionDTO
    {
        public string Text { get; set; }
        public long QuestionCategoryId { get; set; }
        public string DisplayText { get; set; }
    }
}
