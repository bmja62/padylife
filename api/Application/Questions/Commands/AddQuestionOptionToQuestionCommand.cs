using Application.Cqrs.Commands;
using Application.Questions.DTOs;
using Data.Contracts;
using Entities.Questions;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Questions.Commands
{
    public class AddQuestionOptionToQuestionCommand(AddQuestionOptionToQuestionDTO input) : ICommand<ServiceResult>
    {
        public AddQuestionOptionToQuestionDTO Input { get; } = input;
    }
    public class AddQuestionOptionToQuestionCommandHandler(IRepository<Question> questionRepository) : ICommandHandler<AddQuestionOptionToQuestionCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(AddQuestionOptionToQuestionCommand request, CancellationToken cancellationToken)
        {
            var questionInDb = await questionRepository.Table.Where(t => t.Id == request.Input.QuestionId).FirstOrDefaultAsync();

            questionInDb.AddQuestionOptions(QuestionOption.CreateWithQuestionId(request.Input.Text, request.Input.QuestionId));

            await questionRepository.UpdateAsync(questionInDb, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
