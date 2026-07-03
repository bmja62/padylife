using Application.Cqrs.Commands;
using Application.Points.Commands;
using Application.Points.Extentions;
using Data.Contracts;
using Entities.Common;
using Entities.Common.Events;
using Entities.Plans;
using Entities.Users;

namespace Application.Points.Events
{
    public class IncreaseUserPointByUserIdEvent(long userId, int amount, long entityRefrenceId, EntityType entityType) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public long UserId { get; } = userId;
        public int Amount { get; } = amount;
        public long EntityRefrenceId { get; } = entityRefrenceId;
        public EntityType EntityType { get; } = entityType;
    }

    public class IncreaseUserPointForUserStepAnswerAction
      (
      IRepository<Plan> planRepository,
      ICommandDispatcher commandDispatcher,
      IRepository<UserPlan> userPlanRepository
      ) : IDomainEventAction<IncreaseUserPointByUserIdEvent>
    {
        public async Task ExecuteAsync(IncreaseUserPointByUserIdEvent @event, CancellationToken cancellationToken)
        {
            await commandDispatcher
                .SendAsync(new EarnPointsCommand(
                    @event.UserId,
                    @event.Amount,
                    PointMessages.GetIncresePointMessgeForAnsweringUserStepAnswer(@event.EntityRefrenceId),
                    @event.EntityRefrenceId,
                    @event.EntityType
                    )
                );


        }
    }
}
