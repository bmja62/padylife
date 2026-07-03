using Application.Cqrs.Commands;
using Application.PlanRelations.DTOs;
using Data.Contracts;
using Entities.Plans;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.PlanRelations.Commands
{
    public record CreatePlanRelationCommand(CreatePlanRelationDto Dto) : ICommand<ServiceResult>;

    public class CreatePlanRelationCommandHandler(IRepository<PlanRelation> relationRepo, IRepository<Plan> PlanRepo)
        : ICommandHandler<CreatePlanRelationCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CreatePlanRelationCommand request, CancellationToken cancellationToken)
        {
            // چک وجود داشتن پلن‌ها
            var sourceExists = await PlanRepo.TableNoTracking
                .AnyAsync(p => p.Id == request.Dto.SourcePlanId, cancellationToken);

            var targetExists = await PlanRepo.TableNoTracking
                .AnyAsync(p => p.Id == request.Dto.TargetPlanId, cancellationToken);

            if (!sourceExists || !targetExists)
                return ServiceResult.NotFound("پلن مبدا یا مقصد یافت نشد");

            // چک اتصال قبلی
            var existForSource = await relationRepo.TableNoTracking
                .AnyAsync(r => r.SourcePlanId == request.Dto.SourcePlanId, cancellationToken);

            var existForTarget = await relationRepo.TableNoTracking
                .AnyAsync(r => r.TargetPlanId == request.Dto.TargetPlanId, cancellationToken);

            if (existForSource || existForTarget)
                return ServiceResult.BadRequest("این پلن قبلاً به پلن دیگری متصل شده است. ابتدا حذف کنید.");

            // ایجاد رابطه
            var relation = new PlanRelation
            {
                SourcePlanId = request.Dto.SourcePlanId,
                TargetPlanId = request.Dto.TargetPlanId,
                Order = request.Dto.Order
            };

            await relationRepo.AddAsync(relation, cancellationToken);
            return ServiceResult.Ok();
        }

    }


}
