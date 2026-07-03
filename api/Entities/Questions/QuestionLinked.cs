using Entities.Common;
using Entities.Excersies;
using Entities.Plans;

namespace Entities.Questions
{
    public class QuestionLinked : IEntity
    {
        private QuestionLinked() { }
        public QuestionLinked(
            long? linkedPlanQuestionId, // سوال بعدی در صورت وجود
            List<ExerciseLinked> linkedExercises  // لیست تمرینات مرتبط
            )
        {
            if (!HasValidLinks)
                throw new InvalidOperationException("نمی توان به یک پاسخ سوال به همراه تمرین لینک کرد");

            LinkedPlanQuestionId = linkedPlanQuestionId;
            if (linkedExercises != null)
            {
                ExerciseLinks = linkedExercises.ToList();
            }
        }

        public QuestionLinked(long planId, long planQuestionId, long questionOptionId,
                             long? linkedPlanQuestionId, List<ExerciseLinked> linkedExercises)
            : this(linkedPlanQuestionId, linkedExercises)
        {
            PlanId = planId;
            PlanQuestionId = planQuestionId;
            QuestionOptionId = questionOptionId;
            LinkedPlanQuestionId = linkedPlanQuestionId;
            ExerciseLinks = linkedExercises;
        }

        // اعتبارسنجی برای اطمینان از وجود فقط یک لینک
        public bool HasValidLinks
            => !(LinkedPlanQuestionId != null && ExerciseLinks?.Any() == true);


        // کلید مصنوعی برای ارتباطات
        public long Id { get; set; }

        //FKs  کلیدهای ترکیبی اصلی
        public long PlanId { get; set; }
        public long PlanQuestionId { get; set; }
        public long QuestionOptionId { get; set; }
        public long? LinkedPlanQuestionId { get; set; } // سوال بعدی

        //Navigations
        public Plan Plan { get; set; }
        public PlanQuestion PlanQuestion { get; set; }
        public QuestionOption QuestionOption { get; set; }
        public PlanQuestion LinkedPlanQuestion { get; set; }
        public List<ExerciseLinked> ExerciseLinks { get; set; } = new();

        //Methods
        public void SetLinkedQuestion(long? objectId) => LinkedPlanQuestionId = objectId;

        public void AddExerciseLink(ExerciseLinked exerciseLink)
        {
            if (ExerciseLinks.Any(x => x.ExerciseId == exerciseLink.ExerciseId))
                return; // از اضافه شدن تمرین تکراری جلوگیری می‌کند

            ExerciseLinks.Add(exerciseLink);
        }

        public void RemoveExerciseLink(long exerciseId)
        {
            var link = ExerciseLinks.FirstOrDefault(x => x.ExerciseId == exerciseId);
            if (link != null)
            {
                ExerciseLinks.Remove(link);
            }
        }

        public static QuestionLinked CreateDefault(long planId, long planQuestionId, long questionOptionId,
                                                long? linkedPlanQuestionId, List<ExerciseLinked> linkedExercises)
            => new(planId, planQuestionId, questionOptionId, linkedPlanQuestionId, linkedExercises);
    }


}
