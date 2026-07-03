using Application.Cqrs.Queris;
using Application.DynamicSiteSettings.DTO;
using Data.Contracts;
using Entities.DynamicSiteSettings;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.DynamicSiteSettings.Query.GetDynamicSiteSettingByTypeAndKey
{
    public record GetDynamicSiteSettingByTypeAndKeyQuery(string Type, string Key)
            : IQuery<ServiceResult<DynamicSiteSettingResponseDTO>>;
    public class GetDynamicSiteSettingByTypeAndKeyHandler
    : IQueryHandler<GetDynamicSiteSettingByTypeAndKeyQuery, ServiceResult<DynamicSiteSettingResponseDTO>>
    {
        private readonly IRepository<DynamicSiteSetting> _repository;

        public GetDynamicSiteSettingByTypeAndKeyHandler(IRepository<DynamicSiteSetting> repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<DynamicSiteSettingResponseDTO>> Handle(GetDynamicSiteSettingByTypeAndKeyQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.Table
                .Where(x => x.Type == request.Type && x.Key == request.Key)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)

                return ServiceResult.NotFound<DynamicSiteSettingResponseDTO>("DynamicSiteSetting not found");


            var dto = new DynamicSiteSettingResponseDTO(
                entity.Id,
                entity.Key,
                entity.Type,
                entity.JsonValue,
                entity.CreateDate,
                entity.UpdateDate
            );

            return ServiceResult<DynamicSiteSettingResponseDTO>.Ok(dto);

        }
    }

}
