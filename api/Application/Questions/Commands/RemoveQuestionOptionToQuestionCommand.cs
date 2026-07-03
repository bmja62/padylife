using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Questions;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Questions.Commands
{
    public class RemoveQuestionOptionToQuestionCommand(long optionId) : ICommand<ServiceResult>
    {
        public long OptionId { get; } = optionId;
    }

    public class RemoveQuestionOptionToQuestionCommandHandler(IRepository<QuestionOption> questionOptionRepository) : ICommandHandler<RemoveQuestionOptionToQuestionCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(RemoveQuestionOptionToQuestionCommand request, CancellationToken cancellationToken)
        {
            var questionOptionInDb = await questionOptionRepository.Table.Where(t => t.Id == request.OptionId).FirstOrDefaultAsync(cancellationToken);
            await questionOptionRepository.DeleteAsync(questionOptionInDb, cancellationToken);
            return ServiceResult.Ok();
        }
    }
}
