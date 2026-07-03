using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Commands
{
    public record DiactiveExpertPlanByExpertCommand(long planId, long expertId) : ICommand<ServiceResult>;

    public class DiactiveExpertPlanByExpertCommandHandler(IRepository<ExpertPlanPrice> repository) : ICommandHandler<DiactiveExpertPlanByExpertCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(DiactiveExpertPlanByExpertCommand request, CancellationToken cancellationToken)
        {
            var dataInDb = await repository.Table.Where(t => t.ExpertId == request.expertId && t.PlanId == request.planId).FirstOrDefaultAsync(cancellationToken);

            if (dataInDb is null)
                return ServiceResult.NotFound("یافت نشد");

            dataInDb.Deactivate();
            await repository.UpdateAsync(dataInDb,cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
