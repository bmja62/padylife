using FluentValidation;

namespace Application.Tickets.Commands.CreateTicket
{
    public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
    {
        public CreateTicketCommandValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("عنوان تیکت را وارد کنید")
                .MaximumLength(200).WithMessage("عنوان تیکت نباید بیشتر از 5000 کاراکتر باشد");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("متن تیکت را وارد کنید")
                .MaximumLength(5000).WithMessage("متن تیکت نباید بیشتر از 5000 کاراکتر باشد");
        }
    }

}
