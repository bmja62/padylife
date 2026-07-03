using Common.Exceptions;
using Data.Contracts;
using Entities.Common.Events;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services.Services.CommentServices.DTOs;
using Services.Services.RateServices;


namespace Application.Rates.Events
{
    public class RateAddedEvent(long userId, CreateRateDTO input) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public long UserId { get; } = userId;
        public CreateRateDTO Input { get; } = input;
    }
    public class SpecialistRateValidationAction(IRepository<UserPlan> userPlanRepository) : IDomainEventAction<RateAddedEvent>
    {
        public async Task ExecuteAsync(RateAddedEvent @event, CancellationToken cancellationToken)
        {
            if (@event.Input.EntityType == Entities.Common.EntityType.Specialist)
            {
                var hasPermission = await userPlanRepository.TableNoTracking
                    .AnyAsync(a => a.UserId == @event.UserId && a.Experts.Any(e => e.ExpertId == @event.Input.EntityId), cancellationToken);

                if (!hasPermission)
                    throw new LackAccessUnconnectedPeopleException("شما دوره‌ای با متخصص ذکر شده ندارید");
            }
        }
    }
    public class AddRateAction(IRateService rateService) : IDomainEventAction<RateAddedEvent>
    {
        public async Task ExecuteAsync(RateAddedEvent @event, CancellationToken cancellationToken)
        {
            await rateService.AddRateAsync(@event.UserId, @event.Input);
        }
    }
}
