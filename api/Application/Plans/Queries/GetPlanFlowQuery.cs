using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Application.Plans.Helpers;
using Data.Contracts;
using Entities.Plans;
using Entities.Questions;
using Services;

namespace Application.Plans.Queries
{
    public class GetPlanFlowQuery(PlanFlowDTO input) : IQuery<ServiceResult<GetPlanFlowDTO>>
    {
        public PlanFlowDTO Input { get; } = input;
    }

    public class GetPlanFlowQueryHandler(IRepository<QuestionLinked> questionLinkedRepository, IRepository<PlanQuestion> planQuestionRepository) : IQueryHandler<GetPlanFlowQuery, ServiceResult<GetPlanFlowDTO>>
    {
        public async Task<ServiceResult<GetPlanFlowDTO>> Handle(GetPlanFlowQuery request, CancellationToken cancellationToken)
        => ServiceResult.Ok(PlanFlowService.GetNextStep(
                questionLinkedRepository.Table,
                planQuestionRepository.Table,
                request.Input.PlanId,
                request.Input.CurrentPlanQuestionId,
                request.Input.SelectedQuestionOptionId
                ));
    }
}
