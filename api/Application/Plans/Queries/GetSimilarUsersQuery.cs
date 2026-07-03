using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Services.UserServices;

namespace Application.Plans.Queries
{
    public record GetSimilarUsersQuery(long UserId) : IQuery<ServiceResult<List<SimilarUserDTO>>>;

    public class GetSimilarUsersQueryHandler(
  IRepository<User> userRepo,
  IUserSimilarityService similarityService
) : IQueryHandler<GetSimilarUsersQuery, ServiceResult<List<SimilarUserDTO>>>
    {
        public async Task<ServiceResult<List<SimilarUserDTO>>> Handle(GetSimilarUsersQuery request, CancellationToken cancellationToken)
        {
            var userId = request.UserId;

            var similarityResults = await similarityService.CalculateSimilarityAsync(userId, 10, cancellationToken);

            if (!similarityResults.Any())
                return ServiceResult.Ok(new List<SimilarUserDTO>());

            var userIds = similarityResults.Select(x => x.UserId).ToList();

            var users = await userRepo.TableNoTracking
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, cancellationToken);

            var result = similarityResults.Select(item =>
            {
                var (title, icon) = UserBadge.GetBadgeMeta(UserBadge.GetBadgeType(item.Similarity));
                return new SimilarUserDTO
                {
                    UserId = item.UserId,
                    FullName = users.TryGetValue(item.UserId, out var u) ? !string.IsNullOrEmpty(u.FullName) ? u.FullName : u.UserName : "ناشناس",
                    SimilarityPercent = item.Similarity,
                    BadgeTitle = title,
                    BadgeIcon = icon
                };
            }).ToList();

            return ServiceResult.Ok(result);
        }
    }
}


