using Application.Cqrs.Queris;
using Application.Users.DTOs;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Users.Queries
{
    public record GetMyBadgesQuery(long UserId) : IQuery<ServiceResult<List<MyBadgeDTO>>>;

    public class GetMyBadgesQueryHandler(
    IRepository<UserBadge> badgeRepo,
    IRepository<User> userRepo
) : IQueryHandler<GetMyBadgesQuery, ServiceResult<List<MyBadgeDTO>>>
    {
        public async Task<ServiceResult<List<MyBadgeDTO>>> Handle(GetMyBadgesQuery request, CancellationToken cancellationToken)
        {
            var similarUserIds = await badgeRepo.TableNoTracking
                .Where(b => b.UserId == request.UserId)
                .Select(b => b.SimilarUserId)
                .ToListAsync(cancellationToken);

            var userMap = await userRepo.TableNoTracking
                .Where(u => similarUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, cancellationToken);

            var badges = await badgeRepo.TableNoTracking
                .Where(b => b.UserId == request.UserId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync(cancellationToken);

            var result = badges.Select(b =>
            {
                var (title, icon) = UserBadge.GetBadgeMeta(b.BadgeType);
                return new MyBadgeDTO
                {
                    BadgeTitle = title,
                    BadgeIcon = icon,
                    SimilarityPercent = Math.Round(b.SimilarityPercent, 1),
                    SimilarUserName = userMap.TryGetValue(b.SimilarUserId, out var similarUser)
                        ? similarUser.FullName
                        : "ناشناس",
                    CreatedAt = b.CreatedAt
                };
            }).ToList();

            return ServiceResult.Ok(result);
        }


    }
}
