using Asp.Versioning;
using AutoMapper;
using Common.GridResults;
using Data.Contracts;
using Common.Utilities;
using Entities.Excersies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PadyLife.Api.Models.DTOs;
using Services;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// مراحل تمرینات
    /// </summary>
    [ApiVersion("1")]
    public class StepsController(IRepository<Step> exerciseStepRepository, IMapper mapper,IHttpContextAccessor accessor) : CrudController<StepDTO, StepDTO, Step, long>(exerciseStepRepository, mapper)
    {
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<GlobalGridResult<StepDTO>>> GetAllByFilter(int pageNumber, int count, string search, bool? allUsers, CancellationToken cancellationToken)
        {
            var query = exerciseStepRepository.Table.Include(t => t.StepOptions).AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(t => t.Name.Contains(search));
         

            var currentUserId = accessor.HttpContext.User.Identity.GetUserId<long>();

            if (allUsers.HasValue && !allUsers.Value)
                query = query.Where(t => t.CreatedByUserId == currentUserId);

            var data = await query
                .Skip((pageNumber - 1) * count)
                .Take(count)
                .Select(t => new StepDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    CreatedByUserId = t.CreatedByUserId,
                }).ToListAsync(cancellationToken);

            var totalCount = await query.CountAsync(cancellationToken);
            return (ServiceResult.Ok(new GlobalGridResult<StepDTO>
            {
                Data = data,
                TotalCount = totalCount
            })).ToApiResult();
        }
    }
}
