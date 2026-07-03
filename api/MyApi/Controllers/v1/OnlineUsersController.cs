using Application.Plans.DTOs;
using Asp.Versioning;
using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PadyLife.Api.Models.DTOs;
using Services.Hubs;
using Services.Services.OnlineUsersServices;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کاربران حاضر
    /// </summary>
    [ApiVersion("1")]
    public class OnlineUsersController : BaseController
    {
        private readonly IOnlineUsersService _onlineUsersService;
        private readonly ILogger<OnlineUsersController> _logger;

        public OnlineUsersController(
            IOnlineUsersService onlineUsersService,
            ILogger<OnlineUsersController> logger)
        {
            _onlineUsersService = onlineUsersService;
            _logger = logger;
        }

        [HttpGet("count")]
        public async Task<ApiResult<CountDTO>> GetOnlineUsersCount()
        {
            try
            {
                var count = CountDTO.Create(await _onlineUsersService.GetOnlineUsersCountAsync());
                return new ApiResult<CountDTO>(true, ApiResultStatusCode.Success, count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting online users count");
                return new ApiResult<CountDTO>(false, ApiResultStatusCode.ServerError, CountDTO.Create(0));
            }
        }

        [HttpGet("active")]
        public async Task<ApiResult<List<OnlineUserDto>>> GetActiveUsers()
        {
            try
            {
                var users = await _onlineUsersService.GetActiveUsersAsync();
                return new ApiResult<List<OnlineUserDto>>(true, ApiResultStatusCode.Success, users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active users");
                return new ApiResult<List<OnlineUserDto>>(false, ApiResultStatusCode.ServerError, new List<OnlineUserDto>());
            }
        }

        [HttpGet("stats")]
        public async Task<ApiResult<OnlineUserStatsDto>> GetOnlineStats()
        {
            try
            {
                var stats = await _onlineUsersService.GetOnlineUsersStatsAsync();
                return new ApiResult<OnlineUserStatsDto>(true, ApiResultStatusCode.Success, stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting online stats");
                return new ApiResult<OnlineUserStatsDto>(false, ApiResultStatusCode.ServerError, new OnlineUserStatsDto());
            }
        }

        [HttpGet("pages/top")]
        public async Task<ApiResult<List<PageStatDto>>> GetTopPages()
        {
            try
            {
                var stats = await _onlineUsersService.GetOnlineUsersStatsAsync();
                return new ApiResult<List<PageStatDto>>(true, ApiResultStatusCode.Success, stats.Pages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top pages");
                return new ApiResult<List<PageStatDto>>(false, ApiResultStatusCode.ServerError, new List<PageStatDto>());
            }
        }

        public class CountDTO 
        {
            public CountDTO(int count)
            {
                Count = count;
            }
            public int Count { get; set; }

            internal static CountDTO Create(int count) => new(count);
        }
    }
}
