namespace Application.Questions.DTOs
{
    public class CreateOrUpdateLinkedQuestionCommandDTO
    {
        public long PlanId { get; set; }
        public long PlanQuestionId { get; set; }
        public long QuestionOptionId { get; set; }
        public long? LinkedPlanQuestionId { get; set; }
        public List<ExerciseLinkDTO> ExerciseLinks { get; set; }
    }


}
public class ExerciseLinkDTO
{
    public long ExerciseId { get; set; }
    public int? Priority { get; set; } // اولویت نمایش تمرین
}
