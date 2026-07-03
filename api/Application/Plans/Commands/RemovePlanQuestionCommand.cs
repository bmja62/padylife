using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Plans;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Commands
{
    public class RemovePlanQuestionCommand(long id) : ICommand<ServiceResult>
    {
        public long Id { get; } = id;
    }

    public class RemovePlanQuestionCommandHandler(IRepository<PlanQuestion> planQuestionRepository) : ICommandHandler<RemovePlanQuestionCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(RemovePlanQuestionCommand request, CancellationToken cancellationToken)
        {
            var planQuestionInDb = await planQuestionRepository.Table.Where(t => t.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            await planQuestionRepository.DeleteAsync(planQuestionInDb, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
