using Entities.Plans;

namespace Application.Plans.DTOs
{
    public class GetSignUpPlanDTO
    {
        public GetSignUpPlanDTO(long id, long planCategoryId, string name, string description, Entities.Plans.Plan.PlanStatus status, bool isSignUpPlan, GetSignUpPlanMainQuestionDTO planMainQuestion)
        {
            Id = id;
            PlanCategoryId = planCategoryId;
            Name = name;
            PlanMainQuestion = planMainQuestion;
            Description = description;
            Status = status;
            IsSignUpPlan = isSignUpPlan;
        }

        public long Id { get; }
        public long PlanCategoryId { get; }
        public string Name { get; }
        public GetSignUpPlanMainQuestionDTO PlanMainQuestion { get; }
        public string Description { get; private set; }
        public Plan.PlanStatus Status { get; private set; }
        public bool IsSignUpPlan { get; private set; }

        internal static GetSignUpPlanDTO CreateDefault(
            long id,
            long planCategoryId,
            string name,
            string description,
            Entities.Plans.Plan.PlanStatus status,
            bool isSignUpPlan,
            GetSignUpPlanMainQuestionDTO planMainQuestion)
        => new(
            id,
            planCategoryId,
            name,
            description,
            status,
            isSignUpPlan,
            planMainQuestion
            );
    }
    public class GetSignUpPlanMainQuestionDTO
    {
        public GetSignUpPlanMainQuestionDTO(long id, long questionId, string text, bool isMain, List<GetSignUpPlanMainQuestionQuestioOptionDTO> questioOptions)
        {
            Id = id;
            QuestionId = questionId;
            Text = text;
            IsMain = isMain;
            QuestioOptions = questioOptions;
        }

        public long Id { get; }
        public long QuestionId { get; }
        public string Text { get; }
        public bool IsMain { get; }
        public List<GetSignUpPlanMainQuestionQuestioOptionDTO> QuestioOptions { get; }

        internal static GetSignUpPlanMainQuestionDTO CreateDefault(
            long id,
            long questionId,
            string text,
            bool isMain,
            List<GetSignUpPlanMainQuestionQuestioOptionDTO> questioOptions
            )
        => new(id, questionId, text, isMain, questioOptions);
    }

    public class GetSignUpPlanMainQuestionQuestioOptionDTO
    {
        public GetSignUpPlanMainQuestionQuestioOptionDTO(long id, string text)
        {
            Id = id;
            Text = text;
        }

        public long Id { get; }
        public string Text { get; }

        internal static GetSignUpPlanMainQuestionQuestioOptionDTO CreateDefaut(long id, string text)
         => new(id, text);
    }
}
