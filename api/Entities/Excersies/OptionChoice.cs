using Entities.Common;
using Entities.StepOprions;
using Entities.Users;

namespace Entities.Excersies
{
    public class OptionChoice : BaseEntity<long>
    {
        public long StepOptionId { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public int Order { get; set; }
        public string Feedback { get; set; } // بازخورد هنگام انتخاب این گزینه
        public MultipleChoiceStepOption StepOption { get; set; }

        public ICollection<UserSelectedChoice> UserSelectedChoices { get; set; }
    }

}
