using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Questions;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Questions.Commands
{
    public class DeleteQuestionCommand(long id) : ICommand<ServiceResult>
    {
        public long Id { get; } = id;
    }

    public class DeleteQuestionCommandHandler(IRepository<Question> questionRepository) : ICommandHandler<DeleteQuestionCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
        {
            var questionInDb = await questionRepository.Table.Where(t => t.Id == request.Id).FirstOrDefaultAsync();

            await questionRepository.SoftDeleteAsync(questionInDb, cancellationToken);
            return ServiceResult.Ok();
        }
    }
}
