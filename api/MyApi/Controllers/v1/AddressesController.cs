using Asp.Versioning;
using AutoMapper;
using Common.GridResults;
using Data.Contracts;
using Entities.Addresses.ECommerce.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PadyLife.Api.Models.DTOs;
using Services;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر آدرس ها
    /// </summary>
    /// <param name="addressRepository"></param>
    /// <param name="mapper"></param>
    [ApiVersion("1")]
    public class AddressesController(IRepository<Address> addressRepository, IMapper mapper) : CrudController<AddressDTO, AddressDTO, Address, long>(addressRepository, mapper)
    {
        [HttpGet]
        public async Task<ApiResult<GlobalGridResult<AddressDTO>>> GetByFilter(int pageNumber, int count, string search, long? userId, long? countryId, long? provinceId, long? cityId, CancellationToken cancellationToken)
        {
            var query = addressRepository.Table;


            if (!string.IsNullOrEmpty(search))
                query = query.Where(t =>
                t.Province.ProvinceName.Contains(search) ||
                t.Province.ProvinceNameFa.Contains(search) ||
                t.City.CityName.Contains(search) ||
                t.City.CityNameFa.Contains(search) ||
                t.AddressText.Contains(search) ||
                t.RecipientName.Contains(search) ||
                t.RecipientPhone.Contains(search) ||
                t.LandlinePhone.Contains(search)
                );

            if (userId.HasValue)
                query = query.Where(t => t.UserId == userId.Value);

            if (countryId.HasValue)
                query = query.Where(t => t.CountryId == countryId);

            if (provinceId.HasValue)
                query = query.Where(t => t.ProvinceId == provinceId);

            if (cityId.HasValue)
                query = query.Where(t => t.CityId == cityId);

            var data = await query.Select(t => new AddressDTO
            {
                Id = t.Id,
                CityId = t.CityId,
                CountryId = t.CountryId,
                ProvinceId = t.ProvinceId,
                UserId = t.UserId,
                AddressText = t.AddressText,
                AddressType = t.AddressType,
                Floor = t.Floor,
                GeoLocation = t.GeoLocation,
                IsDefault = t.IsDefault,
                LandlinePhone = t.LandlinePhone,
                Plaque = t.Plaque,
                PostalCode = t.PostalCode,
                RecipientName = t.RecipientName,
                RecipientPhone = t.RecipientPhone,
                Unit = t.Unit
            }).Skip((pageNumber - 1) * count).Take(count)
            .ToListAsync(cancellationToken);
            var totalCount = await query.CountAsync(cancellationToken);

            return (ServiceResult.Ok(new GlobalGridResult<AddressDTO>
            {
                Data = data,
                TotalCount = totalCount
            })).ToApiResult();
        }
    }
}
