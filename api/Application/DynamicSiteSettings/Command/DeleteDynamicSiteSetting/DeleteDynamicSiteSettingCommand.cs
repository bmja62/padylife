using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.DynamicSiteSettings;
using Services;

namespace Application.DynamicSiteSettings.Command.DeleteDynamicSiteSetting
{
    public record DeleteDynamicSiteSettingCommand(long Id)
     : ICommand<ServiceResult>;
    public class DeleteDynamicSiteSettingHandler
        : ICommandHandler<DeleteDynamicSiteSettingCommand, ServiceResult>
    {
        private readonly IRepository<DynamicSiteSetting> _repository;

        public DeleteDynamicSiteSettingHandler(IRepository<DynamicSiteSetting> repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult> Handle(DeleteDynamicSiteSettingCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(cancellationToken, request.Id);
            if (entity == null)
                return ServiceResult.NotFound("DynamicSiteSetting not found");

            await _repository.DeleteAsync(entity, cancellationToken);
            return ServiceResult.Ok();
        }
    }

}
