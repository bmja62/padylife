using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public record GetPlanStartStatsQuery(long UserPlanId) : IQuery<ServiceResult<GetPlanStartStatsDto>>;

    public class GetPlanStartStatsQueryHandler(IRepository<UserPlan> UserPlanRepository) : IQueryHandler<GetPlanStartStatsQuery, ServiceResult<GetPlanStartStatsDto>>
    {
        public async Task<ServiceResult<GetPlanStartStatsDto>> Handle(GetPlanStartStatsQuery request, CancellationToken cancellationToken)
        {

            var MyPlan = await UserPlanRepository.TableNoTracking.Where(a => a.Id == request.UserPlanId).FirstOrDefaultAsync(cancellationToken);
            if (MyPlan == null)
                return ServiceResult.BadRequest<GetPlanStartStatsDto>("Plan Not Found");
            var result = await UserPlanRepository.TableNoTracking
                .Where(a => a.CreatedAt.Date == MyPlan.CreatedAt.Date && a.PlanId == MyPlan.PlanId && MyPlan.Id != a.Id)
                .CountAsync(cancellationToken);

            return ServiceResult<GetPlanStartStatsDto>.Ok(new GetPlanStartStatsDto() { Count = result });
        }
    }
}
