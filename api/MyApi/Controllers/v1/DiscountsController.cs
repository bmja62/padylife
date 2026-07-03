using Asp.Versioning;
using AutoMapper;
using Data.Contracts;
using Entities.Discounts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PadyLife.Api.Models.DTOs;
using Services;
using System.Data.Entity;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر کد تخفیف
    /// </summary>
    /// <param name="discountRepository"></param>
    /// <param name="mapper"></param>
    [ApiVersion("1")]
    public class DiscountsController(IRepository<Discount> discountRepository, IMapper mapper) : CrudController<DiscountDTO, DiscountDTO, Discount, long>(discountRepository, mapper)
    {
        public override async Task<ApiResult<DiscountDTO>> Create(DiscountDTO dto, CancellationToken cancellationToken)
        {
            if (discountRepository.Table.Any(t => t.Code.Equals(dto.Code))) 
            {
                return (ServiceResult.BadRequest<DiscountDTO>($" کد < {dto.Code} > تکراری میباشد")).ToApiResult();
            }
            return await base.Create(dto, cancellationToken);
        }
    }
}
