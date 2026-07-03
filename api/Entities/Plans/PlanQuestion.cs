using Entities.Common;
using Entities.Questions;
using Entities.Users;

namespace Entities.Plans
{
    public class PlanQuestion : BaseEntity<long>
    {
        //Ctors
        public PlanQuestion(
            long planId,
            long questionId,
            bool isMain
        )
        {
            PlanId = planId;
            QuestionId = questionId;
            IsMain = isMain;
        }

        private PlanQuestion() { }

        //Props
        public bool IsMain { get; set; } = false;

        //FKs
        public long PlanId { get; set; }
        public long QuestionId { get; set; }


        // Navigations
        public Plan Plan { get; set; }
        public Question Question { get; set; }
        public List<QuestionLinked> QuestionLinks { get; set; } = new();
        public List<QuestionLinked> PlanQuestionLinks { get; set; } = new();
        public List<UserPlanAnswer> UserPlanQuestionAnswers { get; set; } = new();

        //Factory Method 
        public static PlanQuestion CreateDefault(long planId, long questionId, bool isMain) => new(planId, questionId, isMain);
    }
}
