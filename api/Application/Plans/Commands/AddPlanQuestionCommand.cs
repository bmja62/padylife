using Application.Cqrs.Commands;
using Application.Plans.DTOs;
using Data.Contracts;
using Entities.Plans;
using Services;

namespace Application.Plans.Commands
{
    public class AddPlanQuestionCommand(AddPlanQuestionDTO input) : ICommand<ServiceResult>
    {
        public AddPlanQuestionDTO Input { get; } = input;
    }

    public class AddPlanQuestionCommandHandler(IRepository<Plan> planRepository) : ICommandHandler<AddPlanQuestionCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(AddPlanQuestionCommand request, CancellationToken cancellationToken)
        {
            var planInDb = await planRepository.GetByIdAsync(cancellationToken, request.Input.PlanId);

            planInDb.AddPlanQuestion(PlanQuestion.CreateDefault(request.Input.PlanId, request.Input.QuestionId, request.Input.IsMainQuestion));

            await planRepository.UpdateAsync(planInDb, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
