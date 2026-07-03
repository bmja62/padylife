using Entities.Common;

namespace Entities.Excersies
{
    public class ExerciseStep : IEntity
    {
        public ExerciseStep(long stepId)
        {
            StepId = stepId;
        }

        private ExerciseStep()
        {

        }

        //FKs
        public long StepId { get; set; }
        public long ExerciseId { get; set; }

        //Navigations
        public Step Step { get; set; }
        public Exercise Exercise { get; set; }

        public static ExerciseStep CreateByStepId(long stepId) => new(stepId);
    }
}
