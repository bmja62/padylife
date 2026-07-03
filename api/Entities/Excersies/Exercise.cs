using Entities.Common;
using Entities.Users;

namespace Entities.Excersies
{
    //مدل تمرین
    public class Exercise : BaseEntity<long>
    {

        public Exercise(string title, string imageUrl, ExerciseType exerciseType, string documentLink, string exerciseGoal, string practiceMethod, int exerciseCount, string exerciseEstimate, long exerciseCategoryId, long creator, List<ExerciseStep> exerciseSteps)
        {
            Title = title;
            ImageUrl = imageUrl;
            ExerciseType = exerciseType;
            DocumentLink = documentLink;
            ExerciseGoal = exerciseGoal;
            PracticeMethod = practiceMethod;
            ExerciseCount = exerciseCount;
            ExerciseEstimate = exerciseEstimate;
            ExerciseCategoryId = exerciseCategoryId;
            ExerciseSteps = exerciseSteps;
            CreatedByUserId = creator;
        }

        private Exercise()
        {

        }

        //FKs
        public long ExerciseCategoryId { get; set; }

        //Props
        public string Title { get; set; }
        public ExerciseType ExerciseType { get; set; }
        public string DocumentLink { get; set; }
        public string ExerciseGoal { get; set; }
        public string PracticeMethod { get; set; }
        public int ExerciseCount { get; set; }
        public string ExerciseEstimate { get; set; }
        public string ImageUrl { get; set; }
        public long? CreatedByUserId { get; set; }

        //Navigations
        public ICollection<ExerciseStep> ExerciseSteps { get; set; }
        public ICollection<ExerciseLinked> ExerciseLinks { get; set; }
        public ICollection<UserExercise> UserExercises { get; set; }
        public ICollection<UserPlanExcersiesAnswer> UserPlanExcersiesAnswers { get; set; }
        public ExerciseCategory ExerciseCategory { get; set; }
        public User CreatedByUser { get; set; }

        public static Exercise CreateDefault(
            string title,
            string imageUrl,
            ExerciseType exerciseType,
            string documentLink,
            string exerciseGoal,
            string practiceMethod,
            int exerciseCount,
            string exerciseEstimate,
            long exerciseCategoryId,
            long creator,
            List<ExerciseStep> exerciseSteps) =>
            new(title, imageUrl, exerciseType, documentLink, exerciseGoal, practiceMethod, exerciseCount, exerciseEstimate, exerciseCategoryId,creator, exerciseSteps);

        public void AddStep(ExerciseStep stepInDb)
        {
            ExerciseSteps.Add(stepInDb);
        }

        public void SetCreateByUserId(long creator) => CreatedByUserId = creator;

        public void SetDocumentLink(string documentLink)
        => DocumentLink = documentLink;

        public void SetExerciseCategoryInDb(long exersieCategoryId)
        => ExerciseCategoryId = exersieCategoryId;

        public void SetExerciseCount(int exerciseCount)
        => ExerciseCount = exerciseCount;

        public void SetExerciseEstimate(string exerciseEstimate)
        => ExerciseEstimate = exerciseEstimate;

        public void SetExerciseGoal(string exerciseGoal)
        => ExerciseGoal = exerciseGoal;

        public void SetExerciseType(ExerciseType exerciseType)
        => ExerciseType = exerciseType;

        public void SetImageUrl(string imageUrl) => ImageUrl = imageUrl;

        public void SetPracticeMethod(string practiceMethod)
        => PracticeMethod = practiceMethod;

        public void SetTitle(string title)
        => Title = title;
    }
}
