using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public record GetPlanLeaderboardQuery(GetPlanLeaderboardQueryDTO Input)
        : IQuery<ServiceResult<List<PlanLeaderboardUserDTO>>>;
    public class GetPlanLeaderboardQueryHandler(
    IRepository<UserPlanAnswer> answerRepo,
    IRepository<PointTransaction> pointTransactionRepo,
    IRepository<User> userRepo
) : IQueryHandler<GetPlanLeaderboardQuery, ServiceResult<List<PlanLeaderboardUserDTO>>>
    {
        public async Task<ServiceResult<List<PlanLeaderboardUserDTO>>> Handle(GetPlanLeaderboardQuery request, CancellationToken cancellationToken)
        {
            var planId = request.Input.PlanId;

            // گرفتن کاربرها و تعداد پاسخ‌ها در این پلن
            var answerStats = await answerRepo.TableNoTracking
                .Where(a => a.PlanQuestion.PlanId == planId)
                .GroupBy(a => a.UserPlan.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    AnswerCount = g.Count()
                })
                .ToListAsync(cancellationToken);

            var userIds = answerStats.Select(a => a.UserId).ToList();

            // گرفتن مجموع امتیازات
            var points = await pointTransactionRepo.TableNoTracking
                .Where(pt => pt.TransactionType == PointTransactionType.Earn && !pt.IsReverted && userIds.Contains(pt.UserId))
                .GroupBy(pt => pt.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    TotalPoints = g.Sum(x => x.Amount)
                })
                .ToListAsync(cancellationToken);

            var pointMap = points.ToDictionary(p => p.UserId, p => p.TotalPoints);

            var users = await userRepo.TableNoTracking
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, cancellationToken);

            var leaderboard = answerStats
                .Select(a => new PlanLeaderboardUserDTO
                {
                    UserId = a.UserId,
                    FullName = users.TryGetValue(a.UserId, out var u) ? u.FullName : "نامشخص",
                    ProfileImage = users.TryGetValue(a.UserId, out var up) ? up.ProfileImage: "",
                    AnswerCount = a.AnswerCount,
                    TotalPoints = pointMap.TryGetValue(a.UserId, out var p) ? p : 0
                })
                .OrderByDescending(x => x.TotalPoints)
                .ThenByDescending(x => x.AnswerCount)
                .ToList();

            for (int i = 0; i < leaderboard.Count; i++)
            {
                leaderboard[i].Rank = i + 1;
            }

            return ServiceResult.Ok(leaderboard.Take(request.Input.TopCount).ToList());
        }
    }

}
