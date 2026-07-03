//using Application.Cqrs.Commands;
//using Data.Repositories;
//using Entities.Wallets;
//using Services;
//using Services.Services.WalletsServices;

//namespace Application.Wallets.Commands.ChargeWalletByCoupon
//{
//    public class ChargeWalletByCouponCommandHandler
//        (IRepository<Wallet> repository,
//        IWalletService walletService,
//        IRepository<Coupon> couponRepository,
//        IRepository<UsedCoupon> usedCouponRepository,
//        ICouponService couponService)
//        : ICommandHandler<ChargeWalletByCouponCommand, ServiceResult>
//    {
//        public async Task<ServiceResult> Handle(ChargeWalletByCouponCommand request, CancellationToken cancellationToken)
//        {

//            var canUseResult = await couponService.CanUserUseCoupon(request.CouponCode, request.UserId);
//            if (!canUseResult.IsSuccess)
//                return canUseResult;

//            var coupon = canUseResult.Data;

//            if (coupon.ValueBehavior != CouponValueBehavior.Numeric)
//                return ServiceResult.BadRequest("این کد قابل استفاده بر روی کیف پول نیست");

//            if(coupon.MinOrderPrice.HasValue)
//                return ServiceResult.BadRequest("این کد قابل استفاده بر روی کیف پول نیست");

//            var wallet = await walletService.GetOrCreateByUserId(request.UserId);

//            wallet.Deposit(coupon.Value, request.UserId, $"افزایش شارژ کیف پول به دلیل استفاده از کد تخفیف {coupon.Code}");

//            var usedCoupon = new UsedCoupon(coupon.Id, request.UserId);
//            if (coupon.Count.HasValue)
//                coupon.Count -= 1;

//            coupon.UsedCount++;

//            await repository.UpdateAsync(wallet,cancellationToken);
//            await couponRepository.UpdateAsync(coupon, cancellationToken);
//            await usedCouponRepository.AddAsync(usedCoupon, cancellationToken);

//            return ServiceResult.Ok();
//        }
//    }
//}
