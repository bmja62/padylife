using Application.Excersies.DTOs;

namespace Application.Plans.DTOs
{
    public class PlanQuestionOptionDTO
    {
        public long Id { get; set; }
        public long QuestionId { get; set; }
        public string Text { get; set; }
        public bool HasValidLinks { get; set; }
        public long? LinkedPlanQuestionId { get; set; }
        public PlanQuestionDTO LinkedQuestion { get; internal set; }
        public List<ExerciseDTO> LinkedExercises { get; internal set; }


    }

    public class ReadOnlyQuestionOptionsDTO
    {
        public ReadOnlyQuestionOptionsDTO(long id, string text, int priority)
        {
            Id = id;
            Text = text;
            Priority = priority;
        }

        public long Id { get; set; }
        public string Text { get; set; }
        public int Priority { get; set; }
        internal static ReadOnlyQuestionOptionsDTO CreateDefault(long id, string text, int priority)
        => new(id, text, priority);
    }
}
