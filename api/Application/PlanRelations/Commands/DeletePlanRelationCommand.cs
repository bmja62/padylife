using Application.Cqrs.Commands;
using Application.PlanRelations.DTOs;
using Data.Contracts;
using Entities.Plans;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.PlanRelations.Commands
{
    public record DeletePlanRelationCommand(DeletePlanRelationDto Dto) : ICommand<ServiceResult>;

    public class DeletePlanRelationCommandHandler(IRepository<PlanRelation> relationRepo)
        : ICommandHandler<DeletePlanRelationCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(DeletePlanRelationCommand request, CancellationToken cancellationToken)
        {
            var relation = await relationRepo.Table
                .FirstOrDefaultAsync(r => r.SourcePlanId == request.Dto.SourcePlanId
                                       && r.TargetPlanId == request.Dto.TargetPlanId, cancellationToken);

            if (relation == null)
                return ServiceResult.NotFound("رابطه یافت نشد");

            await relationRepo.DeleteAsync(relation, cancellationToken);
            return ServiceResult.Ok();
        }
    }

}
