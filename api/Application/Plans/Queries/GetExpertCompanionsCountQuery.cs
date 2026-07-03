using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public record GetExpertCompanionsCountQuery(long expertId, bool? isCompleted) : IQuery<ServiceResult<GetExpertCompanionsCountDTO>>;

    public class GetExpertCompanionsCountQueryHandler(IRepository<UserPlanExpert> userPlanExpertRepository, IRepository<UserPlan> userPlanRepository) : IQueryHandler<GetExpertCompanionsCountQuery, ServiceResult<GetExpertCompanionsCountDTO>>
    {
        public async Task<ServiceResult<GetExpertCompanionsCountDTO>> Handle(GetExpertCompanionsCountQuery request, CancellationToken cancellationToken) =>
            ServiceResult.Ok(
                GetExpertCompanionsCountDTO.Init(
                    await userPlanExpertRepository.TableNoTracking.Where(t => t.ExpertId == request.expertId &&
                    userPlanRepository.Table.Where(tt => request.isCompleted.HasValue ? tt.IsCompleted == request.isCompleted : true).Select(a => a.Id).Contains(t.UserPlanId)
                    ).CountAsync(cancellationToken)
                    ));
    }
}
