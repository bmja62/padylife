using Application.Warehouseing.Commands;
using FluentValidation;

namespace Application.Warehouseing.Validators
{
    public class DeactivateWarehouseCommandValidator : AbstractValidator<DeactivateWarehouseCommand>
    {
        public DeactivateWarehouseCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("شناسه انبار نامعتبر است");
        }
    }
}
