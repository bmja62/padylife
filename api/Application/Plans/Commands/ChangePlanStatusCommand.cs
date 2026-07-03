using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Plans;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Commands
{
    public class ChangePlanStatusCommand(long id, Plan.PlanStatus status) : ICommand<ServiceResult>
    {
        public long Id { get; } = id;
        public Plan.PlanStatus Status { get; } = status;
    }
    public class ChangePlanStatusCommandHandler(IRepository<Plan> planRepository) : ICommandHandler<ChangePlanStatusCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(ChangePlanStatusCommand request, CancellationToken cancellationToken)
        {
            var planInDb = await planRepository.Table.Where(t => t.Id == request.Id).FirstOrDefaultAsync();
            if (planInDb == null)
                return ServiceResult.NotFound("یافت نشد");

            planInDb.SetStatus(request.Status);
            await planRepository.UpdateAsync(planInDb, cancellationToken);

            return ServiceResult.Ok();
        }
    }

}
