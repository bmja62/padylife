using Application.Excersies.DTOs;
using Application.Questions.DTOs;

namespace Application.Plans.DTOs
{
    public class GetPlanFlowDTO
    {
        public GetPlanFlowDTO(List<GetAllExcersieDTO> excersies)
        {
            Excersie = excersies;
        }

        public GetPlanFlowDTO(GetByIdQuestionDTO question)
        {
            Question = question;
        }

        public GetPlanFlowDTO(long planId, long planQuestionId, long selectedQuestionOptionId)
        {
            PlanFlow = PlanFlowDTO.Create(planId, planQuestionId, selectedQuestionOptionId);
        }

        public List<GetAllExcersieDTO> Excersie { get; set; }
        public GetByIdQuestionDTO Question { get; set; }
        public PlanFlowDTO PlanFlow { get; internal set; }

        internal static GetPlanFlowDTO CreateExercise(List<GetAllExcersieDTO> excersie)
        => new(excersie);

        internal static GetPlanFlowDTO CreateQuestion(GetByIdQuestionDTO question)
        => new(question);

        internal static GetPlanFlowDTO SetPlanFlow(long planId, long planQuestionId, long selectedQuestionOptionId)
        => new(planId, planQuestionId, selectedQuestionOptionId);
    }
}
