using Application.Cqrs.Commands;
using Application.Plans.DTOs;
using Data.Contracts;
using Entities.Plans;
using Entities.Questions;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Commands
{
    public class AddPlanQuestionToPlanCommand(AddPlanQuestionToPlanCommandDTO input) : ICommand<ServiceResult>
    {
        public AddPlanQuestionToPlanCommandDTO Input { get; } = input;
    }

    public class AddPlanQuestionToPlanCommandHandler(IRepository<Plan> planRepository, IRepository<QuestionOption> questionOptionRepository) : ICommandHandler<AddPlanQuestionToPlanCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(AddPlanQuestionToPlanCommand request, CancellationToken cancellationToken)
        {
            var planInDb = await planRepository.Table.Where(t => t.Id == request.Input.PlanId).FirstOrDefaultAsync();
            if (planInDb is null)
                return ServiceResult.NotFound("یافت نشد");

            var planQuestions = request.Input.NestedPlanQuestions.Select(t => PlanQuestion.CreateDefault(planInDb.Id, t.QuestionId, t.IsMainQuestion)).ToList();
            planInDb.AddPlanQuestions(planQuestions);

            await planRepository.UpdateAsync(planInDb, cancellationToken);
            return ServiceResult.Ok();
        }
    }
}
