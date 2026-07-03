using Application.Cqrs.Queris;
using Application.Reports.DTOs;
using Common.GridResults;
using Common.Utilities;
using Data.Contracts;
using Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Reports.Queries
{
    public record GetReportPlanQuery(GetReportPlanRequestDto Input) : IQuery<ServiceResult<GlobalGridResult<GetReportPlanResponseDto>>>;

    public class GetReportPlanQueryHandler(IHttpContextAccessor httpContextAccessor, IRepository<ExpertPlanPrice> ExpertPlanPriceRepository, IRepository<UserPlanExpert> UserPlanExpertRepository) : IQueryHandler<GetReportPlanQuery, ServiceResult<GlobalGridResult<GetReportPlanResponseDto>>>
    {
        public async Task<ServiceResult<GlobalGridResult<GetReportPlanResponseDto>>> Handle(GetReportPlanQuery request, CancellationToken cancellationToken)
        {

            var userId = httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();


            try
            {
                var expertPlans = ExpertPlanPriceRepository.TableNoTracking
                    .Include(x => x.Plan)
                    .Where(x => x.ExpertId == userId);

                if (!string.IsNullOrEmpty(request.Input.Search))
                {
                    expertPlans = expertPlans.Where(x => x.Plan.Title.Contains(request.Input.Search));
                }

                var allowedPlans = await expertPlans
                    .Select(x => new { x.PlanId, x.Plan.Title,x.Plan.ImageUrl })
                    .ToListAsync();

                var userPlanQuery = UserPlanExpertRepository.TableNoTracking
                    .Include(x => x.UserPlan)
                        .ThenInclude(up => up.User)
                    .Include(x => x.UserPlan.Plan)
                    .Where(x => x.ExpertId == userId && allowedPlans.Select(p => p.PlanId).Contains(x.UserPlan.PlanId));

                var existingStats = await userPlanQuery
                    .GroupBy(x => x.UserPlan.Plan)
                    .Select(g => new
                    {
                        PlanId = g.Key.Id,
                        PersonCount = g.Select(x => x.UserPlan.UserId).Distinct().Count(),
                        TotalCount = g.Count(),
                        CompletedCount = g.Count(x => x.UserPlan.IsCompleted),
                        MaleCount = g.Count(x => x.UserPlan.User.Gender == GenderType.Male),
                        FemaleCount = g.Count(x => x.UserPlan.User.Gender == GenderType.Female),
                        AgeSum = g.Where(x => x.UserPlan.User.Age > 0).Sum(x => x.UserPlan.User.Age),
                        AgeCount = g.Count(x => x.UserPlan.User.Age > 0)
                    })
                    .ToListAsync();

                var processedStats = existingStats.Select(s => new
                {
                    s.PlanId,
                    s.PersonCount,
                    AverageAge = s.AgeCount > 0 ? (double)s.AgeSum / s.AgeCount : 0,
                    PercentageOfPeopleWhoCompletedThePlan = s.TotalCount > 0 ? (s.CompletedCount * 100.0 / s.TotalCount) : 0,
                    ManGender = s.TotalCount > 0 ? (s.MaleCount * 100.0 / s.TotalCount) : 0,
                    WomanGender = s.TotalCount > 0 ? (s.FemaleCount * 100.0 / s.TotalCount) : 0
                }).ToList();

                var allResults = allowedPlans.Select(plan =>
                {
                    var stats = processedStats.FirstOrDefault(s => s.PlanId == plan.PlanId);

                    return new GetReportPlanResponseDto
                    {
                        PlanId = plan.PlanId,
                        PlanName = plan.Title,
                        PlanImageUrl=plan.ImageUrl,
                        PersonCount = stats?.PersonCount ?? 0,
                        AverageAge = stats?.AverageAge ?? 0,
                        PercentageOfPeopleWhoCompletedThePlan = stats?.PercentageOfPeopleWhoCompletedThePlan ?? 0,
                        ManGender = stats?.ManGender ?? 0,
                        WomanGender = stats?.WomanGender ?? 0
                    };
                }).ToList();

                var totalCount = allResults.Count;

                var pagedData = allResults
                    .Skip((request.Input.PageNumber.Value - 1) * request.Input.Count.Value)
                    .Take(request.Input.Count.Value)
                    .ToList();

                var result = new GlobalGridResult<GetReportPlanResponseDto>
                {
                    TotalCount = totalCount,
                    Data = pagedData
                };

                return ServiceResult.Ok(result);
            }
            catch (Exception ex)
            {
                return ServiceResult.BadRequest<GlobalGridResult<GetReportPlanResponseDto>>(ex.Message);
            }

        }
    }
}
