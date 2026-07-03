using Application.Cqrs.Queris;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public record GetLeaderboardQuery(int TopCount = 10)
        : IQuery<ServiceResult<LeaderboardReportDTO>>;

    public class GetLeaderboardQueryHandler(
        IRepository<User> userRepo,
        IRepository<PointTransaction> pointRepo,
        IRepository<UserPlanAnswer> answerRepo
    ) : IQueryHandler<GetLeaderboardQuery, ServiceResult<LeaderboardReportDTO>>
    {
        public async Task<ServiceResult<LeaderboardReportDTO>> Handle(
            GetLeaderboardQuery request,
            CancellationToken cancellationToken)
        {
            // محاسبه امتیازات کاربران
            var userPoints = await pointRepo.TableNoTracking
                .Where(p => p.TransactionType == PointTransactionType.Earn && !p.IsReverted)
                .GroupBy(p => p.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    TotalPoints = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.TotalPoints)
                .Take(request.TopCount)
                .ToListAsync(cancellationToken);

            var userIds = userPoints.Select(x => x.UserId).ToList();

            // دریافت اطلاعات کاربران
            var users = await userRepo.TableNoTracking
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, cancellationToken);

            // محاسبه تعداد پاسخ‌ها برای هر کاربر
            var userAnswers = await answerRepo.TableNoTracking
                .Where(a => userIds.Contains(a.UserPlan.UserId))
                .GroupBy(a => a.UserPlan.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    AnswerCount = g.Count()
                })
                .ToListAsync(cancellationToken);

            var answerMap = userAnswers.ToDictionary(x => x.UserId, x => x.AnswerCount);

            // ساخت لیست رهبران
            var leaders = new List<LeaderboardUserDTO>();
            int rank = 1;

            foreach (var point in userPoints)
            {
                users.TryGetValue(point.UserId, out var user);
                answerMap.TryGetValue(point.UserId, out var answers);

                leaders.Add(new LeaderboardUserDTO
                {
                    Rank = rank++,
                    UserId = point.UserId,
                    UserName = user?.FullName ?? "نامشخص",
                    ProfileImage = user?.ProfileImage,
                    TotalPoints = point.TotalPoints,
                    AnswerCount = answers,
                    Badges = CalculateBadges(point.TotalPoints)
                });
            }

            var result = new LeaderboardReportDTO
            {
                Leaders = leaders,
                GeneratedAt = DateTime.UtcNow
            };

            return ServiceResult.Ok(result);
        }

        private List<string> CalculateBadges(int points)
        {
            var badges = new List<string>();

            if (points >= 1000) badges.Add("طلایی");
            else if (points >= 500) badges.Add("نقره‌ای");
            else if (points >= 100) badges.Add("برنزی");

            if (points >= 200) badges.Add("فعال");
            if (points >= 50) badges.Add("تازه‌کار");

            return badges;
        }
    }

    public class LeaderboardReportDTO
    {
        public List<LeaderboardUserDTO> Leaders { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
    }

    public class LeaderboardUserDTO
    {
        public int Rank { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string ProfileImage { get; set; }
        public int TotalPoints { get; set; }
        public int AnswerCount { get; set; }
        public List<string> Badges { get; set; } = new();
    }
}