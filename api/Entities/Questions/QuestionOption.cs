
using Entities.Common;
using Entities.Users;

namespace Entities.Questions
{
    // مدل گزینه‌های پاسخ
    public class QuestionOption : BaseEntity<long>
    {



        public QuestionOption(string text)
        {
            Text = text;
        }

        private QuestionOption()
        {

        }

        public QuestionOption(string text, long questionId)
        {
            QuestionId = questionId;
            Text = text;
        }
        public QuestionOption(string text, int priority)
        {
            Priority = priority;
            Text = text;
        }

        //FKs
        public long QuestionId { get; set; }

        //Props
        public string Text { get; set; }
        public int Priority { get; set; } = 0;

        //Navigations
        public Question Question { get; set; }
        public List<UserPlanAnswer> SelectedOptionAnswers { get; set; } = new();
        public List<QuestionLinked> QuestionLinks { get; set; } = new();

        public static QuestionOption CreateDefault(string text, int priority) => new(text, priority);

        public static QuestionOption CreateWithQuestionId(string text, long questionId) => new(text, questionId);

        public void SetText(string text) => Text = text;

        public void SetPriority(int priority) => Priority = priority;
    }
}
