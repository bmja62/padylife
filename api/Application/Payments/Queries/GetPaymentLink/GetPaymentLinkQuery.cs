using Application.Cqrs.Commands;
using Application.Cqrs.Events;
using Application.Cqrs.Queris;
using Application.Orders.Events;
using Application.Orders.Extentions;
using Application.Payments.Commands.Verify;
using Application.Payments.DTOs;
using Data.Contracts;
using Entities.Orders;
using Entities.Payments;
using Entities.Plans;
using Entities.Wallets;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Services.PaymentServices;
using Services.Services.PaymentServices.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payments.Queries.GetPaymentLink
{
    public record GetPaymentLinkQuery(long OrderId) : ICommand<ServiceResult<PaymentInvoiceDTO>>;
    public class GetPaymentLinkQueryHandler(IRepository<Order> _orderRepository, IRepository<Plan> _planRepository, IRepository<Wallet> _walletRepository, IPaymentService PaymentService, IDomainEventDispatcher domainEventDispatcher) : ICommandHandler<GetPaymentLinkQuery, ServiceResult<PaymentInvoiceDTO>>
    {
        public async Task<ServiceResult<PaymentInvoiceDTO>> Handle(GetPaymentLinkQuery request, CancellationToken cancellationToken)
        {
            var Order = await _orderRepository.Table.Where(t => t.Id == request.OrderId).Include(t => t.Discount).Include(t => t.Items).FirstOrDefaultAsync(cancellationToken);
            if (Order == null)
                return ServiceResult.BadRequest<PaymentInvoiceDTO>("درخواست تولید لینک پرداخت با مشکلی مواجه شده");
            //پرداخت تخفیف صد درصدی
            if (Order.FinalAmount == 0 && (Order.DiscountId.HasValue && Order.DiscountId.Value > 0) && Order.Discount.DiscountPercentage == 100)
            {
                var orderItemsList = Order.Items.ToList();
                Order.SetPayedFullDiscount();
                Order.AddDomainEvent(new OrderPaymentCompletedEvent(Order.UserId, orderItemsList));

                await _orderRepository.UpdateAsync(Order, cancellationToken);
                return ServiceResult.Ok<PaymentInvoiceDTO>(new PaymentInvoiceDTO
                {
                    Link = null,
                    TokenRef = null,
                });
            }
            //پلن رایگان
            if (Order.FinalAmount == 0 && Order.Items.Any(a => a.ItemType == OrderItemType.Plan))
            {
                var orderItemsList = Order.Items.ToList();
                var oIds = orderItemsList.Select(oi => oi.ObjectId).ToList();
                var plan = await _planRepository.Table.Where(a => oIds.Contains(a.Id)).FirstOrDefaultAsync(cancellationToken);
                if (plan is not null && plan.FinalPrice is null)
                {
                    Order.SetFreePlan();
                    Order.AddDomainEvent(new OrderPaymentCompletedEvent(Order.UserId, orderItemsList));
                    await _orderRepository.UpdateAsync(Order, cancellationToken);
                    return ServiceResult.Ok<PaymentInvoiceDTO>(new PaymentInvoiceDTO
                    {
                        Link = null,
                        TokenRef = null,
                    });
                }
            }

            var Payment = await PaymentService.GetPaymentLink((long)Order.FinalAmount, request.OrderId, Order.UserId);
            return Payment.IsSuccess ? ServiceResult.Ok<PaymentInvoiceDTO>(Payment.Data) : ServiceResult.BadRequest<PaymentInvoiceDTO>("درخواست تولید لینک پرداخت با مشکلی مواجه شده");
        }
    }
}
