using Entities.Common;
using Entities.Excersies;
using Entities.StepOprions;

namespace Entities.Users
{
    public class UserPlanExcersiesAnswer : BaseEntity<long>
    {
        public long UserPlanId { get; set; }
        public long ExcersieId { get; set; }
        public long StepId { get; set; }

        // پاسخ تکی (برای غیر چند گزینه‌ای)
        public long? SelectedStepOptionId { get; set; }

        // پاسخ‌های چند گزینه‌ای
        public virtual ICollection<UserSelectedChoice> SelectedChoices { get; set; }

        public string Text { get; set; }
        public string ImageUrl { get; set; }

        public UserPlan UserPlan { get; set; }
        public Exercise Exercise { get; set; }
        public Step Step { get; set; }
        public StepOption SelectedStepOption { get; set; }
    }

    public class UserSelectedChoice : IEntity
    {
        public long UserPlanExcersiesAnswerId { get; set; }
        public long OptionChoiceId { get; set; }

        public virtual UserPlanExcersiesAnswer Answer { get; set; }
        public virtual OptionChoice Choice { get; set; }
    }
}
