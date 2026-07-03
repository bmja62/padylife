using Application.Cqrs.Queris;
using Application.Rates.DTOs;
using Common.Utilities;
using Data.Contracts;
using Entities.Common;
using Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Services.RateServices;

namespace Application.Rates.Queries
{
    public record class IsRateExistQuery(long entityId, EntityType entityType) : IQuery<ServiceResult<IsRateExistDTO>>
    {

    }

    public class IsRateExistQueryHandler(IHttpContextAccessor accessor, IRateService rateService, IRepository<UserPlan> userPlanRepository) : IQueryHandler<IsRateExistQuery, ServiceResult<IsRateExistDTO>>
    {
        public async Task<ServiceResult<IsRateExistDTO>> Handle(IsRateExistQuery request, CancellationToken cancellationToken)
        {
            var a = (new IsRateExistDTO
            {
                IsRated = await rateService.IsRateExist(accessor.HttpContext.User.Identity.GetUserId<long>(), request.entityId, request.entityType),
            });
            if (request.entityType == EntityType.Specialist)
            {
                var userId = accessor.HttpContext.User.Identity.GetUserId<long>();
                var hasPermission = await userPlanRepository.TableNoTracking
                    .AnyAsync(a => a.UserId == userId && a.Experts.Any(e => e.ExpertId == request.entityId), cancellationToken);
                a.CanGiveRate = hasPermission;
            }
            return ServiceResult.Ok(a);


        }
    }
}
