using Application.Cqrs.Commands;
using Application.Inventories.DTOs;
using Data.Contracts;
using Entities.Products;
using FluentValidation;
using Services;
using Services.Services.StockManagerServices;

namespace Application.Inventories.Commands
{
    public class IncreaseStockCommand(AdjustStockDTO dto) : ICommand<ServiceResult>
    {
        public AdjustStockDTO Dto { get; } = dto;
    }

    public class IncreaseStockCommandValidator : AbstractValidator<IncreaseStockCommand>
    {
        public IncreaseStockCommandValidator()
        {
            RuleFor(x => x.Dto.ProductId)
                .GreaterThan(0).WithMessage("شناسه محصول نامعتبر است");

            RuleFor(x => x.Dto.Quantity)
                .GreaterThan(0).WithMessage("مقدار باید بزرگتر از صفر باشد");

            RuleFor(x => x.Dto.WarehouseId)
                .GreaterThan(0).WithMessage("شناسه انبار نامعتبر است");

            RuleFor(x => x.Dto.Reason)
                .NotEmpty().WithMessage("علت افزایش موجودی الزامی است")
                .MaximumLength(500).WithMessage("علت نمی‌تواند بیش از 500 کاراکتر باشد");
        }
    }

    public class IncreaseStockCommandHandler(
        IStockManagerService stockManagerService,
        IRepository<Product> productRepository,
        IRepository<ProductVariant> variantRepository)
        : ICommandHandler<IncreaseStockCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(
            IncreaseStockCommand request,
            CancellationToken cancellationToken)
        {
            // بررسی وجود محصول/واریانت
            if (request.Dto.VariantId.HasValue)
            {
                var variant = await variantRepository.GetByIdAsync(cancellationToken, request.Dto.VariantId.Value);
                if (variant == null)
                    return ServiceResult.NotFound("واریانت مورد نظر یافت نشد");

                await stockManagerService.IncreaseStockAsync(
                    variant,
                    request.Dto.Quantity,
                    request.Dto.WarehouseId,
                    request.Dto.Reason);
            }
            else
            {
                var product = await productRepository.GetByIdAsync(cancellationToken, request.Dto.ProductId);
                if (product == null)
                    return ServiceResult.NotFound("محصول مورد نظر یافت نشد");

                await stockManagerService.IncreaseStockAsync(
                    product,
                    request.Dto.Quantity,
                    request.Dto.WarehouseId,
                    request.Dto.Reason);
            }

            return ServiceResult.Ok();
        }
    }
}