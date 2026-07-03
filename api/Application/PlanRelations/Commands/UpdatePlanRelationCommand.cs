using Application.Cqrs.Commands;
using Application.PlanRelations.DTOs;
using Data.Contracts;
using Entities.Plans;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.PlanRelations.Commands
{
    public record UpdatePlanRelationCommand(UpdatePlanRelationDto Dto) : ICommand<ServiceResult>;

    public class UpdatePlanRelationCommandHandler(IRepository<PlanRelation> relationRepo)
        : ICommandHandler<UpdatePlanRelationCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(UpdatePlanRelationCommand request, CancellationToken cancellationToken)
        {
            var relation = await relationRepo.Table
                .FirstOrDefaultAsync(r => r.SourcePlanId == request.Dto.SourcePlanId
                                       && r.TargetPlanId == request.Dto.TargetPlanId, cancellationToken);

            if (relation == null)
                return ServiceResult.NotFound("رابطه یافت نشد");

            relation.Order = request.Dto.Order;
            await relationRepo.UpdateAsync(relation, cancellationToken);

            return ServiceResult.Ok();
        }
    }

}
