using Application.Cqrs.Commands;
using Application.StepOptions.DTOs;
using Data.Contracts;
using Entities.Excersies;
using Entities.StepOprions;
using Services;

namespace Application.StepOptions.Commands
{
    public class CreateMultipleChoiceStepOptionCommand : ICommand<ServiceResult>
    {
        public CreateMultipleChoiceStepOptionCommand(CreateMultipleChoiceStepOptionCommandDTO input)
        {
            Input = input;
        }

        public CreateMultipleChoiceStepOptionCommandDTO Input { get; }
    }

    public class CreateMultipleChoiceStepOptionCommandHandler : ICommandHandler<CreateMultipleChoiceStepOptionCommand, ServiceResult>
    {
        private readonly IRepository<MultipleChoiceStepOption> _optionRepository;
        private readonly IRepository<OptionChoice> _choiceRepository;

        public CreateMultipleChoiceStepOptionCommandHandler(
            IRepository<MultipleChoiceStepOption> optionRepository,
            IRepository<OptionChoice> choiceRepository)
        {
            _optionRepository = optionRepository;
            _choiceRepository = choiceRepository;
        }

        public async Task<ServiceResult> Handle(CreateMultipleChoiceStepOptionCommand request, CancellationToken cancellationToken)
        {
            var input = request.Input;

            var entity = new MultipleChoiceStepOption
            {
                StepId = input.StepId,
                Title = input.Title,
                Description = input.Description,
                Order = input.Order,
                AllowMultipleSelection = input.AllowMultipleSelection,
                CorrectAnswerHint = input.CorrectAnswerHint
            };

            await _optionRepository.AddAsync(entity, cancellationToken);

            foreach (var choiceDto in input.Choices)
            {
                var choice = new OptionChoice
                {
                    StepOptionId = entity.Id,
                    Text = choiceDto.Text,
                    IsCorrect = choiceDto.IsCorrect,
                    Order = choiceDto.Order
                };
                await _choiceRepository.AddAsync(choice, cancellationToken);
            }

            return ServiceResult.Ok();
        }
    }
}
