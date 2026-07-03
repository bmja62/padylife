using Application.Baskets.DTOs;
using Application.Baskets.Extentions;
using Application.Cqrs.Commands;
using Application.Cqrs.Events;
using Application.Orders.Events;
using Common.Utilities;
using Data.Contracts;
using Entities.Baskets;
using Entities.Discounts;
using Entities.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Services.PaymentServices;
using Services.Services.PaymentServices.DTOs;

namespace Application.Baskets.Commands
{
    public class BasketToOrderCommand(BasketToOrderDTO input) : ICommand<ServiceResult<OrderIdDto>>
    {
        public BasketToOrderDTO Input { get; } = input;
    }

    public class BasketToOrderCommandHandler(
        IHttpContextAccessor accessor,
        IRepository<Basket> basketRepository,
        IRepository<Discount> discountRepository,
        IRepository<Order> orderRepository
        ) : ICommandHandler<BasketToOrderCommand, ServiceResult<OrderIdDto>>
    {
        public async Task<ServiceResult<OrderIdDto>> Handle(BasketToOrderCommand request, CancellationToken cancellationToken)
        {
            var userId = accessor.HttpContext.User.Identity.GetUserId<long>();
            var basket = await basketRepository.Table
               .Where(b => b.UserId == userId)
               .Include(b => b.Items)
               .FirstOrDefaultAsync(cancellationToken);

            var discount = await discountRepository.Table.Where(t => t.Code.Equals(request.Input.DiscountCode)).FirstOrDefaultAsync();

            // تبدیل سبد به سفارش
            ServiceResult<Order> orderResult = basket.ConvertToOrder(discount);
            if (orderResult.IsSuccess)
            {
                var order = orderResult.Data;
                // ذخیره سفارش
                await orderRepository.AddAsync(order, cancellationToken);

                order.AddDomainEvent(new NewOrderEvent(userId, basket.Items, order.Items));
              
                return  ServiceResult<OrderIdDto>.Ok(new OrderIdDto { OrderId=order.Id}) ;
            }
            return  ServiceResult.BadRequest<OrderIdDto> ("حطا در روند ثبت سفارش");
        }
    }
}
