using Application.Cqrs.Commands;
using Application.Inventories.DTOs;
using Data.Contracts;
using Entities.Products;
using FluentValidation;
using Services;
using Services.Services.StockManagerServices;
using Services.Services.Warehousing.WarehouseServices;

namespace Application.Inventories.Commands
{
    public class TransferStockCommand(TransferStockDTO dto) : ICommand<ServiceResult>
    {
        public TransferStockDTO Dto { get; } = dto;
    }

    public class TransferStockCommandValidator : AbstractValidator<TransferStockCommand>
    {
        public TransferStockCommandValidator()
        {
            RuleFor(x => x.Dto.ProductId)
                .GreaterThan(0).WithMessage("شناسه محصول نامعتبر است");

            RuleFor(x => x.Dto.Quantity)
                .GreaterThan(0).WithMessage("مقدار باید بزرگتر از صفر باشد");

            RuleFor(x => x.Dto.FromWarehouseId)
                .GreaterThan(0).WithMessage("شناسه انبار مبدأ نامعتبر است");

            RuleFor(x => x.Dto.ToWarehouseId)
                .GreaterThan(0).WithMessage("شناسه انبار مقصد نامعتبر است")
                .NotEqual(x => x.Dto.FromWarehouseId)
                .WithMessage("انبار مبدأ و مقصد نمی‌توانند یکسان باشند");

            RuleFor(x => x.Dto.Reason)
                .NotEmpty().WithMessage("علت انتقال الزامی است")
                .MaximumLength(500).WithMessage("علت نمی‌تواند بیش از 500 کاراکتر باشد");
        }
    }

    public class TransferStockCommandHandler(
        IWarehouseService warehouseService,
        IStockManagerService stockManagerService,
        IRepository<Product> productRepository,
        IRepository<ProductVariant> variantRepository)
        : ICommandHandler<TransferStockCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(
            TransferStockCommand request,
            CancellationToken cancellationToken)
        {
            // بررسی وجود انبارها
            var fromWarehouse = await warehouseService.GetWarehouseAsync(request.Dto.FromWarehouseId);
            var toWarehouse = await warehouseService.GetWarehouseAsync(request.Dto.ToWarehouseId);

            if (fromWarehouse == null || toWarehouse == null)
                return ServiceResult.BadRequest("انبار مبدأ یا مقصد یافت نشد");

            // بررسی وجود محصول/واریانت
            if (request.Dto.VariantId.HasValue)
            {
                var variant = await variantRepository.GetByIdAsync(cancellationToken, request.Dto.VariantId.Value);
                if (variant == null)
                    return ServiceResult.NotFound("واریانت مورد نظر یافت نشد");

                // بررسی موجودی کافی در انبار مبدأ
                var hasStock = await stockManagerService.HasStockAsync(
                    variant,
                    request.Dto.Quantity,
                    request.Dto.FromWarehouseId);

                if (!hasStock)
                    return ServiceResult.NotFound("موجودی کافی در انبار مبدأ نیست");

                await warehouseService.TransferStockAsync(
                    variant.ProductId,
                    variant.Id,
                    request.Dto.FromWarehouseId,
                    request.Dto.ToWarehouseId,
                    request.Dto.Quantity,
                    request.Dto.Reason);
            }
            else
            {
                var product = await productRepository.GetByIdAsync(cancellationToken, request.Dto.ProductId);
                if (product == null)
                    return ServiceResult.NotFound("محصول مورد نظر یافت نشد");

                // بررسی موجودی کافی در انبار مبدأ
                var hasStock = await stockManagerService.HasStockAsync(
                    product,
                    request.Dto.Quantity,
                    request.Dto.FromWarehouseId);

                if (!hasStock)
                    return ServiceResult.BadRequest("موجودی کافی در انبار مبدأ نیست");

                await warehouseService.TransferStockAsync(
                    request.Dto.ProductId,
                    null,
                    request.Dto.FromWarehouseId,
                    request.Dto.ToWarehouseId,
                    request.Dto.Quantity,
                    request.Dto.Reason);
            }

            return ServiceResult.Ok();
        }
    }
}