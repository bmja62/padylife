using Application.Cqrs.Commands;
using Services;
using Services.Services.PaymentServices;
using Services.Services.PaymentServices.DTOs;

namespace Application.Wallets.Commands.ChargeWalletByPayment
{
    public class ChargeWalletByPaymentCommandHandler(
        IPaymentService paymentService) : ICommandHandler<ChargeWalletByPaymentCommand, ServiceResult<PaymentInvoiceDTO>>
    {
        public async Task<ServiceResult<PaymentInvoiceDTO>> Handle(ChargeWalletByPaymentCommand request, CancellationToken cancellationToken)
        {
            //   return ServiceResult.BadRequest<PaymentInvoiceDTO>("خطای ارتباط با درگاه بانکی");
            if (request.Amount < 1000)
                return ServiceResult.BadRequest<PaymentInvoiceDTO>("مبلغ پرداختی باید بزرگ تر از 1000 تومان باشد");

            var paymentLink = await paymentService.GetWalletChargePaymentLink(request.Amount, request.UserId);
            return paymentLink;
        }
    }
}
