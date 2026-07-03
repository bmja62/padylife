using Application.Cqrs.Queris;
using Application.PlanRelations.DTOs;
using Common.Utilities;
using Data.Contracts;
using Entities.Plans;
using Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;


namespace Application.PlanRelations.Queries
{
    public record GetPlanRelationsQuery(long PlanId) : IQuery<ServiceResult<PlanRelationsResultDto>>;

    public class GetPlanRelationsQueryHandler(IRepository<Plan> planRepo, IHttpContextAccessor httpContextAccessor, IRepository<UserPlan> UserPlanRepository)
        : IQueryHandler<GetPlanRelationsQuery, ServiceResult<PlanRelationsResultDto>>
    {
        public async Task<ServiceResult<PlanRelationsResultDto>> Handle(GetPlanRelationsQuery request, CancellationToken cancellationToken)
        {
            var userId = httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();

            // همه‌ی پلن‌هایی که یوزر داره
            var userPlanIds = await UserPlanRepository.Table
                .Where(up => up.UserId == userId)
                .Select(up => up.PlanId)
                .ToListAsync(cancellationToken);

            var plan = await planRepo.Table
                .Include(p => p.NextPlans).ThenInclude(r => r.TargetPlan)
                .Include(p => p.PreviousPlans).ThenInclude(r => r.SourcePlan)
                .FirstOrDefaultAsync(p => p.Id == request.PlanId, cancellationToken);

            if (plan == null)
                return ServiceResult.NotFound<PlanRelationsResultDto>("پلن یافت نشد");

            var dto = new PlanRelationsResultDto
            {
                PlanId = plan.Id,
                Title = plan.Title,
                FinalPrice = plan.FinalPrice,
                NextPlans = plan.NextPlans
                    .OrderBy(r => r.Order)
                    .Select(r => new PlanRelationViewModel
                    {
                        TargetPlanId = r.TargetPlanId,
                        TargetTitle = r.TargetPlan?.Title,
                        Order = r.Order,
                        HasPlan = userPlanIds.Contains(r.TargetPlanId),
                        FinalPrice = r.TargetPlan?.FinalPrice
                    }).ToList(),
                PreviousPlans = plan.PreviousPlans
                    .OrderBy(r => r.Order)
                    .Select(r => new PlanRelationViewModel
                    {
                        TargetPlanId = r.TargetPlanId,
                        TargetTitle = plan.Title,
                        Order = r.Order,
                        HasPlan = userPlanIds.Contains(r.TargetPlanId), 
                        FinalPrice = r.TargetPlan?.FinalPrice
                    }).ToList()
            };

            return ServiceResult.Ok(dto);
        }
    }

}
