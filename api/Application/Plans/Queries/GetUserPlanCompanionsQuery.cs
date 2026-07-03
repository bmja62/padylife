using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Common;
using Common.GridResults;
using Common.Utilities;
using Data.Contracts;
using Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public record GetUserPlanCompanionsQuery(GetUserPlanCompanionsRequestDto Input)
      : IQuery<ServiceResult<GlobalGridResult<UserPlanCompanionDto>>>;
    public class GetUserPlanCompanionsQueryHandler(
        IRepository<UserPlanCompanion> companionRepo,
        IRepository<UserPlan> planRepo,
        IHttpContextAccessor accessor
    ) : IQueryHandler<GetUserPlanCompanionsQuery, ServiceResult<GlobalGridResult<UserPlanCompanionDto>>>
    {
        public async Task<ServiceResult<GlobalGridResult<UserPlanCompanionDto>>> Handle(
            GetUserPlanCompanionsQuery request,
            CancellationToken cancellationToken)
        {
            var currentUserId = accessor.HttpContext.User.Identity.GetUserId<long>();

            var Input = request.Input;
            // بررسی مالکیت پلن
            var userPlan = await planRepo.GetByIdAsync(cancellationToken, Input.UserPlanId);
            if (userPlan == null || userPlan.UserId != currentUserId)
                return ServiceResult.Fail<GlobalGridResult<UserPlanCompanionDto>>(null, "پلن یافت نشد یا متعلق به شما نیست.", ApiResultStatusCode.BadRequest);
            // کوئری اصلی همراهان
            var query = companionRepo.Table
         .Include(a => a.CompanionUser)
         .Where(c => c.UserPlanId == Input.UserPlanId && c.CompanionUser != null)
         .Select(c => new UserPlanCompanionDto
         {
             Id = c.Id,
             CompanionUserId = c.CompanionUserId,
             CompanionFullName = c.CompanionUser.FullName ?? c.CompanionUser.UserName,
             CompanionAvatar = c.CompanionUser.ProfileImage,
             JoinedAt = c.CreatedAt
         })
         .AsQueryable();


            if (!string.IsNullOrWhiteSpace(Input.Search))
            {
                var searchTerm = Input.Search.Trim();
                query = query.Where(c => c.CompanionFullName.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var data = await query
                .Skip(Input.Skip)
                .Take(Input.Take)
                .ToListAsync(cancellationToken);

            return ServiceResult.Ok(new GlobalGridResult<UserPlanCompanionDto>
            {
                Data = data,
                TotalCount = totalCount
            });
        }
    }

}
