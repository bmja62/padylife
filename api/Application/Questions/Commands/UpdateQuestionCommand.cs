using Application.Cqrs.Commands;
using Application.Questions.DTOs;
using Common.Utilities;
using Data.Contracts;
using Entities.Questions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Questions.Commands
{
    public class UpdateQuestionCommand(long id, UpdateQuestionDTO input) : ICommand<ServiceResult>
    {
        public long Id { get; } = id;
        public UpdateQuestionDTO Input { get; } = input;
    }

    public class UpdateQuestionCommandHandler(IRepository<Question> questionRepository,IHttpContextAccessor httpContextAccessor) : ICommandHandler<UpdateQuestionCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
        {
            var questionInDb = await questionRepository.Table.Where(t => t.Id == request.Id).FirstOrDefaultAsync();
            var creator = httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();
            questionInDb.SetText(request.Input.Text);
            questionInDb.SetDisplayText(request.Input.DisplayText);
            questionInDb.SetQuestionCategoryId(request.Input.QuestionCategoryId);
            questionInDb.SetCreateByUserId(creator);

            await questionRepository.UpdateAsync(questionInDb, cancellationToken);
            return ServiceResult.Ok();
        }
    }
}
