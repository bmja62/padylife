using Application.Cqrs.Commands;
using FluentValidation;
using Services;

namespace Application.Tickets.Commands.AnswerToTicket
{
    public class AnswerToTicketCommand(long ticketId, string content) : ICommand<ServiceResult>
    {

        public long TicketId { get; } = ticketId;
        public string Content { get; } = content;
    }

    public class AnswerToTicketCommandValidator : AbstractValidator<AnswerToTicketCommand>
    {
        public AnswerToTicketCommandValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Content)
            .NotEmpty().WithMessage("متن تیکت را وارد کنید")
            .MaximumLength(5000).WithMessage("متن تیکت نباید بیشتر از 5000 کاراکتر باشد");
        }
    }
}
