namespace Entities.Common.Events
{
    /// <summary>
    /// از این در مواقعی استفاده میکنیم که ایونت مورد نظر چند اکشن متفاوت را باید انجام دهد  - مثال NewOrderEvent
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IDomainEventAction<in TEvent> where TEvent : IDomainEvent
    {
        Task ExecuteAsync(TEvent @event, CancellationToken cancellationToken);
    }
}
