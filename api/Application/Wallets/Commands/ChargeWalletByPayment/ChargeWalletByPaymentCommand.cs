using Application.Cqrs.Commands;
using Services;
using Services.Services.PaymentServices.DTOs;

namespace Application.Wallets.Commands.ChargeWalletByPayment
{
    public class ChargeWalletByPaymentCommand(long userId
    , long amount) : ICommand<ServiceResult<PaymentInvoiceDTO>>
    {
        public long UserId { get; } = userId;
        public long Amount { get; } = amount;
    }
}
