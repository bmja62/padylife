using Entities.Excersies;

namespace Application.Excersies.DTOs
{
    public class CreateExcersiesCommandDTO
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public ExerciseType ExerciseType { get; set; }
        public string DocumentLink { get; set; }
        public string ExerciseGoal { get; set; }
        public string PracticeMethod { get; set; }
        public int ExerciseCount { get; set; }
        public string ExerciseEstimate { get; set; }

        public long ExerciseCategoryId { get; set; }
        public long[] StepIds { get; set; }
    }
}
