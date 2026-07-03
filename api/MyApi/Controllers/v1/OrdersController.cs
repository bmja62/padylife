using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Application.Orders.Commands;
using Application.Orders.DTOs;
using Application.Orders.Queries;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.PdfServices;
using Services.Services.PdfServices.DTOs;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر سفارشات
    /// </summary>
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/orders")]
    public class OrdersController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, IPdfService pdfService) : BaseController
    {
        /// <summary>
        /// دریافت لیست سفارشات
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ApiResult> GetOrders([FromQuery] GetOrdersQuery query) =>
            (await queryDispatcher.SendAsync(query)).ToApiResult();

        /// <summary>
        /// دریافت جزئیات سفارش
        /// </summary>
        [HttpGet("{orderId}")]
        [Authorize]
        public async Task<ApiResult> GetOrderDetails(long orderId) =>
            (await queryDispatcher.SendAsync(new GetOrderDetailsQuery(orderId))).ToApiResult();

        /// <summary>
        /// تغییر وضعیت سفارش توسط ادمین
        /// </summary>
        [HttpPut("{orderId}/status")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ApiResult> ChangeStatus(long orderId, [FromBody] UpdateOrderStatusDTO dto) =>
            (await commandDispatcher.SendAsync(new UpdateOrderStatusCommand(orderId, dto.NewStatus))).ToApiResult();

        /// <summary>
        /// دریافت فایل PDF فاکتور سفارش
        /// </summary>
        [HttpGet("{orderId}/invoice")]
        [Authorize]
        public async Task<IActionResult> GetInvoicePdf(long orderId)
        {
            var result = await queryDispatcher.SendAsync(new GetOrderDetailsQuery(orderId));
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            PDFOrderDetailsDTO pdfDTO = new PDFOrderDetailsDTO
            {
                OrderId = result.Data.Id,
                OrderDate = result.Data.CreatedAt,
                TotalPrice = result.Data.TotalAmount,
                Status = result.Data.Status,
                PaymentStatus = result.Data.PaymentStatus,
                Address = result.Data.Address,
                UserInfo = new PDFUserInfoDTO
                {
                    Id = result.Data.UserInfo.Id,
                    FullName = result.Data.UserInfo.FullName,
                    PhoneNumber = result.Data.UserInfo.PhoneNumber
                },
                Items = result.Data.Items.Select(t => new PDFOrderItemDTO
                {
                    ProductName = t.Title,
                    ProductSKU = t.Title,
                    Quantity = t.Quantity,
                    UnitPrice = t.UnitPrice,
                    TotalPrice = t.UnitPrice * t.Quantity,
                }).ToList(),
            };
            var response = await pdfService.GenerateInvoicePdf(pdfDTO);

            return File(response.Data.Content, "application/pdf", response.Data.FileName);
        }

        /// <summary>
        /// کنسل کردن سفارش (با اصلاح موجودی)
        /// </summary>
        [HttpPost("{orderId}/cancel")]
        [Authorize]
        public async Task<ApiResult> CancelOrder(long orderId, long warehouseId) =>
            (await commandDispatcher.SendAsync(new CancelOrderCommand(orderId, warehouseId))).ToApiResult();

        /// <summary>
        /// حذف سفارش (با اصلاح موجودی)
        /// </summary>
        [HttpDelete("{orderId}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ApiResult> DeleteOrder(long orderId, long warehouseId) =>
            (await commandDispatcher.SendAsync(new DeleteOrderCommand(orderId, warehouseId))).ToApiResult();

        ///// <summary>
        ///// گزارش سفارشات (برای ادمین)
        ///// </summary>
        //[HttpGet("report")]
        //[Authorize(Roles = "Admin,SuperAdmin")]
        //public async Task<ApiResult> GetOrderReport([FromQuery] GetOrdersReportQuery query) =>
        //    (await queryDispatcher.SendAsync(query)).ToApiResult();
    }
}
