using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Plans;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Commands
{
    public class DeletePlanCommand(long id) : ICommand<ServiceResult>
    {
        public long Id { get; } = id;
    }

    public class DeletePlanCommandHander(IRepository<Plan> planRepository) : ICommandHandler<DeletePlanCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(DeletePlanCommand request, CancellationToken cancellationToken)
        {
            var planInDb = await planRepository.Table.Where(t => t.Id == request.Id && !t.IsDeleted).FirstOrDefaultAsync();
            if (planInDb is null)
                return ServiceResult.NotFound("یافت نشد");
            await planRepository.SoftDeleteAsync(planInDb, cancellationToken);
            return ServiceResult.Ok();
        }
    }

}
