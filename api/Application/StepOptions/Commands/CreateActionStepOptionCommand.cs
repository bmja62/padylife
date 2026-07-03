using Application.Cqrs.Commands;
using Application.StepOptions.DTOs;
using Data.Contracts;
using Entities.StepOprions;
using Services;

namespace Application.StepOptions.Commands
{
    public class CreateActionStepOptionCommand : ICommand<ServiceResult>
    {
        public CreateActionStepOptionCommand(CreateActionStepOptionCommandDTO input)
        {
            Input = input;
        }

        public CreateActionStepOptionCommandDTO Input { get; }
    }
    public class CreateActionStepOptionCommandHandler : ICommandHandler<CreateActionStepOptionCommand, ServiceResult>
    {
        private readonly IRepository<StepOption> _stepOptionRepository;

        public CreateActionStepOptionCommandHandler(IRepository<StepOption> stepOptionRepository)
        {
            _stepOptionRepository = stepOptionRepository;
        }

        public async Task<ServiceResult> Handle(CreateActionStepOptionCommand request, CancellationToken cancellationToken)
        {
            var input = request.Input;

            var actionStepOption = new ActionStepOption
            {
                StepId = input.StepId,
                Title = input.Title,
                Description = input.Description,
                Order = input.Order,
                ActionCommand = input.ActionCommand,
                ActionParameters = input.ActionParameters,
                RequiresConfirmation = input.RequiresConfirmation
            };

            await _stepOptionRepository.AddAsync(actionStepOption, cancellationToken);

            return ServiceResult.Ok();
        }
    }

}
