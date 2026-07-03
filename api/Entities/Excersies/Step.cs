using Entities.Common;
using Entities.StepOprions;
using Entities.Users;

namespace Entities.Excersies
{
    public class Step : BaseEntity<long>
    {
        //Props
        public string Name { get; set; }
        public ICollection<ExerciseStep> ExerciseSteps { get; set; }
        public ICollection<StepOption> StepOptions { get; set; }
        public ICollection<UserPlanExcersiesAnswer> UserPlanExcersiesAnswers { get; set; }

        public long? CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }

    }

}
