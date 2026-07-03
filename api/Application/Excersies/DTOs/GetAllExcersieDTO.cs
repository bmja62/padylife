using Entities.Excersies;

namespace Application.Excersies.DTOs
{
    public class GetAllExcersieDTO
    {
        public GetAllExcersieDTO(long id, string title, string imageUrl, long exerciseCategoryId, string name, string documentLink, DateTime createdAt, string exerciseEstimate, string exerciseGoal, int exerciseCount, ExerciseType exerciseType, DateTime? updatedAt, string practiceMethod, long questionLinkedId, List<ExerciseStepsDTO> exerciseStepsDTOs)
        {
            Id = id;
            Title = title;
            ImageUrl = imageUrl;
            ExerciseCategoryId = exerciseCategoryId;
            Name = name;
            DocumentLink = documentLink;
            CreatedAt = createdAt;
            ExerciseEstimate = exerciseEstimate;
            ExerciseGoal = exerciseGoal;
            ExerciseCount = exerciseCount;
            ExerciseType = exerciseType;
            UpdatedAt = updatedAt;
            PracticeMethod = practiceMethod;
            QuestionLinkedId = questionLinkedId;
            ExerciseStepsDTOs = exerciseStepsDTOs;
        }

        public long Id { get; }
        public string Title { get; }
        public string ImageUrl { get; set; }
        public long ExerciseCategoryId { get; }
        public string Name { get; }
        public string DocumentLink { get; }
        public DateTime CreatedAt { get; }
        public string ExerciseEstimate { get; }
        public string ExerciseGoal { get; }
        public string PracticeMethod { get; }
        public long QuestionLinkedId { get; set; }
        public int ExerciseCount { get; }
        public ExerciseType ExerciseType { get; }
        public DateTime? UpdatedAt { get; }
        public List<ExerciseStepsDTO> ExerciseStepsDTOs { get; }

        internal static GetAllExcersieDTO CreateDefalt
            (
            long id,
            string title,
            string imageUrl,
            long exerciseCategoryId,
            string name,
            string documentLink,
            DateTime createdAt,
            string exerciseEstimate,
            string exerciseGoal,
            int exerciseCount,
            ExerciseType exerciseType,
            DateTime? updatedAt,
            string practiceMethod,
            long questionLinkedId,
            List<ExerciseStepsDTO> exerciseStepsDTOs)
        => new
            (
            id,
            title,
            imageUrl,
            exerciseCategoryId,
            name,
            documentLink,
            createdAt,
            exerciseEstimate,
            exerciseGoal,
            exerciseCount,
            exerciseType,
            updatedAt,
            practiceMethod,
            questionLinkedId,
            exerciseStepsDTOs
            );
    }

    public class ExerciseStepsDTO
    {
        public ExerciseStepsDTO(long stepId, long exerciseId, string name, DateTime createdAt)
        {
            StepId = stepId;
            ExerciseId = exerciseId;
            Name = name;
            CreatedAt = createdAt;
        }

        public long StepId { get; }
        public long ExerciseId { get; }
        public string Name { get; }
        public DateTime CreatedAt { get; }

        internal static ExerciseStepsDTO CreateDefault(long stepId, long exerciseId, string name, DateTime createdAt)
        => new(stepId, exerciseId, name, createdAt);
    }
}
