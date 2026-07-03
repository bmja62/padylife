using Application.Cqrs.Queris;
using Application.DynamicSiteSettings.DTO;
using Common.GridResults;
using Data.Contracts;
using Entities.DynamicSiteSettings;
using Microsoft.EntityFrameworkCore;
using Services;


namespace Application.DynamicSiteSettings.Query.GetAllDynamicSiteSettingsByType
{
    public record GetAllDynamicSiteSettingsByTypeQuery(GetAllDynamicSiteSettingsByTypeRequestDto Input)
      : IQuery<ServiceResult<GlobalGridResult<DynamicSiteSettingResponseDTO>>>;
    public class GetAllDynamicSiteSettingsByTypeHandler
        : IQueryHandler<GetAllDynamicSiteSettingsByTypeQuery, ServiceResult<GlobalGridResult<DynamicSiteSettingResponseDTO>>>
    {
        private readonly IRepository<DynamicSiteSetting> _repository;

        public GetAllDynamicSiteSettingsByTypeHandler(IRepository<DynamicSiteSetting> repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<GlobalGridResult<DynamicSiteSettingResponseDTO>>> Handle(GetAllDynamicSiteSettingsByTypeQuery request, CancellationToken cancellationToken)
        {


            var Query = _repository.Table
                   .Where(x => x.Type == request.Input.Type
                   && !string.IsNullOrEmpty(request.Input.Search) ? x.Key.Contains(request.Input.Search) : true)
                   .Select(e => new DynamicSiteSettingResponseDTO(e.Id, e.Key, e.Type, e.JsonValue, e.CreateDate, e.UpdateDate));
            var TotalCount = await Query.CountAsync();
            var Data = await Query.Skip(request.Input.Skip)
                .Take(request.Input.Count.Value).ToListAsync();


            GlobalGridResult<DynamicSiteSettingResponseDTO> result = new GlobalGridResult<DynamicSiteSettingResponseDTO> { Data = Data, TotalCount = TotalCount };
            return ServiceResult<GlobalGridResult<DynamicSiteSettingResponseDTO>>.Ok(result);


        }
    }

}
