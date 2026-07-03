using Application.Warehouseing.Commands;
using FluentValidation;

namespace Application.Warehouseing.Validators
{
    public class AddZoneCommandValidator : AbstractValidator<AddZoneCommand>
    {
        public AddZoneCommandValidator()
        {
            RuleFor(x => x.WarehouseId)
                .GreaterThan(0).WithMessage("شناسه انبار نامعتبر است");

            RuleFor(x => x.Input.Name)
                .NotEmpty().WithMessage("نام منطقه الزامی است")
                .MaximumLength(50).WithMessage("نام منطقه نمی‌تواند بیشتر از 50 کاراکتر باشد");

            RuleFor(x => x.Input.Code)
                .NotEmpty().WithMessage("کد منطقه الزامی است")
                .MaximumLength(20).WithMessage("کد منطقه نمی‌تواند بیشتر از 20 کاراکتر باشد")
                .Matches("^[a-zA-Z0-9_-]*$").WithMessage("کد منطقه فقط می‌تواند شامل حروف، اعداد، خط تیره و زیرخط باشد");

            RuleFor(x => x.Input.Capacity)
                .GreaterThan(0).WithMessage("ظرفیت منطقه باید بیشتر از صفر باشد");
        }
    }
}
