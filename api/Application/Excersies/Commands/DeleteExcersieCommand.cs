using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Excersies;
using Services;

namespace Application.Excersies.Commands
{
    public class DeleteExcersieCommand(long id) : ICommand<ServiceResult>
    {
        public long Id { get; } = id;
    }

    public class DeleteExcersieCommandHandler(IRepository<Exercise> exerciseRepository) : ICommandHandler<DeleteExcersieCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(DeleteExcersieCommand request, CancellationToken cancellationToken)
        {
            await exerciseRepository.SoftDeleteAsync(await exerciseRepository.GetByIdAsync(cancellationToken, request.Id), cancellationToken);
            return ServiceResult.Ok();
        }
    }
}
