using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Application.Warehouseing.Commands;
using Application.Warehouseing.DTOs;
using Application.Warehouseing.DTOs.Application.Warehousing.DTOs;
using Application.Warehouseing.Queries;
using Asp.Versioning;
using Common.GridResults;
using Common.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر انبار
    /// </summary>
    [ApiVersion("1")]
    public class WarehouseController : BaseController
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public WarehouseController(
            ICommandDispatcher commandDispatcher,
            IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        /// <summary>
        /// دریافت لیست انبارها
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{UserRoleNames.Admin}")]
        public async Task<ApiResult<GlobalGridResult<WarehouseDTO>>> GetWarehouses([FromQuery] GlobalGrid globalGrid, bool? isActive) =>
            (await _queryDispatcher.SendAsync(new GetWarehousesQuery(globalGrid, isActive)))
            .ToApiResult();

        /// <summary>
        /// دریافت اطلاعات یک انبار
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{UserRoleNames.Admin}")]
        public async Task<ApiResult<WarehouseDetailDTO>> GetWarehouse(long id) =>
            (await _queryDispatcher.SendAsync(new GetWarehouseQuery(id)))
            .ToApiResult();

        /// <summary>
        /// ایجاد انبار جدید
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{UserRoleNames.Admin}")]
        public async Task<ApiResult<WarehouseDTO>> Create([FromBody] CreateWarehouseDTO input) =>
            (await _commandDispatcher.SendAsync(new CreateWarehouseCommand(input)))
            .ToApiResult();

        /// <summary>
        /// ویرایش انبار
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{UserRoleNames.Admin}")]
        public async Task<ApiResult> Update(long id, [FromBody] UpdateWarehouseDTO input) =>
            (await _commandDispatcher.SendAsync(new UpdateWarehouseCommand(id, input)))
            .ToApiResult();

        /// <summary>
        /// غیرفعال کردن انبار
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{UserRoleNames.Admin}")]
        public async Task<ApiResult> Deactivate(long id) =>
            (await _commandDispatcher.SendAsync(new DeactivateWarehouseCommand(id)))
            .ToApiResult();

        /// <summary>
        /// افزودن منطقه به انبار
        /// </summary>
        [HttpPost("{warehouseId}/zones")]
        [Authorize(Roles = $"{UserRoleNames.Admin}")]
        public async Task<ApiResult<WarehouseZoneDTO>> AddZone(long warehouseId, [FromBody] AddZoneDTO input) =>
            (await _commandDispatcher.SendAsync(new AddZoneCommand(warehouseId, input)))
            .ToApiResult();

        /// <summary>
        /// افزودن منطقه به انبار
        /// </summary>
        [HttpPost("zones")]
        [Authorize(Roles = $"{UserRoleNames.Admin}")]
        public async Task<ApiResult> RemoveZone([FromBody] RemoveZoneCommand input) =>
            (await _commandDispatcher.SendAsync(input))
            .ToApiResult();

        /// <summary>
        /// دریافت موجودی یک محصول/واریانت در انبارها
        /// </summary>
        [HttpGet("inventory/{productId}")]
        [Authorize]
        public async Task<ApiResult<GetProductInventoryDTO>> GetProductInventory(
            long productId, long? variantId = null) =>
            (await _queryDispatcher.SendAsync(new GetProductInventoryQuery(productId, variantId)))
            .ToApiResult();
    }
}