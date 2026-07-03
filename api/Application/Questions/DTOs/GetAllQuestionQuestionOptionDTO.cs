
using Application.Excersies.DTOs;

namespace Application.Questions.DTOs
{
    public class GetAllQuestionQuestionOptionDTO
    {
        public GetAllQuestionQuestionOptionDTO(long id, long questionId, string text, int priority)
        {
            Id = id;
            QuestionId = questionId;
            Text = text;
            Priority = priority;
        }
        public long Id { get; internal set; }
        public long QuestionId { get; internal set; }
        public string Text { get; internal set; }
        public int Priority { get; set; }
        public static GetAllQuestionQuestionOptionDTO CreateDefault(long id, long questionId, string text, int priority)
        => new(id, questionId, text, priority);
    }


    // DTO برای وضعیت پاسخ‌های کاربر در یک پلن
    public class UserPlanStatusDTO
    {
        public long PlanId { get; set; }
        public string PlanName { get; set; }
        public long UserPlanId { get; internal set; }
        public string PlanLevel { get; internal set; }
        public List<UserAnsweredQuestionDTO> AnsweredQuestions { get; set; }
        public List<ExerciseDTO> LastAnswerExercises { get; set; }
        public GetByIdQuestionDTO NextUnansweredQuestion { get; set; }
        public string ImageUrl { get; set; }
    }

    // DTO برای سوالات پاسخ داده شده
    public class UserAnsweredQuestionDTO
    {
        public long UserPlanId { get; internal set; }
        public long PlanQuestionId { get; set; }
        public string QuestionText { get; set; }
        public long SelectedOptionId { get; set; }
        public string SelectedOptionText { get; set; }
        public DateTime AnswerDate { get; set; }
        public long PlanId { get; internal set; }
        public string PlanTitle { get; internal set; }
    }

}
