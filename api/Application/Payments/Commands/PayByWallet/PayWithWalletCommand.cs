using Application.Cqrs.Commands;
using Application.Orders.Events;
using Data;
using Data.Contracts;
using Entities.Orders;
using Entities.Wallets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payments.Commands.PayByWallet
{
    public record PayWithWalletCommand(long OrderId) : ICommand<ServiceResult<WalletPaymentResultDTO>>;

    public class WalletPaymentResultDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public long OrderId { get; set; }
        public decimal Amount { get; set; }
    }

    public class PayWithWalletCommandHandler : ICommandHandler<PayWithWalletCommand, ServiceResult<WalletPaymentResultDTO>>
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Wallet> _walletRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<PayWithWalletCommandHandler> _logger;

        public PayWithWalletCommandHandler(
            IRepository<Order> orderRepository,
            IRepository<Wallet> walletRepository,
            ILogger<PayWithWalletCommandHandler> logger,
            ApplicationDbContext dbContext)
        {
            _orderRepository = orderRepository;
            _walletRepository = walletRepository;
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<ServiceResult<WalletPaymentResultDTO>> Handle(PayWithWalletCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("درخواست پرداخت از کیف پول برای سفارش {OrderId}", request.OrderId);

            // دریافت سفارش
            var order = await _orderRepository.Table
                .Where(t => t.Id == request.OrderId)
                .Include(t => t.Items)
                .FirstOrDefaultAsync(cancellationToken);

            if (order == null)
            {
                _logger.LogWarning("سفارش با شناسه {OrderId} یافت نشد", request.OrderId);
                return ServiceResult.BadRequest<WalletPaymentResultDTO>("سفارش یافت نشد");
            }

            // دریافت کیف پول کاربر
            var userWallet = await _walletRepository.Table
                .Where(t => t.UserId == order.UserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (userWallet == null)
            {
                _logger.LogWarning("کیف پول برای کاربر {UserId} یافت نشد", order.UserId);
                return ServiceResult.BadRequest<WalletPaymentResultDTO>("کیف پول کاربر یافت نشد");
            }

            // بررسی موجودی کافی
            if (userWallet.Credit < order.FinalAmount)
            {
                _logger.LogWarning("موجودی کیف پول کاربر {UserId} کافی نیست. موجودی: {Credit}, مبلغ سفارش: {Amount}",
                    order.UserId, userWallet.Credit, order.FinalAmount);
                return ServiceResult.BadRequest<WalletPaymentResultDTO>("موجودی کیف پول کافی نیست");
            }

            try
            {
          

                // برداشت از کیف پول
                userWallet.Withdraw(order.FinalAmount, order.UserId, $"پرداخت سفارش #{order.Id}");
                await _walletRepository.UpdateAsync(userWallet, cancellationToken);

                // بروزرسانی وضعیت سفارش
                var orderItemsList = order.Items.ToList();
                order.SetPayedByWallet();
                order.AddDomainEvent(new OrderPaymentCompletedEvent(order.UserId, orderItemsList));
                await _orderRepository.UpdateAsync(order, cancellationToken);

              

                _logger.LogInformation("پرداخت از کیف پول برای سفارش {OrderId} با موفقیت انجام شد. مبلغ: {Amount}",
                    order.Id, order.FinalAmount);

                return ServiceResult.Ok(new WalletPaymentResultDTO
                {
                    Success = true,
                    Message = "پرداخت با موفقیت انجام شد",
                    OrderId = order.Id,
                    Amount = order.FinalAmount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در پرداخت از کیف پول برای سفارش {OrderId}", request.OrderId);
                return ServiceResult.BadRequest<WalletPaymentResultDTO>("خطا در پرداخت از کیف پول");
            }
        }
    }
}
