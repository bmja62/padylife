using Application.Cqrs.Commands;
using Entities.Common;
using Services;
using Services.Services.RateServices;
using Services.Services.RateServices.DTOs;

namespace Application.Rates.Commands
{
    public class UpdateRateCommand(long userId, long entityId, EntityType entityType, UpdateRateDTO input) : ICommand<ServiceResult>
    {
        public long UserId { get; } = userId;
        public long EntityId { get; } = entityId;
        public EntityType EntityType { get; } = entityType;
        public UpdateRateDTO Input { get; } = input;
    }

    public class UpdateRateCommandHandler(IRateService rateService) : ICommandHandler<UpdateRateCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(UpdateRateCommand request, CancellationToken cancellationToken)
        {
            await rateService.UpdateRateAsync(request.UserId, request.EntityId, request.EntityType, request.Input);
            return ServiceResult.Ok();
        }
    }
}
