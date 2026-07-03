using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Common.GridResults;
using Common.Utilities;
using Data.Contracts;
using Entities.Plans;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public record GetAllForUIQuery(GlobalGrid globalGrid, bool? containUserPlans, bool? containSginUpPlans, long? categoryId) : IQuery<ServiceResult<GlobalGridResult<GetPlanDTO>>>;

    public class GetAllForUIQueryHandler(IRepository<Plan> planRepository, IHttpContextAccessor accessor) : IQueryHandler<GetAllForUIQuery, ServiceResult<GlobalGridResult<GetPlanDTO>>>
    {
        public async Task<ServiceResult<GlobalGridResult<GetPlanDTO>>> Handle(GetAllForUIQuery request, CancellationToken cancellationToken)
        {
            var query = planRepository.Table.Where(t => t.Status == Plan.PlanStatus.Active)
                .Include(t => t.UserPlans)
                .Include(t => t.PlanCategory)
                .AsQueryable();

            if (request.containUserPlans.HasValue)
                if (request.containUserPlans.Value == true)
                {
                    var currentUserId = accessor.HttpContext.User.Identity.GetUserId<long>();
                    var userPlanIds = query.Where(t => t.UserPlans.Any(a => a.UserId == currentUserId)).SelectMany(t => t.UserPlans).Select(t => t.PlanId).ToList();
                    query = query.Where(t => !userPlanIds.Contains(t.Id));
                }
            if (request.containSginUpPlans.HasValue)
                query = query.Where(t => t.IsSignUpPlan == request.containSginUpPlans.Value);

            if (request.categoryId.HasValue)
                query = query.Where(t => t.PlanCategoryId == request.categoryId.Value);

            var data = await query.Select(t =>
                GetPlanDTO.CreateDefault(
                    t.Id,
                    t.Title,
                    t.ImageUrl,
                    t.PlanCategoryId,
                    t.PlanCategory.Name,
                    t.Description,
                    t.IsSignUpPlan,
                    t.Status,
                    t.Level,
                    t.Price,
                    t.DiscountPrice,
                    t.FinalPrice,
                    null))
                .Skip(request.globalGrid.Skip)
                .Take(request.globalGrid.Take)
                .ToListAsync(cancellationToken);
            var totalCount = await query.CountAsync(cancellationToken);

            return ServiceResult.Ok(new GlobalGridResult<GetPlanDTO>
            {
                Data = data,
                TotalCount = totalCount
            });
        }
    }
}
