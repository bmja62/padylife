namespace Application.Questions.DTOs
{
    public class UpdateQuestionOptionLinkedsDTO
    {
        public long PlanId { get; set; }
        public long QuestionOptionId { get; set; }
        public UpdateQuestionOptionType Type { get; set; }
        public long ObjectId { get; set; }

    }
    public class UpdateQuestionOptionDTO
    {
        public long QuestionOptionId { get; set; }
        public string Text { get; set; }
        public int Priority { get; set; }
    }

    public enum UpdateQuestionOptionType
    {
        Excercise = 1,
        Question = 2,
    }
}
