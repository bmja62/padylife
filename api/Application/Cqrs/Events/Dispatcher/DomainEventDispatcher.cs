namespace Application.Cqrs.Events.Dispatcher
{
    using Common;
    using Entities.Common.Events;
    using Microsoft.Extensions.DependencyInjection;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    namespace Application.Cqrs.Events.Dispatcher
    {
        public class DomainEventDispatcher : IDomainEventDispatcher, IScopedDependency
        {
            private readonly IServiceProvider _serviceProvider;

            public DomainEventDispatcher(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken)
            {
                foreach (var domainEvent in domainEvents)
                {
                    var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
                    var handlers = (IEnumerable<object>)_serviceProvider.GetServices(handlerType);

                    foreach (var handler in handlers)
                    {
                        await ((dynamic)handler).HandleAsync((dynamic)domainEvent, cancellationToken);
                    }
                }
            }
        }
    }
}
