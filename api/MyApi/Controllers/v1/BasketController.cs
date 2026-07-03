using Application.Baskets.Commands;
using Application.Baskets.DTOs;
using Application.Baskets.Queries;
using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر سبد خرید
    /// </summary>
    [ApiVersion("1")]
    public class BasketController : BaseController
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public BasketController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        /// <summary>
        /// مشاهده سبد خرید کاربر
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        [Authorize]
        public async Task<ApiResult> GetOrCreateBasket(long userId) =>
            (await _queryDispatcher
                .SendAsync(new GetOrCreateBasketQuery(userId)))
            .ToApiResult();

        /// <summary>
        /// افزودن آیتم به سبد خرید
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("{userId}/items")]
        [Authorize]
        public async Task<ApiResult> AddItem(long userId, [FromBody] AddBasketItemDTO input) =>
            (await _commandDispatcher
                .SendAsync(new AddItemToBasketCommand(userId, input)))
            .ToApiResult();

        /// <summary>
        /// ویرایش تعداد آیتم در سبد خرید
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{userId}/items/{itemId}")]
        [Authorize]
        public async Task<ApiResult> UpdateItemQuantity([FromBody] UpdateBasketItemQuantityCommandDTO input) =>
            (await _commandDispatcher
                .SendAsync(new UpdateBasketItemQuantityCommand(input)))
            .ToApiResult();

        /// <summary>
        /// حذف آیتم از سبد خرید
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [HttpDelete("{userId}/items/{itemId}")]
        [Authorize]
        public async Task<ApiResult> RemoveItem(RemoveItemFromBasketCommandDTO input) =>
            (await _commandDispatcher
                .SendAsync(new RemoveItemFromBasketCommand(input)))
            .ToApiResult();

        /// <summary>
        /// مشاهده تاریخچه تغییرات سبد خرید
        /// </summary>
        /// <param name="basketId"></param>
        /// <returns></returns>
        [HttpGet("{basketId}/history")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ApiResult> GetHistory(long basketId) =>
            (await _queryDispatcher
                .SendAsync(new GetBasketHistoryQuery(basketId)))
            .ToApiResult();

        /// <summary>
        /// تبدیل سبد به سفارش
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> BasketToOrder([FromBody] BasketToOrderDTO input) =>
            (await _commandDispatcher
                .SendAsync(new BasketToOrderCommand(input)))
            .ToApiResult();


        /// <summary>
        /// دریافت جزئیات سبد مربوط به محصولات
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("basket/details")]
        public async Task<ApiResult> GetBasketItemDetailsAsProduct([FromBody] GetBasketItemDetailsQuery query) =>
              (await _queryDispatcher
                .SendAsync(query))
            .ToApiResult();


    }
}
