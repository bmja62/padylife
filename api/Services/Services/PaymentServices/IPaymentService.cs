using Common;
using Data.Contracts;
using Entities.Payments;
using IdGen;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Services.Services.PaymentServices.DTOs;

namespace Services.Services.PaymentServices
{
    public interface IPaymentService
    {
        Task<ServiceResult<PaymentInvoiceDTO>> GetPaymentLink(long price, long orderId, long userId);
        Task<ServiceResult<PaymentInvoiceDTO>> GetWalletChargePaymentLink(long amount, long userId);
    }
    public class PaymentService(IConfiguration configuration, IdGenerator idGenerator,
        IHttpContextAccessor httpContextAccessor,
        IPaymentGatewayService paymentGatewayService,
        IRepository<Payment> paymentRepositry) : IPaymentService, IScopedDependency
    {
        public async Task<ServiceResult<PaymentInvoiceDTO>> GetPaymentLink(long price, long orderId, long userId)
        {
            var systemTrackId = idGenerator.CreateId();
            var backendUrl = configuration["BaseUrl"];
            var callBack = $"{backendUrl}/api/v1/Payments/Verify/Verify?trackId={systemTrackId}";
            var result = await paymentGatewayService.CreatePaymentLink(systemTrackId, price, callBack);
            if (!result.IsSuccess)
                return ServiceResult.BadRequest<PaymentInvoiceDTO>(result.Message);

            var payment = new Payment
            {
                Amount = price,
                UserId = userId,
                TrackingNumber = systemTrackId,
                OrderId = orderId,
                Token = result.Data.TokenRef?? systemTrackId.ToString(),
                IsPaid = false,
                WalletCharge = false,
                GatewayAccountName = "ZarinPal Gateway",
                GatewayName = "ZarinPal",
                CreatedOn = DateTime.Now,
                GatewayReferenceNumber = systemTrackId.ToString(),
                TransactionCode = systemTrackId.ToString()
            };

            await paymentRepositry.AddAsync(payment, CancellationToken.None, true);
            return result;
        }
        public async Task<ServiceResult<PaymentInvoiceDTO>> GetWalletChargePaymentLink(long amount, long userId)
        {
            var systemTrackId = idGenerator.CreateId();
            var backendUrl = configuration["BaseUrl"];
            var callBack = $"{backendUrl}/api/v1/Payments/Verify/Verify?trackId={systemTrackId}";
            var result = await paymentGatewayService.CreatePaymentLink(systemTrackId, amount, callBack);
            if (!result.IsSuccess)
                return ServiceResult.BadRequest<PaymentInvoiceDTO>(result.Message);

            var payment = new Payment()
            {
                Amount = amount,
                UserId = userId,
                IsPaid = false,
                WalletCharge = true,
                TrackingNumber = systemTrackId,
                Token = result.Data.TokenRef ?? systemTrackId.ToString(),
                GatewayAccountName = "ZarinPal Gateway",
                GatewayName = "ZarinPal",
                CreatedOn = DateTime.Now,
                GatewayReferenceNumber = systemTrackId.ToString(),
                TransactionCode = systemTrackId.ToString()
            };


            try
            {
                await paymentRepositry.AddAsync(payment, CancellationToken.None, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }
    }
}
