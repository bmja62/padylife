using Application.Cqrs.Commands;
using Application.Inventories.DTOs;
using Data.Contracts;
using Entities.Products;
using FluentValidation;
using Services;
using Services.Services.Warehousing.ProductStockServices;

namespace Application.Inventories.Commands
{
    public class ReserveStockCommand(ReserveStockDTO dto) : ICommand<ServiceResult>
    {
        public ReserveStockDTO Dto { get; } = dto;
    }

    public class ReserveStockCommandValidator : AbstractValidator<ReserveStockCommand>
    {
        public ReserveStockCommandValidator()
        {
            RuleFor(x => x.Dto.ProductId)
                .GreaterThan(0).WithMessage("شناسه محصول نامعتبر است");

            RuleFor(x => x.Dto.Quantity)
                .GreaterThan(0).WithMessage("مقدار باید بزرگتر از صفر باشد");

            RuleFor(x => x.Dto.WarehouseId)
                .GreaterThan(0).WithMessage("شناسه انبار نامعتبر است");

            RuleFor(x => x.Dto.ReferenceId)
                .NotEmpty().WithMessage("شناسه مرجع الزامی است")
                .MaximumLength(50).WithMessage("شناسه مرجع نمی‌تواند بیش از 50 کاراکتر باشد");
        }
    }

    public class ReserveStockCommandHandler(
        IProductStockService productStockService,
        IRepository<Product> productRepository,
        IRepository<ProductVariant> variantRepository)
        : ICommandHandler<ReserveStockCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(
            ReserveStockCommand request,
            CancellationToken cancellationToken)
        {
            // بررسی وجود محصول/واریانت
            if (request.Dto.VariantId.HasValue)
            {
                var variant = await variantRepository.GetByIdAsync(cancellationToken, request.Dto.VariantId.Value);
                if (variant == null)
                    return ServiceResult.NotFound("واریانت مورد نظر یافت نشد");
            }
            else
            {
                var product = await productRepository.GetByIdAsync(cancellationToken, request.Dto.ProductId);
                if (product == null)
                    return ServiceResult.NotFound("محصول مورد نظر یافت نشد");
            }

            try
            {
                await productStockService.ReserveStockForOrderAsync(
                    request.Dto.ProductId,
                    request.Dto.VariantId,
                    request.Dto.Quantity);

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(ex.Message);
            }
        }
    }
}