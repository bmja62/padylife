using Application.Cqrs.Commands;
using Application.Questions.DTOs;
using Common.Utilities;
using Data.Contracts;
using Entities.Questions;
using Microsoft.AspNetCore.Http;
using Services;

namespace Application.Questions.Commands
{
    public class CreateQuestionCommand(CreateQuestionCommandDTO input) : ICommand<ServiceResult>
    {
        public CreateQuestionCommandDTO Input { get; } = input;
    }
    public class CreateQuestionCommandHandler(IRepository<Question> questionRepository,IHttpContextAccessor httpContextAccessor) : ICommandHandler<CreateQuestionCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
        {

            var creator = httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();
            var questionCategory = request.Input.QuestionCategoryId;
            var questionText = request.Input.Text;
            var displayText = request.Input.DisplayText;
            int priority = 0;
            var questionOptions = request.Input.QuestionOptions.Select(t => QuestionOption.CreateDefault(t.Text, priority++)).ToList();

            Question newQuestion = Question.CreateDefault(questionCategory, questionText, displayText, creator, questionOptions);
            await questionRepository.AddAsync(newQuestion, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
