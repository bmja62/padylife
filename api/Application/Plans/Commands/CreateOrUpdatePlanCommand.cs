using Application.Cqrs.Commands;
using Application.Plans.DTOs;
using Common.Utilities;
using Data.Contracts;
using Entities.Plans;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Commands
{
    public class CreateOrUpdatePlanCommand(CreateOrUpdatePlanCommandDTO input) : ICommand<ServiceResult<GetPlanDTO>>
    {
        public CreateOrUpdatePlanCommandDTO Input { get; } = input;
    }

    public class CreatePlanCommandHandler(IRepository<Plan> planRepository, IHttpContextAccessor accessor) : ICommandHandler<CreateOrUpdatePlanCommand, ServiceResult<GetPlanDTO>>
    {
        public async Task<ServiceResult<GetPlanDTO>> Handle(CreateOrUpdatePlanCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = accessor.HttpContext.User.Identity.GetUserId<long>();
            if (currentUserId <= 0)
                return ServiceResult.BadRequest<GetPlanDTO>("لطفا ابتدا لاگین کنید");

            if (request.Input.IsSignUpPlan)
                if (await planRepository.Table.AnyAsync(t => t.IsSignUpPlan && t.Status == Plan.PlanStatus.Active))
                    return ServiceResult.BadRequest<GetPlanDTO>("شما در حال حاضر یک پلن فعال دارید");

            var planInDb = await planRepository.Table.Where(t => t.Id == request.Input.Id).FirstOrDefaultAsync();
            if (planInDb is null)
            {
                Plan newPlan = Plan.CreateDefault
                    (
                    request.Input.Title,
                    request.Input.ImageUrl,
                    request.Input.PlanCategoryId,
                    request.Input.IsSignUpPlan,
                    request.Input.Description,
                    request.Input.Level,
                    currentUserId,
                    request.Input.Price
                    );
                await planRepository.AddAsync(newPlan, cancellationToken);
                return ServiceResult.Ok(GetPlanDTO.GetPlanId(newPlan.Id));
            }

            planInDb.SetTitle(request.Input.Title);
            planInDb.SetPlanCategoryId(request.Input.PlanCategoryId);
            planInDb.SetIsSignUpPlan(request.Input.IsSignUpPlan);
            planInDb.SetDescription(request.Input.Description);
            planInDb.SetLevel(request.Input.Level);
            planInDb.SetPrice(request.Input.Price);
            planInDb.SetImageUrl(request.Input.ImageUrl);

            await planRepository.UpdateAsync(planInDb, cancellationToken);
            return ServiceResult.Ok(GetPlanDTO.GetPlanId(planInDb.Id));
        }
    }
}
