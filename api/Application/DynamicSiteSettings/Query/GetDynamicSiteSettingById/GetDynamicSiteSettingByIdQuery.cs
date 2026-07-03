using Application.Cqrs.Queris;
using Application.DynamicSiteSettings.DTO;
using Data.Contracts;
using Entities.DynamicSiteSettings;
using Services;

namespace Application.DynamicSiteSettings.Query.GetDynamicSiteSettingById
{
    public record GetDynamicSiteSettingByIdQuery(long Id)
      : IQuery<ServiceResult<DynamicSiteSettingResponseDTO>>;
    public class GetDynamicSiteSettingByIdHandler
        : IQueryHandler<GetDynamicSiteSettingByIdQuery, ServiceResult<DynamicSiteSettingResponseDTO>>
    {
        private readonly IRepository<DynamicSiteSetting> _repository;

        public GetDynamicSiteSettingByIdHandler(IRepository<DynamicSiteSetting> repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<DynamicSiteSettingResponseDTO>> Handle(GetDynamicSiteSettingByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(cancellationToken, request.Id);
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
