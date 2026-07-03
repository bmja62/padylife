using Common;
using Data.Contracts;
using Entities.Payments;
using Microsoft.Extensions.Logging;
using Parbad;
using Parbad.Gateway.ZarinPal;
using Services.Services.PaymentServices.DTOs;

namespace Services.Services.PaymentServices
{
    public interface IPaymentGatewayService
    {
        Task<ServiceResult<PaymentInvoiceDTO>> CreatePaymentLink(long trackingId, long amount, string callBackUrl);
        Task<ServiceResult<VerifyPaymentResultDTO>> VerifyPayment(long trackId);
    }

    public class PaymentGatewayService(
        IOnlinePayment onlinePayment,
        IRepository<Payment> paymentRepository,
        ILogger<PaymentGatewayService> logger
        ) : IPaymentGatewayService, IScopedDependency
    {

        public async Task<ServiceResult<PaymentInvoiceDTO>> CreatePaymentLink(long trackingId, long amount, string callBackUrl)
        {
            if (amount <= 0)
                return ServiceResult.BadRequest<PaymentInvoiceDTO>("مبلغ قابل پرداخت برای ورود بع درگاه باید بزرگ تر از صفر باشد");

            /*            var invoice = new Invoice()
                        {
                            Amount = amount * 10,
                            CallbackUrl = new CallbackUrl(callBackUrl),
                            GatewayName = "IranKish",
                            TrackingNumber = trackingId
                        };

                        logger.LogInformation("start creating gateway invoice : {@(invoice)}", invoice);
            */
            //  var paymentRequestResult = await onlinePayment.RequestAsync(invoice);
            var paymentRequestResult = await onlinePayment.RequestAsync(invoice =>
            {
                invoice
                    .SetTrackingNumber((long)trackingId)
                .SetAmount(amount * 10)
                    .SetCallbackUrl(new CallbackUrl(callBackUrl))
                    .SetGateway("ZarinPal")
                    .SetZarinPalData(new ZarinPalInvoice(
                        description: "توضیحات پرداخت",
                        email: "",
                        mobile: ""
                    ));
            });
            logger.LogInformation("create gateway invoice  result: {@(paymentRequestResult)}", paymentRequestResult);


            if (paymentRequestResult != null && paymentRequestResult.IsSucceed)
            {

                var paymentLink = paymentRequestResult.GatewayTransporter.Descriptor.Url;


                logger.LogInformation("payment link created paymentLink: {paymentLink}", paymentLink);
                return ServiceResult.Ok<PaymentInvoiceDTO>(new PaymentInvoiceDTO
                {
                    Link = paymentLink,
                    TokenRef = paymentRequestResult.GatewayTransporter.Descriptor.Form?.Where(s => s.Key == "tokenIdentity").Select(s => s.Value).FirstOrDefault(),
                });
            }
            else
            {
                logger.LogInformation("payment link creation failed reason: {paymentLink}", paymentRequestResult.Message);
                return ServiceResult.BadRequest<PaymentInvoiceDTO>($"خطا در ایجاد لینک پرداخت از درگاه علت : {paymentRequestResult.Message}");
            }
        }

        public async Task<ServiceResult<VerifyPaymentResultDTO>> VerifyPayment(long trackId)
        {

            logger.LogInformation("start fetching invoice with trackId: {trackId}", trackId);

            var paymentFetchResult = await onlinePayment.FetchAsync(trackId);

            logger.LogInformation("invoice fetched invoice: {@(paymentFetchResult)}", paymentFetchResult);

            if (paymentFetchResult != null)
            {
                logger.LogInformation("invoice fetched success: {@(paymentFetchResult)}", paymentFetchResult);
                logger.LogInformation("start verify fetched invoice: {@(paymentFetchResult)}", paymentFetchResult);



                var result = await onlinePayment.VerifyAsync(paymentFetchResult);



                logger.LogInformation("verify invoice result: {@(result)}", result);

                if (result != null && result.Status == PaymentVerifyResultStatus.Succeed)
                {
                    var payment = paymentRepository.Table.Where(t => t.TrackingNumber == trackId).FirstOrDefault();



                    logger.LogInformation("invoice verified success with result: {@(result)}", result);
                    return ServiceResult.Ok(new VerifyPaymentResultDTO
                    {
                        ReferenceNumber = result.TransactionCode,
                    });
                }
                else
                    return ServiceResult.BadRequest<VerifyPaymentResultDTO>("پرداخت نامعتبر");
            }



            else
                return ServiceResult.BadRequest<VerifyPaymentResultDTO>("پرداخت نامعتبر");

        }

    }
}
