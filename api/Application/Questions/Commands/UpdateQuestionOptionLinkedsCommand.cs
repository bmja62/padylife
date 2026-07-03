using Application.Cqrs.Commands;
using Application.Questions.DTOs;
using Data.Contracts;
using Entities.Questions;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Questions.Commands
{
    public class UpdateQuestionOptionLinkedsCommand(UpdateQuestionOptionLinkedsDTO input) : ICommand<ServiceResult>
    {
        public UpdateQuestionOptionLinkedsDTO Input { get; } = input;
    }

    public class UpdateQuestionOptionLinkedsCommandHandler(IRepository<QuestionLinked> questionLinkedRepository) : ICommandHandler<UpdateQuestionOptionLinkedsCommand, ServiceResult>
    {

        public async Task<ServiceResult> Handle(UpdateQuestionOptionLinkedsCommand request, CancellationToken cancellationToken)
        {
            var questionLinkedInDb = await questionLinkedRepository.Table.Where(t => t.PlanId == request.Input.PlanId && t.QuestionOptionId == request.Input.QuestionOptionId).FirstOrDefaultAsync(cancellationToken);
            if (questionLinkedInDb is null)
            {
                return ServiceResult.NotFound("یافت نشد");
            }

            bool needUpdate = false;
            if (request.Input.Type == UpdateQuestionOptionType.Question)
            {
                needUpdate = true;
                questionLinkedInDb.SetLinkedQuestion(request.Input.ObjectId);
            }

            if (needUpdate && questionLinkedInDb.HasValidLinks)
                await questionLinkedRepository.UpdateAsync(questionLinkedInDb, cancellationToken);
            else
                return ServiceResult.BadRequest("آپدیتی رخ نداد");

            return ServiceResult.Ok();
        }
    }
}
