using Entities.Common;
using Entities.Plans;
using Entities.Users;

namespace Entities.Questions
{
    // مدل سوال
    public class Question : BaseEntity<long>
    {
        public Question(long questionCategoryId, string questionText, string displayText, long creator, List<QuestionOption> questionOptions)
        {
            QuestionCategoryId = questionCategoryId;
            Text = questionText;
            QuestionOptions = questionOptions;
            DisplayText = displayText;
            CreatedByUserId = creator;  
        }

        private Question()
        {

        }

        //FKs
        public long QuestionCategoryId { get; set; }

        //Props
        public string Text { get; set; }
        public string DisplayText { get; set; }
        public long? CreatedByUserId { get; set; }

        //Navigations
        public QuestionCategory QuestionCategory { get; set; }
        public List<QuestionOption> QuestionOptions { get; set; } = new();
        public List<PlanQuestion> PlanQuestions { get; set; } = new();
        public User CreatedByUser { get; set; }

        public static Question CreateDefault(long questionCategory, string questionText,string displayText, long creator, List<QuestionOption> questionOptions) => new(questionCategory, questionText, displayText, creator, questionOptions);

        public void AddQuestionOptions(QuestionOption questionOption) => QuestionOptions.Add(questionOption);

        public void SetQuestionCategoryId(long questionCategoryId) => QuestionCategoryId = questionCategoryId;

        public void SetText(string text) => Text = text;
        public void SetDisplayText(string displayText) => DisplayText = displayText;

        public void SetCreateByUserId(long creator) => CreatedByUserId = creator;
    }

}
