using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Users.Commands
{
    public record RegisterSimilarUserCommand(long UserId, long SimilarUserId, double SimilarityPercent)
        : ICommand<ServiceResult>;

    public class RegisterSimilarUserCommandHandler(
    IRepository<UserBadge> badgeRepo
) : ICommandHandler<RegisterSimilarUserCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(RegisterSimilarUserCommand request, CancellationToken cancellationToken)
        {
            var badgeType = UserBadge.GetBadgeType(request.SimilarityPercent);
            if (badgeType == null)
                return ServiceResult.Fail("مقدار شباهت برای دریافت نشان کافی نیست.");

            var exists = await badgeRepo.TableNoTracking
                .AnyAsync(b => b.UserId == request.UserId && b.SimilarUserId == request.SimilarUserId && b.BadgeType == badgeType, cancellationToken);

            if (exists)
                return ServiceResult.Ok("Badge قبلاً ثبت شده است.");

            var badge = new UserBadge
            {
                UserId = request.UserId,
                SimilarUserId = request.SimilarUserId,
                SimilarityPercent = request.SimilarityPercent,
                BadgeType = badgeType.Value,
                CreatedAt = DateTime.UtcNow
            };

            await badgeRepo.AddAsync(badge, cancellationToken);
            return ServiceResult.Ok("Badge با موفقیت ثبت شد.");
        }
    }


}
