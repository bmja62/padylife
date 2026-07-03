namespace Application.Plans.DTOs
{
    public class PlanFlowDTO
    {
        public PlanFlowDTO(long planId, long planQuestionId, long selectedQuestionOptionId)
        {
            PlanId = planId;
            CurrentPlanQuestionId = planQuestionId;
            SelectedQuestionOptionId = selectedQuestionOptionId;
        }

        public long PlanId { get; set; }
        public long CurrentPlanQuestionId { get; set; }
        public long SelectedQuestionOptionId { get; set; }

        public static PlanFlowDTO Create(long planId, long planQuestionId, long selectedQuestionOptionId) =>
        new(planId, planQuestionId, selectedQuestionOptionId);
    }

    public class GetNextUnansweredQuestionDTO
    {
        public GetNextUnansweredQuestionDTO(long planId, long userId)
        {
            PlanId = planId;
            UserId = userId;
        }

        public long PlanId { get; set; }
        public long UserId { get; set; }

        public static GetNextUnansweredQuestionDTO Create(long planId, long userId) => new(planId, userId);

    }
}
