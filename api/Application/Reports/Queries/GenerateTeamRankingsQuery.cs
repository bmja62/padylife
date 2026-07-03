using Application.Cqrs.Queris;
using Application.Reports.DTOs;
using Data.Contracts;
using Entities.Plans;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Reports.Queries
{
    public record GenerateTeamRankingsQuery
        : IQuery<ServiceResult<List<TeamRankDTO>>>;

    public class GenerateTeamRankingsHandler(
        IRepository<UserPlanAnswer> answerRepo,
        IRepository<PointTransaction> pointRepo,
        IRepository<Plan> planRepo,
        IRepository<UserBadge> badgeRepo
    ) : IQueryHandler<GenerateTeamRankingsQuery, ServiceResult<List<TeamRankDTO>>>
    {
        public async Task<ServiceResult<List<TeamRankDTO>>> Handle(GenerateTeamRankingsQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow.Date;
            var fromDate = now.AddDays(-30); // ماه اخیر

            // مرحله 1: گروه‌بندی بر اساس Plan
            var planAnswers = await answerRepo.TableNoTracking
                .Include(t => t.PlanQuestion)
                .Include(t => t.UserPlan)
                .Where(a => a.CreatedAt.Date >= fromDate)
                .ToListAsync(cancellationToken);

            var plans = await planRepo.TableNoTracking
                .ToDictionaryAsync(p => p.Id, cancellationToken);

            var grouped = planAnswers
                .GroupBy(a => a.PlanQuestion.PlanId)
                .ToList();

            var allUserIds = grouped.SelectMany(g => g.Select(a => a.UserPlan.UserId)).Distinct().ToList();

            var points = await pointRepo.TableNoTracking
                .Where(p => allUserIds.Contains(p.UserId) && !p.IsReverted && p.TransactionType == PointTransactionType.Earn && p.TransactionDate >= fromDate)
                .ToListAsync(cancellationToken);

            // مرحله 2: ساخت لیست رتبه‌ها
            var teams = new List<TeamRankDTO>();

            foreach (var group in grouped)
            {
                var planId = group.Key;
                var memberIds = group.Select(a => a.UserPlan.UserId).Distinct().ToList();

                var planPoints = points
                    .Where(p => memberIds.Contains(p.UserId))
                    .Sum(p => p.Amount);

                teams.Add(new TeamRankDTO
                {
                    TeamName = plans.TryGetValue(planId, out var p) ? p.Title : $"پلن {planId}",
                    TotalAnswers = group.Count(),
                    TotalPoints = planPoints,
                    MemberUserIds = memberIds
                });
            }

            // مرحله 3: رتبه‌بندی و علامت‌گذاری تیم برتر
            var ordered = teams
                .OrderByDescending(t => t.TotalPoints)
                .ThenByDescending(t => t.TotalAnswers)
                .ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].Rank = i + 1;
                ordered[i].IsTopTeam = i == 0;
            }

            // مرحله 4: اعطای نشان تیم برتر
            var topTeam = ordered.FirstOrDefault(t => t.IsTopTeam);
            if (topTeam is not null)
            {
                foreach (var userId in topTeam.MemberUserIds)
                {
                    foreach (var similarUserId in topTeam.MemberUserIds)
                    {
                        if (userId != similarUserId) // جلوگیری از اضافه کردن نشان برای خود کاربر
                        {


                            var badgeType = SimilarityBadgeType.TeamChampion;

                            // بررسی می‌کنیم که آیا نشان برای این ترکیب از کاربر و مشابه ثبت شده است یا نه
                            var exists = await badgeRepo.TableNoTracking
                                .AnyAsync(b => b.UserId == userId && b.SimilarUserId == similarUserId && b.BadgeType == badgeType, cancellationToken);

                            if (!exists)
                            {
                                // ایجاد نشان برای کاربر اصلی (UserId) به همراه کاربر مشابه (SimilarUserId)
                                var badgeForUser = new UserBadge
                                {
                                    UserId = userId, // کاربر اصلی
                                    SimilarUserId = similarUserId, // کاربر مشابه
                                    BadgeType = badgeType,
                                    SimilarityPercent = 100,
                                    CreatedAt = DateTime.UtcNow
                                };

                                await badgeRepo.AddAsync(badgeForUser, cancellationToken);

                                // ایجاد نشان برای کاربر مشابه (SimilarUserId) به همراه کاربر اصلی (UserId)
                                var badgeForSimilarUser = new UserBadge
                                {
                                    UserId = similarUserId, // کاربر مشابه
                                    SimilarUserId = userId, // کاربر اصلی
                                    BadgeType = badgeType,
                                    SimilarityPercent = 100,
                                    CreatedAt = DateTime.UtcNow
                                };

                                await badgeRepo.AddAsync(badgeForSimilarUser, cancellationToken);
                            }
                        }
                    }
                }
            }

            return ServiceResult.Ok(ordered);
        }
    }
}
