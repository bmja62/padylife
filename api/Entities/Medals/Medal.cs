using Entities.Common;
using Entities.Medals.Events;

namespace Entities.Medals
{
    public class Medal : BaseEntity<long>
    {
        private readonly List<MedalCondition> _conditions = new();
        private readonly List<UserMedal> _userMedals = new();
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string IconUrl { get; private set; }

        // فقط لیست خواندنی در دسترس باشد
        public IReadOnlyCollection<MedalCondition> Conditions => _conditions.AsReadOnly();
        public IReadOnlyCollection<UserMedal> UserMedals => _userMedals.AsReadOnly();



        public Medal(string title, string description, string iconUrl)
        {
            Title = title;
            Description = description;
            IconUrl = iconUrl;
        }

        public void UpdateDetails(string title, string description, string iconUrl)
        {
            Title = title;
            Description = description;
            IconUrl = iconUrl;

            // رویداد تغییرات مدال
            AddDomainEvent(new MedalUpdatedEvent(this));
        }

        public void AddCondition(string conditionType, string @operator, string value)
        {
            var condition = new MedalCondition(this.Id, conditionType, @operator, value);
            _conditions.Add(condition);

            AddDomainEvent(new MedalConditionAddedEvent(this, condition));
        }

        public void RemoveCondition(long conditionId)
        {
            var condition = _conditions.FirstOrDefault(c => c.Id == conditionId);
            if (condition == null) return;

            _conditions.Remove(condition);
            AddDomainEvent(new MedalConditionRemovedEvent(this, condition));
        }
    }

}
