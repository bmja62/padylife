using Entities.Common;
using Entities.Common.Events;
using Entities.Medals.Events;

namespace Entities.Medals
{
    public class MedalCondition : BaseEntity<long>
    {
        public long MedalId { get; private set; }
        public Medal Medal { get; private set; }

        public string ConditionType { get; private set; } // مثل "CompletedPlans"
        public string Operator { get; private set; }      // مثل ">="
        public string Value { get; private set; }         // مثل "5"

        public MedalCondition(long medalId, string conditionType, string @operator, string value)
        {
            MedalId = medalId;
            ConditionType = conditionType;
            Operator = @operator;
            Value = value;
        }

        public void UpdateCondition(string conditionType, string @operator, string value)
        {
            ConditionType = conditionType;
            Operator = @operator;
            Value = value;

            AddDomainEvent(new MedalConditionUpdatedEvent(this));
        }

        // نگهداری رویدادهای دامنه
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
        protected void AddDomainEvent(IDomainEvent eventItem)
        {
            _domainEvents.Add(eventItem);
        }
    }
}
