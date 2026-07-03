using Application.Cqrs.Commands;
using Application.StepOptions.DTOs;
using Data.Contracts;
using Entities.StepOprions;
using Services;

namespace Application.StepOptions.Commands
{
    public class CreateTextStepOptionCommand : ICommand<ServiceResult>
    {
        public CreateTextStepOptionCommand(CreateTextStepOptionCommandDTO input)
        {
            Input = input;
        }

        public CreateTextStepOptionCommandDTO Input { get; }
    }

    public class CreateTextStepOptionCommandHandler : ICommandHandler<CreateTextStepOptionCommand, ServiceResult>
    {
        private readonly IRepository<TextStepOption> _repository;

        public CreateTextStepOptionCommandHandler(IRepository<TextStepOption> repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult> Handle(CreateTextStepOptionCommand request, CancellationToken cancellationToken)
        {
            var input = request.Input;

            var entity = new TextStepOption
            {
                StepId = input.StepId,
                Title = input.Title,
                Description = input.Description,
                Order = input.Order,
                Content = input.Content,
                IsHtml = input.IsHtml,
                TextFormat = input.TextFormat
            };

            await _repository.AddAsync(entity, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
