using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Common.GridResults;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public record GetUsersThatAlreadyInPlanIdQuery(GlobalGrid globalGrid, long planId) : IQuery<ServiceResult<GlobalGridResult<GetUsersThatAlreadyInPlanIdDTO>>>;

    public class GetUsersThatAlreadyInPlanIdQueryHandler(IRepository<User> userRepository, IRepository<UserPlanExpert> userPlanExpertRepository, IRepository<UserPlan> userPlanRepository) : IQueryHandler<GetUsersThatAlreadyInPlanIdQuery, ServiceResult<GlobalGridResult<GetUsersThatAlreadyInPlanIdDTO>>>
    {
        public async Task<ServiceResult<GlobalGridResult<GetUsersThatAlreadyInPlanIdDTO>>> Handle(GetUsersThatAlreadyInPlanIdQuery request, CancellationToken cancellationToken)
        {
            var validUserIds = userPlanRepository.Table.Where(t => t.PlanId == request.planId).Select(t => t.UserId);
            var query = userRepository.Table.Where(t => validUserIds.Contains(t.Id));
            var data = await query.Select(t => GetUsersThatAlreadyInPlanIdDTO.Create(t)).Skip(request.globalGrid.Skip).Take(request.globalGrid.Take).ToListAsync(cancellationToken);
            var total = await query.CountAsync(cancellationToken);

            return ServiceResult.Ok(new GlobalGridResult<GetUsersThatAlreadyInPlanIdDTO>
            {
                Data = data,
                TotalCount = total
            });
        }
    }
}
