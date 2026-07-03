using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Application.Plans.Helpers;
using Application.Questions.DTOs;
using Common.GridResults;
using Common.Utilities;
using Data.Contracts;
using Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Plans.Queries
{
    public record GetUserPlansPlansQuery(long userId, int? pageNumber, int? count, bool? allExperts) : IQuery<ServiceResult<GlobalGridResult<GetPlanDTO>>>;
    public class GetUserPlansPlansQueryHandler(IRepository<UserPlan> userPlansQuery,IHttpContextAccessor httpContextAccessor) : IQueryHandler<GetUserPlansPlansQuery, ServiceResult<GlobalGridResult<GetPlanDTO>>>
    {
        public async Task<ServiceResult<GlobalGridResult<GetPlanDTO>>> Handle(GetUserPlansPlansQuery request, CancellationToken cancellationToken)
        {
            var query = userPlansQuery.Table.Include(t => t.Experts).Include(t => t.Plan).Where(t => t.UserId == request.userId).AsQueryable();

            var creator = httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();

            if (request.allExperts.HasValue && !request.allExperts.Value)
                query = query.Where(t => t.Experts.Any(a => a.ExpertId == creator));

            var data = await query.Select(t => GetPlanDTO.CreateDefault
            (
                    t.Plan.Id,
                    t.Plan.Title,
                    t.Plan.ImageUrl,
                    t.Plan.PlanCategoryId,
                    t.Plan.PlanCategory.Name,
                    t.Plan.Description,
                    t.Plan.IsSignUpPlan,
                    t.Plan.Status,
                    t.Plan.Level,
                    t.Plan.Price,
                    t.Plan.DiscountPrice,
                    t.Plan.FinalPrice,
                    null
                )).Skip((request.pageNumber.Value - 1) * request.count.Value).Take(request.count.Value).ToListAsync(cancellationToken);

            var totalCount = await query.CountAsync(cancellationToken);

            return ServiceResult.Ok(new GlobalGridResult<GetPlanDTO> 
            {
                Data = data,
                TotalCount = totalCount
            });
        }
    }
}
