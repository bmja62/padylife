using Application.Cqrs.Commands;
using Application.Points.Events;
using Application.Points.Extentions;
using Data.Contracts;
using Entities.Users;
using Entities.Wallets;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Services.WalletsServices;

namespace Application.Points.Commands
{
    public class ConvertPointsToWalletCreditCommand(long userId, int pointsToConvert, string description)
        : ICommand<ServiceResult>
    {
        public long UserId { get; } = userId;
        public int PointsToConvert { get; } = pointsToConvert;
        public string Description { get; } = description;
    }

    public class ConvertPointsToWalletCreditCommandHandler(
        IRepository<UserPoints> userPointsRepository,
        IWalletService walletService,
        IRepository<Wallet> walletRepository)
        : ICommandHandler<ConvertPointsToWalletCreditCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(ConvertPointsToWalletCreditCommand request, CancellationToken cancellationToken)
        {
            // دریافت اطلاعات امتیازات کاربر
            var userPoints = await userPointsRepository.Table
                .Include(up => up.PointTransactions)
                .FirstOrDefaultAsync(up => up.UserId == request.UserId, cancellationToken);

            if (userPoints == null)
                return ServiceResult.Fail(ServiceError.UserPointsNotFound);

            // دریافت کیف پول کاربر
            var wallet = await walletService.GetOrCreateByUserId(userPoints.UserId);

            if (wallet == null)
                return ServiceResult.Fail(ServiceError.WalletNotFound);

            // اعتبارسنجی
            if (request.PointsToConvert <= 0)
                return ServiceResult.Fail(ServiceError.InvalidPointsAmount);

            if (request.PointsToConvert > userPoints.AvailablePoints)
                return ServiceResult.Fail(ServiceError.InsufficientPoints);

            // محاسبه ارزش پولی
            decimal moneyValue = userPoints.CalculateMoneyValue(request.PointsToConvert);

            // ثبت تراکنش مصرف امتیاز
            userPoints.ConsumePoints(
                request.PointsToConvert,
                request.Description,
                referenceId: null,
                referenceType: default);

            // واریز به کیف پول
            wallet.Deposit(
                moneyValue,
                request.UserId,
                $"واریز بابت تبدیل {request.PointsToConvert} امتیاز");

            // ذخیره تغییرات
            await userPointsRepository.UpdateAsync(userPoints, cancellationToken);
            await walletRepository.UpdateAsync(wallet, cancellationToken);

            // ثبت رویداد دامنه
            userPoints.AddDomainEvent(new PointsConvertedToWalletCreditEvent(
                request.UserId,
                request.PointsToConvert,
                moneyValue,
                request.Description));
            return ServiceResult.Ok(new
            {
                ConvertedPoints = request.PointsToConvert,
                CreditAdded = moneyValue,
                RemainingPoints = userPoints.AvailablePoints
            });
        }
    }
}