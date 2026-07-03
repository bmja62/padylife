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
    public record GetAllPlanQuery(GetAllPlanFilter input) : IQuery<ServiceResult<GlobalGridResult<GetPlanDTO>>>;

    public class GetAllPlanQueryHandler(IRepository<Plan> planRepository,IHttpContextAccessor httpContextAccessor) : IQueryHandler<GetAllPlanQuery, ServiceResult<GlobalGridResult<GetPlanDTO>>>
    {
        public async Task<ServiceResult<GlobalGridResult<GetPlanDTO>>> Handle(GetAllPlanQuery request, CancellationToken cancellationToken)
        {
            var query = planRepository.Table.Include(t => t.PlanCategory).AsQueryable();

            if (!string.IsNullOrEmpty(request.input.Search))
            {
                query = query.Where(t => t.Title.Contains(request.input.Search));
            }

            var identity = httpContextAccessor.HttpContext.User.Identity;
            var creator = identity.GetUserId<long>();

            if (!identity.IsAdmin())
            {
                if (request.input.UserId.HasValue && request.input.UserId.Value > 0)
                    query = query.Where(t => t.OwnerUserId == request.input.UserId.Value);
            }

            var data = await query
                .Skip(request.input.Skip)
                .Take(request.input.Take)
                .Select(t =>
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
                   null
                   )
                ).ToListAsync();

            var totalCount = await query.CountAsync();

            return ServiceResult.Ok(new GlobalGridResult<GetPlanDTO>
            {
                Data = data,
                TotalCount = totalCount,
            });
        }
    }
}
