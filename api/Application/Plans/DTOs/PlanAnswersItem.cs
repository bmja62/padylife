

namespace Application.Plans.DTOs
{
    public record PlanAnswersItem(
        long UserPlanId,
        long UserId,
        string UserFullName,
        DateTime StartDate,
        bool IsCompleted,
        List<AnswerDto> Answers
    );

    public record AnswerDto(
        long PlanQuestionId,
        bool IsMain,
        long QuestionId,
        string QuestionText,
        long SelectedOptionId,
        string SelectedOptionText
    );
}
