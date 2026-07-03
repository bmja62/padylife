using Entities.Common.Events;

namespace Application.Cqrs.Events.Dispatcher
{
    public class DomainEventActionDispatcher<TEvent> : IDomainEventHandler<TEvent>
      where TEvent : IDomainEvent
    {
        private readonly IEnumerable<IDomainEventAction<TEvent>> _actions;

        public DomainEventActionDispatcher(IEnumerable<IDomainEventAction<TEvent>> actions)
        {
            _actions = actions;
        }

        public async Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken)
        {
            foreach (var action in _actions)
            {
                await action.ExecuteAsync(domainEvent, cancellationToken);
            }
        }
    }
}
