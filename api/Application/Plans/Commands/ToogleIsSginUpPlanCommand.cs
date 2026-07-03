using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Plans;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Commands
{
    public class ToogleIsSginUpPlanCommand(long planId) : ICommand<ServiceResult>
    {
        public long PlanId { get; } = planId;
    }

    public class ToogleIsSginUpPlanCommandHandler(IRepository<Plan> planRepository) : ICommandHandler<ToogleIsSginUpPlanCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(ToogleIsSginUpPlanCommand request, CancellationToken cancellationToken)
        {
            var plans = await planRepository.Table.ToListAsync();
            plans.ForEach(item => item.SetIsSignUpPlan(false));
            plans.Where(t => t.Id == request.PlanId).FirstOrDefault().SetIsSignUpPlan(true);
            await planRepository.UpdateRangeAsync(plans, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
