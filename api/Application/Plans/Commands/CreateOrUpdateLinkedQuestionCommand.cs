using Application.Cqrs.Commands;
using Application.Questions.DTOs;
using Data.Contracts;
using Entities.Excersies;
using Entities.Questions;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Commands
{
    public class CreateOrUpdateLinkedQuestionCommand(CreateOrUpdateLinkedQuestionCommandDTO input) : ICommand<ServiceResult>
    {
        public CreateOrUpdateLinkedQuestionCommandDTO Input { get; } = input;
    }

    public class UpdateLinkedQuestionCommandHandler(IRepository<QuestionLinked> questionLinkedRepository) : ICommandHandler<CreateOrUpdateLinkedQuestionCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CreateOrUpdateLinkedQuestionCommand request, CancellationToken cancellationToken)
        {
            var LinkedQuestionInDb = await questionLinkedRepository.Table
                .Where(t =>
                t.PlanId == request.Input.PlanId &&
                t.PlanQuestionId == request.Input.PlanQuestionId &&
                t.QuestionOptionId == request.Input.QuestionOptionId
                ).FirstOrDefaultAsync();

            if (LinkedQuestionInDb is null)
            {
                List<ExerciseLinked> ExerciseLinkeds = new();
                if (request.Input.ExerciseLinks != null && request.Input.ExerciseLinks.Count > 0)
                    ExerciseLinkeds = request.Input?.ExerciseLinks?.Select(t => ExerciseLinked.Create
                        (
                            t.ExerciseId,
                            t.Priority
                            )
                        ).ToList();
                QuestionLinked questionLinked = QuestionLinked.CreateDefault
                    (
                    request.Input.PlanId,
                    request.Input.PlanQuestionId,
                    request.Input.QuestionOptionId,
                    request.Input.LinkedPlanQuestionId,
                    ExerciseLinkeds
                    );
                await questionLinkedRepository.AddAsync(questionLinked, cancellationToken);
                return ServiceResult.Ok();
            }

            LinkedQuestionInDb.SetLinkedQuestion(request.Input.LinkedPlanQuestionId);

            if (!LinkedQuestionInDb.HasValidLinks)
                return ServiceResult.BadRequest("نمی توان به یک پاسخ ،سوال به همراه تمرین لینک کرد");

            await questionLinkedRepository.UpdateAsync(LinkedQuestionInDb, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
