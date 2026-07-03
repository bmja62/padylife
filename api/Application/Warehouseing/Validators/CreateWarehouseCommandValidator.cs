using Application.Warehouseing.Commands;
using FluentValidation;

namespace Application.Warehouseing.Validators
{
    public class CreateWarehouseCommandValidator : AbstractValidator<CreateWarehouseCommand>
    {
        public CreateWarehouseCommandValidator()
        {
            RuleFor(x => x.Input.Name)
                .NotEmpty().WithMessage("نام انبار الزامی است")
                .MaximumLength(100).WithMessage("نام انبار نمی‌تواند بیشتر از 100 کاراکتر باشد");

            RuleFor(x => x.Input.Code)
                .NotEmpty().WithMessage("کد انبار الزامی است")
                .MaximumLength(20).WithMessage("کد انبار نمی‌تواند بیشتر از 20 کاراکتر باشد")
                .Matches("^[a-zA-Z0-9_-]*$").WithMessage("کد انبار فقط می‌تواند شامل حروف، اعداد، خط تیره و زیرخط باشد");

            RuleFor(x => x.Input.Address)
                .NotEmpty().WithMessage("آدرس انبار الزامی است")
                .MaximumLength(200).WithMessage("آدرس انبار نمی‌تواند بیشتر از 200 کاراکتر باشد");
        }
    }
}
