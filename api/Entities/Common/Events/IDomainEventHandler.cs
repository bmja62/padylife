namespace Entities.Common.Events
{
    /// <summary>
    /// اگر ایونت یک کار ساده انجام میداد از این استفاده میکنیم - مثال ExpireBasketsEventHandler
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
    }
}
