using Application.Cqrs.Commands;
using Application.Points.Events;
using Data.Contracts;
using Entities.Common;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Points.Commands
{
    public class EarnPointsCommand(long userId, int amount, string reason, long? referenceId = null, EntityType referenceType = default) : ICommand<ServiceResult>
    {
        public long UserId { get; } = userId;
        public int Amount { get; } = amount;
        public string Reason { get; } = reason;
        public long? ReferenceId { get; } = referenceId;
        public EntityType ReferenceType { get; } = referenceType;
    }

    public class EarnPointsCommandHandler(IRepository<UserPoints> userPointsRepository)
        : ICommandHandler<EarnPointsCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(EarnPointsCommand request, CancellationToken cancellationToken)
        {
            var userPoints = await userPointsRepository.Table
                .FirstOrDefaultAsync(up => up.UserId == request.UserId, cancellationToken);

            if (userPoints == null)
            {
                userPoints = new UserPoints(request.UserId);
                await userPointsRepository.AddAsync(userPoints, cancellationToken);
            }

            userPoints.EarnPoints(request.Amount, request.Reason, request.ReferenceId, request.ReferenceType);
            userPoints.AddDomainEvent(new PointsEarnedEvent(request.UserId, request.Amount, request.Reason, request.ReferenceId));

            await userPointsRepository.UpdateAsync(userPoints, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
