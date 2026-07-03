using Application.Cqrs.Commands;
using Application.Plans.DTOs;
using Data.Contracts;
using Entities.Plans;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Commands
{
    public class AssginPlanToUserCommand(long planId, long userId) : ICommand<ServiceResult<AssginPlanToUserResponseDTO>>
    {
        public long PlanId { get; } = planId;
        public long UserId { get; } = userId;
    }

    public class AssginPlanToUserCommandHandler(IRepository<UserPlan> userPlanRepository, IRepository<Plan> planRepository) : ICommandHandler<AssginPlanToUserCommand, ServiceResult<AssginPlanToUserResponseDTO>>
    {
        public async Task<ServiceResult<AssginPlanToUserResponseDTO>> Handle(AssginPlanToUserCommand request, CancellationToken cancellationToken)
        {
            var isDuplicated = await userPlanRepository.Table.Where(t => t.UserId == request.UserId && t.PlanId == request.PlanId).AnyAsync(cancellationToken);
            if (isDuplicated)
                return ServiceResult.BadRequest<AssginPlanToUserResponseDTO>("این پلن به شخص متصل بوده");

            var planInDb = await planRepository.GetByIdAsync(cancellationToken, request.PlanId);

            UserPlan newUserPlan = UserPlan.CreateDefault(request.PlanId, request.UserId, planInDb.IsSignUpPlan);
            await userPlanRepository.AddAsync(newUserPlan, cancellationToken);

            return ServiceResult.Ok(new AssginPlanToUserResponseDTO
            {
                Id = newUserPlan.Id,
            });
        }
    }
}
