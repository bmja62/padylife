using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Application.Inventories.Commands;
using Application.Inventories.DTOs;
using Application.Inventories.Queries;
using Application.Warehouseing.DTOs.Application.Inventory.DTOs;
using Asp.Versioning;
using Common.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر موجودی
    /// </summary>
    [ApiVersion("1")]
    public class InventoryController : BaseController
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public InventoryController(
            ICommandDispatcher commandDispatcher,
            IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        /// <summary>
        /// افزایش موجودی
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{UserRoleNames.Admin}")]
        public async Task<ApiResult> IncreaseStock([FromBody] AdjustStockDTO dto) =>
            (await _commandDispatcher.SendAsync(new IncreaseStockCommand(dto)))
            .ToApiResult();

        /// <summary>
        /// کاهش موجودی
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{UserRoleNames.Admin}")]
        public async Task<ApiResult> DecreaseStock([FromBody] AdjustStockDTO dto) =>
            (await _commandDispatcher.SendAsync(new DecreaseStockCommand(dto)))
            .ToApiResult();

        /// <summary>
        /// انتقال موجودی بین انبارها
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{UserRoleNames.Admin}")]
        public async Task<ApiResult> TransferStock([FromBody] TransferStockDTO dto) =>
            (await _commandDispatcher.SendAsync(new TransferStockCommand(dto)))
            .ToApiResult();

        /// <summary>
        /// رزرو موجودی
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> ReserveStock([FromBody] ReserveStockDTO dto) =>
            (await _commandDispatcher.SendAsync(new ReserveStockCommand(dto)))
            .ToApiResult();

        /// <summary>
        /// آزادسازی موجودی رزرو شده
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> ReleaseStock([FromBody] ReleaseStockDTO dto) =>
            (await _commandDispatcher.SendAsync(new ReleaseStockCommand(dto)))
            .ToApiResult();

        /// <summary>
        /// دریافت تاریخچه تراکنش‌های موجودی
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{UserRoleNames.Admin}")]
        public async Task<ApiResult<List<InventoryTransactionDTO>>> GetHistory(
            long productId, long? variantId = null, DateTime? fromDate = null, DateTime? toDate = null) =>
            (await _queryDispatcher.SendAsync(new GetInventoryHistoryQuery(productId, variantId, fromDate, toDate)))
            .ToApiResult();

        /// <summary>
        /// دریافت لیست کالاهای با موجودی کم
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{UserRoleNames.Admin}")]
        public async Task<ApiResult<List<LowStockItemDTO>>> GetLowStockItems(long? warehouseId = null) =>
            (await _queryDispatcher.SendAsync(new GetLowStockItemsQuery(warehouseId)))
            .ToApiResult();
    }
}