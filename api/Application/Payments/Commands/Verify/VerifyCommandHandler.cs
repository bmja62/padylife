using Application.Cqrs.Commands;
using Application.Orders.Events;
using Application.Payments.DTOs;
using Data.Contracts;
using Entities.Baskets;
using Entities.Orders;
using Entities.Payments;
using Entities.Plans;
using Entities.Users;
using Entities.Wallets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // اضافه کردن لاگر
using Org.BouncyCastle.Bcpg;
using Services;
using Services.Services.PaymentServices;
using Services.Services.SmsStrategy;
using Services.Services.SmsStrategy.SmsServices;
using Services.Services.WalletsServices;
using System.Threading;

namespace Application.Payments.Commands.Verify
{
    public class VerifyCommandHandler : ICommandHandler<VerifyCommand, ServiceResult<VerifyResultDTO>>
    {
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Wallet> _walletRepository;
        private readonly IRepository<Plan> _planRepository;
        private readonly IRepository<ExpertPlanPrice> _expertPlanPriceRepository;
        private readonly IWalletService _walletService;
        private readonly ILogger<VerifyCommandHandler> _logger;
        private readonly ISmsService _smsService;

        public VerifyCommandHandler(
            IRepository<Payment> paymentRepository,
            IPaymentGatewayService paymentGatewayService,
            IRepository<Order> orderRepository,
            IRepository<User> userRepository,
            IRepository<Wallet> walletRepository,
            IRepository<Plan> planRepository,
            IWalletService walletService,
            ILogger<VerifyCommandHandler> logger,
            ISmsService smsService,
            IRepository<ExpertPlanPrice> expertPlanPriceRepository)
        {
            _paymentRepository = paymentRepository;
            _paymentGatewayService = paymentGatewayService;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _walletRepository = walletRepository;
            _planRepository = planRepository;
            _walletService = walletService;
            _logger = logger;
            _smsService = smsService;
            _expertPlanPriceRepository = expertPlanPriceRepository;
        }

        public async Task<ServiceResult<VerifyResultDTO>> Handle(VerifyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("پرداخت با تراک آیدی {TrackId} در حال بررسی است.", request.TrackId);

            var payment = await _paymentRepository.Table
                .FirstOrDefaultAsync(p => p.TrackingNumber == request.TrackId, cancellationToken);

            if (payment is null)
            {
                _logger.LogWarning("پرداخت با تراک آیدی {TrackId} یافت نشد.", request.TrackId); 
                return ServiceResult.BadRequest<VerifyResultDTO>("پرداخت یافت نشد");
            }

            if (payment.IsPaid)
            {
                _logger.LogWarning("پرداخت با تراک آیدی {TrackId} قبلاً تایید شده است.", request.TrackId);
                return ServiceResult.BadRequest<VerifyResultDTO>("پرداخت تایید شده است");
            }

            var verifyResult = await _paymentGatewayService.VerifyPayment(payment.TrackingNumber);
            if (!verifyResult.IsSuccess)
            {
                _logger.LogError("پرداخت با تراک آیدی {TrackId} با خطا مواجه شد: {Message}", request.TrackId, verifyResult.Message); 
                await HandlePaymentFailed(payment, cancellationToken);
                return ServiceResult.BadRequest<VerifyResultDTO>(verifyResult.Message);
            }

            _logger.LogInformation("پرداخت با تراک آیدی {TrackId} با موفقیت تایید شد.", request.TrackId); 

            payment.SetGatewayReferenceNumber(verifyResult.Data.ReferenceNumber);
            payment.SetPayed();
            await _paymentRepository.UpdateAsync(payment, cancellationToken);

            return payment.WalletCharge
                ? await HandleWalletCharge(payment, cancellationToken)
                : await HandleOrderPayment(payment, cancellationToken);
        }

        private async Task HandlePaymentFailed(Payment payment, CancellationToken cancellationToken)
        {
            if (payment.WalletCharge) return;

            var order = await _orderRepository.TableNoTracking.Include(a => a.Items).Where(a => a.Id == payment.OrderId).FirstOrDefaultAsync();  
            var orderItemsList = order.Items.ToList();
            order.AddDomainEvent(new OrderPaymentFailedEvent(order.UserId, orderItemsList));

            _logger.LogInformation("پرداخت برای سفارش {OrderId} ناموفق بود.", payment.OrderId);
        }

        private async Task<ServiceResult<VerifyResultDTO>> HandleWalletCharge(Payment payment, CancellationToken cancellationToken)
        {
            var wallet = await _walletService.GetOrCreateByUserId(payment.UserId.Value);
            wallet.Deposit(payment.Amount, payment.UserId, "شارژ کیف پول توسط درگاه پرداخت");
            await _walletRepository.UpdateAsync(wallet, cancellationToken);

            _logger.LogInformation("کیف پول کاربر {UserId} با مبلغ {Amount} شارژ شد.", payment.UserId, payment.Amount); 

            return ServiceResult.Ok(new VerifyResultDTO
            {
                InsurancePaperId = null,
                WalletCharge = true
            });
        }

        private async Task<ServiceResult<VerifyResultDTO>> HandleOrderPayment(Payment payment, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.TableNoTracking.Include(t => t.User).Include(a => a.Items).Where(a => a.Id == payment.OrderId).FirstOrDefaultAsync();
            if (order is null)
            {
                _logger.LogWarning("سفارش با آیدی {OrderId} یافت نشد.", payment.OrderId);
                return ServiceResult.BadRequest<VerifyResultDTO>("سفارش یافت نشد");
            }

            var orderItemsList = order.Items.ToList();
            order.SetPayed();
            order.AddDomainEvent(new OrderPaymentCompletedEvent(order.UserId, orderItemsList));
            await _orderRepository.UpdateAsync(order, cancellationToken);

            _logger.LogInformation("پرداخت برای سفارش {OrderId} با موفقیت انجام شد.", payment.OrderId);
            var itemType = order.Items.Select(t => t.ItemType).FirstOrDefault();
            var itemId = order.Items.Select(t => t.ObjectId).FirstOrDefault();
            var itemName =
                itemType == OrderItemType.Plan ? await _planRepository.Table.Where(t => t.Id == itemId).Select(t => t.Title ?? "-").FirstOrDefaultAsync(cancellationToken) :
                itemType == OrderItemType.ExpertPlanPrice ? await _expertPlanPriceRepository.Table.Include(t => t.Expert).Where(t => t.Id == itemId).Select(t => t.Expert.FullName ?? "-").FirstOrDefaultAsync(cancellationToken) :
                "-" ;

            await _smsService.SendPaymentSuccess(order.User.PhoneNumber, itemName);



            return ServiceResult.Ok(new VerifyResultDTO
            {
                CouponPurchase = true,
                OrderId = order.Id,
            });
        }
    }
}
