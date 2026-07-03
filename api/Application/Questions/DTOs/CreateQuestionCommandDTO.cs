namespace Application.Questions.DTOs
{
    public class CreateQuestionCommandDTO
    {

        public long QuestionCategoryId { get; set; }
        public string Text { get; set; }
        public List<CreateQuestionOptionDTO> QuestionOptions { get; set; }
        public string DisplayText { get; set; }

    }
}
