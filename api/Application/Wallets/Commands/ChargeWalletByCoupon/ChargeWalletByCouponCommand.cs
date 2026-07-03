using Application.Cqrs.Commands;
using Services;

namespace Application.Wallets.Commands.ChargeWalletByCoupon
{
    public class ChargeWalletByCouponCommand(long userId, string couponCode) : ICommand<ServiceResult>
    {
        public long UserId { get; } = userId;
        public string CouponCode { get; } = couponCode;
    }
}
