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
    /// کنترلر استان ها
    /// </summary>
    /// <param name="provinceRepository"></param>
    /// <param name="mapper"></param>
    [ApiVersion("1")]
    public class ProvincesController(IRepository<Province> provinceRepository, IMapper mapper) : CrudController<ProvinceDTO, ProvinceDTO, Province, long>(provinceRepository, mapper)
    {
        [HttpGet]
        public async Task<ApiResult<GlobalGridResult<ProvinceDTO>>> GetByFilter(int pageNumber, int count, string search, long? countryId, bool? isActive, CancellationToken cancellationToken)
        {
            var query = provinceRepository.Table;

            if (!string.IsNullOrEmpty(search))
                query = query.Where(t =>
                t.ProvinceName.Contains(search) ||
                t.ProvinceNameFa.Contains(search)
                );

            if (countryId.HasValue)
                query = query.Where(t => t.CountryId == countryId.Value);

            if (isActive.HasValue)
                query = query.Where(t => t.IsActive == isActive);

            var data = await query.Select(t => new ProvinceDTO
            {
                Id = t.Id,
                ProvinceName = t.ProvinceName,
                ProvinceNameFa = t.ProvinceNameFa,
                ProvinceCode = t.ProvinceCode,
                IsActive = t.IsActive,
            }).Skip((pageNumber - 1) * count).Take(count)
            .ToListAsync(cancellationToken);
            var totalCount = await query.CountAsync(cancellationToken);

            return (ServiceResult.Ok(new GlobalGridResult<ProvinceDTO>
            {
                Data = data,
                TotalCount = totalCount
            })).ToApiResult();
        }
    }
}
