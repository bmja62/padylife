using Application.Cqrs.Commands;
using Application.Questions.DTOs;
using Data.Contracts;
using Entities.Questions;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Questions.Commands
{
    public class UpdateQuestionOptionCommand(UpdateQuestionOptionDTO input) : ICommand<ServiceResult>
    {
        public UpdateQuestionOptionDTO Input { get; } = input;
    }

    public class UpdateQuestionOptionCommandHandler(IRepository<QuestionOption> questionOptionRepository) : ICommandHandler<UpdateQuestionOptionCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(UpdateQuestionOptionCommand request, CancellationToken cancellationToken)
        {
            var questionOptionInDb = await questionOptionRepository.Table.Where(t => t.Id == request.Input.QuestionOptionId).FirstOrDefaultAsync();
            if (questionOptionInDb is null)
                return ServiceResult.NotFound("یافت نشد");

            questionOptionInDb.SetText(request.Input.Text);
            questionOptionInDb.SetPriority(request.Input.Priority);

            return ServiceResult.Ok();
        }
    }
}
