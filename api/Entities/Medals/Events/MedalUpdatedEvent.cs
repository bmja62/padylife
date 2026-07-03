using Entities.Common.Events;

namespace Entities.Medals.Events
{
    public class MedalUpdatedEvent : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
        public Medal Medal { get; }

        public MedalUpdatedEvent(Medal medal) => Medal = medal;
    }

    public class MedalConditionAddedEvent : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
        public Medal Medal { get; }
        public MedalCondition Condition { get; }
        public MedalConditionAddedEvent(Medal medal, MedalCondition condition)
        {
            Medal = medal;
            Condition = condition;
        }
    }

    public class MedalConditionRemovedEvent : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
        public Medal Medal { get; }
        public MedalCondition Condition { get; }
        public MedalConditionRemovedEvent(Medal medal, MedalCondition condition)
        {
            Medal = medal;
            Condition = condition;
        }
    }

    public class MedalConditionUpdatedEvent : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
        public MedalCondition Condition { get; }
        public MedalConditionUpdatedEvent(MedalCondition condition) => Condition = condition;
    }
}
