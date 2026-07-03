using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Common.GridResults;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public record GetExpertsThatPeresentPlanIdQuery(GlobalGrid globalGrid, long planId) : IQuery<ServiceResult<GlobalGridResult<GetExpertsThatPeresentPlanIdDTO>>>;

    public class GetExpertsThatPeresentPlanIdQueryHandler(IRepository<Expert> expertRepository, IRepository<UserPlanExpert> userPlanExpertRepository, IRepository<UserPlan> userPlanRepository) : IQueryHandler<GetExpertsThatPeresentPlanIdQuery, ServiceResult<GlobalGridResult<GetExpertsThatPeresentPlanIdDTO>>>
    {
        public async Task<ServiceResult<GlobalGridResult<GetExpertsThatPeresentPlanIdDTO>>> Handle(GetExpertsThatPeresentPlanIdQuery request, CancellationToken cancellationToken)
        {
            var validPlanIds = userPlanRepository.Table.Where(t => t.PlanId == request.planId).Select(t => t.Id);
            var validExpertIds = userPlanExpertRepository.Table.Where(t => validPlanIds.Contains(t.UserPlanId)).Select(t => t.ExpertId);
            var query = expertRepository.Table.Where(t => validExpertIds.Contains(t.Id));

            var data = await query.Select(t => GetExpertsThatPeresentPlanIdDTO.Create(t)).Skip(request.globalGrid.Skip).Take(request.globalGrid.Take).ToListAsync(cancellationToken);
            var totalCount = await query.CountAsync(cancellationToken);

            return ServiceResult.Ok(new GlobalGridResult<GetExpertsThatPeresentPlanIdDTO>
            {
                Data = data,
                TotalCount = totalCount
            });
        }
    }
}
