using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Data.Contracts;
using Entities.Common;
using Entities.Plans;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public record GetUserPlanStatsChartQuery(long UserId)
        : IQuery<ServiceResult<List<UserPlanStatsChartDTO>>>;
    public class GetUserPlanStatsChartQueryHandler(
    IRepository<UserPlanAnswer> answerRepo,
    IRepository<PointTransaction> pointRepo,
    IRepository<PlanQuestion> questionRepo,
    IRepository<Plan> planRepo
) : IQueryHandler<GetUserPlanStatsChartQuery, ServiceResult<List<UserPlanStatsChartDTO>>>
    {
        public async Task<ServiceResult<List<UserPlanStatsChartDTO>>> Handle(GetUserPlanStatsChartQuery request, CancellationToken cancellationToken)
        {
            var userId = request.UserId;

            // پلن‌هایی که کاربر پاسخ داده
            var planIds = await answerRepo.TableNoTracking
                .Where(a => a.UserPlan.UserId == userId)
                .Select(a => a.PlanQuestion.PlanId)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (!planIds.Any())
                return ServiceResult.Ok(new List<UserPlanStatsChartDTO>());

            var plans = await planRepo.TableNoTracking
                .Where(p => planIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, cancellationToken);

            var totalQuestions = await questionRepo.TableNoTracking
                .Where(q => planIds.Contains(q.PlanId))
                .GroupBy(q => q.PlanId)
                .Select(g => new { PlanId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.PlanId, x => x.Count, cancellationToken);

            var answerCounts = await answerRepo.TableNoTracking
                .Where(a => planIds.Contains(a.PlanQuestion.PlanId) && a.UserPlan.UserId == userId)
                .GroupBy(a => a.PlanQuestion.PlanId)
                .Select(g => new { PlanId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.PlanId, x => x.Count, cancellationToken);

            var points = await pointRepo.TableNoTracking
                .Where(p => p.TransactionType == PointTransactionType.Earn && !p.IsReverted && p.UserId == userId)
                .ToListAsync(cancellationToken);

            var pointGroups = points
                .GroupBy(p => p.ReferenceId) // assuming ReferenceId points to Plan or PlanQuestion
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

            var result = planIds.Select(planId =>
            {
                totalQuestions.TryGetValue(planId, out var questionCount);
                answerCounts.TryGetValue(planId, out var answerCount);
                var totalPoints = points
                    .Where(p => p.ReferenceEntityType == EntityType.Plan && p.ReferenceId == planId)
                    .Sum(p => p.Amount);

                return new UserPlanStatsChartDTO
                {
                    PlanId = planId,
                    PlanTitle = plans.TryGetValue(planId, out var plan) ? plan.Title : "ناشناخته",
                    TotalQuestions = questionCount,
                    AnswerCount = answerCount,
                    TotalPoints = totalPoints
                };
            }).ToList();

            return ServiceResult.Ok(result);
        }
    }

}
