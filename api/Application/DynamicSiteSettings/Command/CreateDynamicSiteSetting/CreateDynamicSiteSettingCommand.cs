using Application.Cqrs.Commands;
using Application.DynamicSiteSettings.DTO;
using Data.Contracts;
using Entities.DynamicSiteSettings;
using Services;

namespace Application.DynamicSiteSettings.Command.CreateDynamicSiteSetting
{
    public record CreateDynamicSiteSettingCommand(CreateDynamicSiteSettingDTO Input)
        : ICommand<ServiceResult<DynamicSiteSettingResponseDTO>>;

    public class CreateDynamicSiteSettingHandler
    : ICommandHandler<CreateDynamicSiteSettingCommand, ServiceResult<DynamicSiteSettingResponseDTO>>
    {
        private readonly IRepository<DynamicSiteSetting> _repository;

        public CreateDynamicSiteSettingHandler(IRepository<DynamicSiteSetting> repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<DynamicSiteSettingResponseDTO>> Handle(CreateDynamicSiteSettingCommand request, CancellationToken cancellationToken)
        {
            var entity = new DynamicSiteSetting
            {
                Key = request.Input.Key,
                Type = request.Input.Type,
                JsonValue = request.Input.JsonValue,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            await _repository.AddAsync(entity, cancellationToken);

            return ServiceResult<DynamicSiteSettingResponseDTO>.Ok(
                new DynamicSiteSettingResponseDTO(entity.Id, entity.Key, entity.Type, entity.JsonValue, entity.CreateDate, entity.UpdateDate)
            );
        }
    }



}
