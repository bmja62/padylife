using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Data.Contracts;
using Entities.Plans;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public record GetPlanLeaderboardStatsQuery(long PlanId, long CurrentUserId)
        : IQuery<ServiceResult<PlanLeaderboardStatsDTO>>;

    public class GetPlanLeaderboardStatsQueryHandler(
    IRepository<UserPlanAnswer> answerRepo,
    IRepository<PointTransaction> pointRepo,
    IRepository<PlanQuestion> questionRepo,
    IRepository<User> userRepo
) : IQueryHandler<GetPlanLeaderboardStatsQuery, ServiceResult<PlanLeaderboardStatsDTO>>
    {
        public async Task<ServiceResult<PlanLeaderboardStatsDTO>> Handle(GetPlanLeaderboardStatsQuery request, CancellationToken cancellationToken)
        {
            var totalQuestions = await questionRepo.TableNoTracking
                .Where(q => q.PlanId == request.PlanId)
                .CountAsync(cancellationToken);

            var answers = await answerRepo.TableNoTracking.Include(t => t.UserPlan)
                .Where(a => a.PlanQuestion.PlanId == request.PlanId)
                .ToListAsync(cancellationToken);

            var answerGroups = answers
                .GroupBy(a => a.UserPlan.UserId)
                .Select(g => new { UserId = g.Key, AnswerCount = g.Count() })
                .ToList();

            var userIds = answerGroups.Select(g => g.UserId).ToList();

            var points = await pointRepo.TableNoTracking
                .Where(pt => pt.TransactionType == PointTransactionType.Earn && !pt.IsReverted && userIds.Contains(pt.UserId))
                .GroupBy(pt => pt.UserId)
                .Select(g => new { UserId = g.Key, TotalPoints = g.Sum(x => x.Amount) })
                .ToDictionaryAsync(x => x.UserId, x => x.TotalPoints, cancellationToken);

            var users = await userRepo.TableNoTracking
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, cancellationToken);

            var leaderboard = answerGroups.Select(g =>
            {
                var pointsEarned = points.TryGetValue(g.UserId, out var pt) ? pt : 0;
                var participation = totalQuestions > 0 ? (g.AnswerCount * 100.0) / totalQuestions : 0;

                return new PlanLeaderboardUserStatsDTO
                {
                    UserId = g.UserId,
                    FullName = users[g.UserId].FullName,
                    AnswerCount = g.AnswerCount,
                    TotalPoints = pointsEarned,
                    ParticipationPercent = Math.Round(participation, 1)
                };
            }).OrderByDescending(x => x.TotalPoints)
              .ThenByDescending(x => x.AnswerCount)
              .ToList();

            for (int i = 0; i < leaderboard.Count; i++)
                leaderboard[i].Rank = i + 1;

            var currentUserStats = leaderboard.FirstOrDefault(x => x.UserId == request.CurrentUserId);

            return ServiceResult.Ok(new PlanLeaderboardStatsDTO
            {
                TotalUsers = leaderboard.Count,
                TotalQuestions = totalQuestions,
                TopUsers = leaderboard.Take(10).ToList(),
                CurrentUser = currentUserStats
            });
        }
    }

}
