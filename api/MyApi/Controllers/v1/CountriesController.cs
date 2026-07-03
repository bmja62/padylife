using Asp.Versioning;
using AutoMapper;
using Data.Contracts;
using Entities.Locations;
using PadyLife.Api.Models.DTOs;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر کشور ها
    /// </summary>
    /// <param name="countryRepository"></param>
    /// <param name="mapper"></param>
    [ApiVersion("1")]
    public class CountriesController(IRepository<Country> countryRepository, IMapper mapper) : CrudController<CountryDTO, CountryDTO, Country, long>(countryRepository, mapper)
    {
    }
}
