using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Common.GridResults;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public record GetPlanSubscribersQuery(long planId, GlobalGrid globalGrid, bool? isCompleted) : IQuery<ServiceResult<GlobalGridResult<GetPlanSubscribersDTO>>>;

    public class GetPlanSubscribersQueryHandler(IRepository<User> userRepository) : IQueryHandler<GetPlanSubscribersQuery, ServiceResult<GlobalGridResult<GetPlanSubscribersDTO>>>
    {
        public async Task<ServiceResult<GlobalGridResult<GetPlanSubscribersDTO>>> Handle(GetPlanSubscribersQuery request, CancellationToken cancellationToken)
        {
            var query = userRepository.Table.Include(t => t.UserPlans).Where(a => a.UserPlans.Any(t => t.PlanId == request.planId));

            if (request.isCompleted.HasValue)
                query = query.Where(t => t.UserPlans.Any(a => a.IsCompleted == request.isCompleted));

            var totalCount = await query.CountAsync();
            var data = await query
                .Skip(request.globalGrid.Skip)
                .Take(request.globalGrid.Take)
                .Select(t => GetPlanSubscribersDTO.Create(t.FullName ?? t.UserName, t.ProfileImage)).ToListAsync(cancellationToken);

            return ServiceResult.Ok(new GlobalGridResult<GetPlanSubscribersDTO>
            {
                Data = data,
                TotalCount = totalCount
            });
        }
    }
}
