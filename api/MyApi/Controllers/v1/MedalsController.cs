using Asp.Versioning;
using Common.GridResults;
using Common.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.MedalServices;
using Services.Services.MedalServices.DTOs;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر مدال ها
    /// </summary>
    [ApiVersion("1")]
    public class MedalsController : BaseController
    {
        private readonly IMedalService _medalService;


        public MedalsController(IMedalService medalService)
        {
            _medalService = medalService;
        }

        [HttpPost("assign")]
        [Authorize]
        public async Task<IActionResult> CheckAndAssign([FromQuery] long userId)
        {
            await _medalService.CheckAndAssignMedalsAsync(userId);
            return Ok(new { message = "مدال‌های کاربر بررسی و اختصاص داده شدند." });
        }
        /// <summary>
        /// دریافت تمامی مدال ها
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult<GlobalGridResult<GetMetalDTO>>> GetAll([FromQuery] int pageNumber = 1, int count = 10)
        => (await _medalService.GetAll(pageNumber, count, User.Identity.GetUserId<long>())).ToApiResult();




    }
}
