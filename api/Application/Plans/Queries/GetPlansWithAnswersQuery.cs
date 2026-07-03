using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Common.GridResults;
using Common.Roles;
using Common.Utilities;
using Data.Contracts;
using Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;


namespace Application.Plans.Queries
{
    public record GetPlansWithAnswersQuery(GetPlansHaveAnswerDto Dto)
      : IQuery<ServiceResult<GlobalGridResult<PlanWithAnswersListItem>>>;


    public class GetPlansWithAnswersQueryHandler(IRepository<UserPlan> _userPlanRepo,
    IHttpContextAccessor httpContextAccessor,
    IRepository<Expert> _expertRepo) : IQueryHandler<GetPlansWithAnswersQuery, ServiceResult<GlobalGridResult<PlanWithAnswersListItem>>>
    {
        public async Task<ServiceResult<GlobalGridResult<PlanWithAnswersListItem>>> Handle(GetPlansWithAnswersQuery request, CancellationToken cancellationToken)
        {
            var userId = httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();
            var isAdmin = httpContextAccessor.HttpContext.User.IsInRole(UserRoles.Admin.Name);

            var validUserPlanQuery = _userPlanRepo.TableNoTracking
                .Where(up => !up.IsDeleted)
                .Where(up => !up.Plan.IsDeleted)
                .Where(up => up.Answers.Any(a => !a.IsDeleted));

            if (!isAdmin)
            {
                validUserPlanQuery = validUserPlanQuery.Where(up => up.Experts.Any(e => e.ExpertId == userId));
            }

            if (!string.IsNullOrWhiteSpace(request.Dto.Search))
            {
                validUserPlanQuery = validUserPlanQuery.Where(up => up.Plan.Title.Contains(request.Dto.Search!));
            }

            var planData = await validUserPlanQuery
                .Select(up => new { up.PlanId, up.Plan.Title })
                .ToListAsync();

            var grouped = planData
                .GroupBy(x => new { x.PlanId, x.Title })
                .Select(g => new PlanWithAnswersListItem(
                    g.Key.PlanId,
                    g.Key.Title,
                    g.Count()
                ))
                .OrderByDescending(x => x.CountusersWhoResponded)
                .ThenBy(x => x.PlanTitle)
                .ToList();

            var total = grouped.Count;

            var data = grouped
                .Skip(request.Dto.Skip)
                .Take(request.Dto.Take)
                .ToList();

            return ServiceResult.Ok(new GlobalGridResult<PlanWithAnswersListItem>
            {
                Data = data,
                TotalCount = total
            });


        }
    }
}
