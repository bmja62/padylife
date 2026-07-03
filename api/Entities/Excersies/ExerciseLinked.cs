using Entities.Common;
using Entities.Questions;

namespace Entities.Excersies
{
    public class ExerciseLinked : IEntity
    {
        public ExerciseLinked(long exerciseId, int? priority = null)
        {
            ExerciseId = exerciseId;
            Priority = priority;
        }

        private ExerciseLinked() { }

        //PKs
        public long Id { get; set; }

        //FKs
        public long QuestionLinkedId { get; set; }
        public long ExerciseId { get; set; }

        //Props
        public int? Priority { get; set; } // اولویت نمایش تمرین

        public QuestionLinked QuestionLinked { get; set; }
        public Exercise Exercise { get; set; }

        public static ExerciseLinked Create(long exerciseId, int? priority) =>
        new(exerciseId, priority);
    }
}
