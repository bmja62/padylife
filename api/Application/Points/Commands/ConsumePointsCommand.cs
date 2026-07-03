using Application.Cqrs.Commands;
using Application.Points.Events;
using Data.Contracts;
using Entities.Common;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Points.Commands
{
    public class ConsumePointsCommand(long userId, int amount, string reason, long? referenceId = null, Entities.Common.EntityType referenceType = default) : ICommand<ServiceResult>
    {
        public long UserId { get; } = userId;
        public int Amount { get; } = amount;
        public string Reason { get; } = reason;
        public long? ReferenceId { get; } = referenceId;
        public EntityType ReferenceType { get; } = referenceType;
    }

    public class ConsumePointsCommandHandler(IRepository<UserPoints> userPointsRepository)
        : ICommandHandler<ConsumePointsCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(ConsumePointsCommand request, CancellationToken cancellationToken)
        {
            var userPoints = await userPointsRepository.Table.Include(t => t.PointTransactions)
                .FirstOrDefaultAsync(up => up.UserId == request.UserId, cancellationToken);

            if (userPoints == null)
                return ServiceResult.BadRequest("User has no points");

            userPoints.ConsumePoints(request.Amount, request.Reason, request.ReferenceId, request.ReferenceType);
            userPoints.AddDomainEvent(new PointsConsumedEvent(request.UserId, request.Amount, request.Reason, request.ReferenceId));

            await userPointsRepository.UpdateAsync(userPoints, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
