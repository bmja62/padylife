namespace Entities.Common.Events
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
