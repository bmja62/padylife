using Entities.Common;
using Entities.Plans;
using Entities.Questions;

namespace Entities.Users
{
    // پاسخ‌های کاربر
    public class UserPlanAnswer : BaseEntity<long>
    {
        private UserPlanAnswer() { }
        public UserPlanAnswer(long userPlanId, long planQuestionId, long selectedQuestionOptionId)
        {
            UserPlanId = userPlanId;
            PlanQuestionId = planQuestionId;
            SelectedQuestionOptionId = selectedQuestionOptionId;
        }

        //FKs
        public long UserPlanId { get; set; }
        public long PlanQuestionId { get; set; }
        public long SelectedQuestionOptionId { get; set; }

        // Navigations
        public UserPlan UserPlan { get; set; }
        public PlanQuestion PlanQuestion { get; set; }
        public QuestionOption SelectedQuestionOption { get; set; }

        public static UserPlanAnswer CreateDefault(long userPlanId, long questionId, long selectedQuestionOptionId)
        => new(userPlanId, questionId, selectedQuestionOptionId);
    }
}
