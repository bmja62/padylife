using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Data.Contracts;
using Entities.Plans;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public record GetAllMyPlanLeaderboardsQuery(long UserId)
        : IQuery<ServiceResult<List<UserPlanLeaderboardOverviewDTO>>>;
    public class GetAllMyPlanLeaderboardsQueryHandler(
    IRepository<UserPlanAnswer> answerRepo,
    IRepository<PointTransaction> pointRepo,
    IRepository<PlanQuestion> planQuestionRepo,
    IRepository<Plan> planRepo,
    IRepository<User> userRepo
) : IQueryHandler<GetAllMyPlanLeaderboardsQuery, ServiceResult<List<UserPlanLeaderboardOverviewDTO>>>
    {
        public async Task<ServiceResult<List<UserPlanLeaderboardOverviewDTO>>> Handle(GetAllMyPlanLeaderboardsQuery request, CancellationToken cancellationToken)
        {
            var userId = request.UserId;

            // گرفتن همه PlanIdهایی که این کاربر در آن‌ها پاسخ داده
            var myPlanIds = await answerRepo.TableNoTracking
                .Where(a => a.UserPlan.UserId == userId)
                .Select(a => a.PlanQuestion.PlanId)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (!myPlanIds.Any())
                return ServiceResult.Ok(new List<UserPlanLeaderboardOverviewDTO>());

            var plans = await planRepo.TableNoTracking
                .Where(p => myPlanIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, cancellationToken);

            var planQuestionCounts = await planQuestionRepo.TableNoTracking
                .Where(pq => myPlanIds.Contains(pq.PlanId))
                .GroupBy(pq => pq.PlanId)
                .Select(g => new { PlanId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.PlanId, x => x.Count, cancellationToken);

            var answers = await answerRepo.TableNoTracking
                .Where(a => myPlanIds.Contains(a.PlanQuestion.PlanId))
                .ToListAsync(cancellationToken);

            var points = await pointRepo.TableNoTracking
                .Where(p => p.TransactionType == PointTransactionType.Earn && !p.IsReverted)
                .ToListAsync(cancellationToken);

            var users = await userRepo.TableNoTracking.ToDictionaryAsync(u => u.Id, cancellationToken);

            // خروجی نهایی برای همه پلن‌ها
            var result = new List<UserPlanLeaderboardOverviewDTO>();

            foreach (var planId in myPlanIds)
            {
                var planAnswers = answers.Where(a => a.PlanQuestion.PlanId == planId).ToList();
                var questionCount = planQuestionCounts.GetValueOrDefault(planId, 0);

                var groupedAnswers = planAnswers
                    .GroupBy(a => a.UserPlan.UserId)
                    .Select(g => new
                    {
                        UserId = g.Key,
                        AnswerCount = g.Count()
                    }).ToList();

                var leaderboard = groupedAnswers
                    .Select(g =>
                    {
                        var userPoints = points
                            .Where(p => p.UserId == g.UserId)
                            .Sum(p => p.Amount);

                        var participation = questionCount > 0
                            ? Math.Round((double)g.AnswerCount * 100 / questionCount, 1)
                            : 0;

                        return new PlanLeaderboardUserStatsDTO
                        {
                            UserId = g.UserId,
                            FullName = users.TryGetValue(g.UserId, out var u) ? u.FullName : "نامشخص",
                            AnswerCount = g.AnswerCount,
                            TotalPoints = userPoints,
                            ParticipationPercent = participation
                        };
                    })
                    .OrderByDescending(x => x.TotalPoints)
                    .ThenByDescending(x => x.AnswerCount)
                    .ToList();

                for (int i = 0; i < leaderboard.Count; i++)
                    leaderboard[i].Rank = i + 1;

                var currentUser = leaderboard.FirstOrDefault(x => x.UserId == userId);

                result.Add(new UserPlanLeaderboardOverviewDTO
                {
                    PlanId = planId,
                    PlanTitle = plans.TryGetValue(planId, out var plan) ? plan.Title : "نامشخص",
                    TotalQuestions = questionCount,
                    TotalUsers = leaderboard.Count,
                    TopUsers = leaderboard.Take(5).ToList(),
                    CurrentUser = currentUser
                });
            }

            return ServiceResult.Ok(result);
        }
    }

}
