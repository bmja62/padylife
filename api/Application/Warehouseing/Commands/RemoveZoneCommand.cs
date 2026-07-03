using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Warehouseing;
using Services;

namespace Application.Warehouseing.Commands
{
    public record RemoveZoneCommand(long id) : ICommand<ServiceResult>;

    public class RemoveZoneCommandHandLer(IRepository<WarehouseZone> repository) : ICommandHandler<RemoveZoneCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(RemoveZoneCommand request, CancellationToken cancellationToken)
        {
            var dataInDb = await repository.GetByIdAsync(cancellationToken, request.id);
            if (dataInDb != null)
            {
                await repository.DeleteAsync(dataInDb, cancellationToken);
                return ServiceResult.Ok();
            }
            return ServiceResult.NotFound("یافت نشد");
        }
    }
}
