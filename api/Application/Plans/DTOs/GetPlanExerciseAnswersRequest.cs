using Common.GridResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Plans.DTOs
{
    public class GetPlanExerciseAnswersRequest : GlobalGrid
    {
        public long PlanId { get; set; }
        public bool? OnlyCompleted { get; set; }
        public long? ExerciseId { get; set; }
        public long? StepId { get; set; }
        public bool? ForAdmin { get; set; }
        public string? AnswerType { get; set; } // "text", "image", "choice", "multiple-choice"
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
    }

    public record PlanExerciseAnswersItem(
        long UserPlanId,
        long UserId,
        string UserFullName,
        DateTime StartDate,
        DateTime? EndDate,
        bool IsCompleted,
        bool IsSignUpPlan,
        List<ExerciseAnswerDto> ExerciseAnswers
    );

    public record ExerciseAnswerDto(
        long AnswerId,
        long ExerciseId,
        string ExerciseName,
        long StepId,
        string StepName,
        long? SelectedStepOptionId,
        string SelectedOptionText,
        string TextAnswer,
        string ImageUrl,
        List<SelectedChoiceDto> SelectedChoices
    );

    public record SelectedChoiceDto(
        long OptionChoiceId,
        string ChoiceText,
        bool IsCorrect,
        string Feedback
    );
}
