using Application.Cqrs.Commands;
using Application.DynamicSiteSettings.DTO;
using Data.Contracts;
using Entities.DynamicSiteSettings;
using Services;

namespace Application.DynamicSiteSettings.Command.UpdateDynamicSiteSetting
{
    public record UpdateDynamicSiteSettingCommand(UpdateDynamicSiteSettingDTO Input)
         : ICommand<ServiceResult<DynamicSiteSettingResponseDTO>>;

    public class UpdateDynamicSiteSettingHandler
        : ICommandHandler<UpdateDynamicSiteSettingCommand, ServiceResult<DynamicSiteSettingResponseDTO>>
    {
        private readonly IRepository<DynamicSiteSetting> _repository;

        public UpdateDynamicSiteSettingHandler(IRepository<DynamicSiteSetting> repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<DynamicSiteSettingResponseDTO>> Handle(UpdateDynamicSiteSettingCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(cancellationToken, request.Input.Id);
            if (entity == null)
                return (ServiceResult<DynamicSiteSettingResponseDTO>)
                                     ServiceResult.NotFound("DynamicSiteSetting not found");

            entity.JsonValue = request.Input.JsonValue;
            entity.UpdateDate = DateTime.UtcNow;
            await _repository.UpdateAsync(entity, cancellationToken);

            return ServiceResult<DynamicSiteSettingResponseDTO>.Ok(
                new DynamicSiteSettingResponseDTO(entity.Id, entity.Key, entity.Type, entity.JsonValue, entity.CreateDate, entity.UpdateDate)
            );
        }
    }

}
