using Asp.Versioning;
using AutoMapper;
using Common.GridResults;
using Data.Contracts;
using Entities.Locations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PadyLife.Api.Models.DTOs;
using Services;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر شهرها
    /// </summary>
    /// <param name="cityRepository"></param>
    /// <param name="mapper"></param>
    [ApiVersion("1")]
    public class CitiesController(IRepository<City> cityRepository, IMapper mapper) : CrudController<CityDTO, CityDTO, City, long>(cityRepository, mapper)
    {
        [HttpGet]
        public async Task<ApiResult<GlobalGridResult<CityDTO>>> GetByFilter(int pageNumber, int count, string search, long? provinceId, bool? isActive, CancellationToken cancellationToken)
        {
            var query = cityRepository.Table;


            if (!string.IsNullOrEmpty(search))
                query = query.Where(t =>
                t.CityName.Contains(search) ||
                t.CityNameFa.Contains(search)
                );

            if (provinceId.HasValue)
                query = query.Where(t => t.ProvinceId == provinceId.Value);

            if (isActive.HasValue)
                query = query.Where(t => t.IsActive == isActive);

            var data = await query.Select(t => new CityDTO
            {
                Id = t.Id,
                CityName = t.CityName,
                CityNameFa = t.CityNameFa,
                CityCode = t.CityCode,
                IsActive = t.IsActive,
                ProvinceId = t.ProvinceId
            }).Skip((pageNumber - 1) * count).Take(count)
            .ToListAsync(cancellationToken);
            var totalCount = await query.CountAsync(cancellationToken);

            return (ServiceResult.Ok(new GlobalGridResult<CityDTO>
            {
                Data = data,
                TotalCount = totalCount
            })).ToApiResult();
        }
    }
}
