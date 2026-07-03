using Application.Cqrs.Commands;
using Application.StepOptions.DTOs;
using Data.Contracts;
using Entities.StepOprions;
using Services;

namespace Application.StepOptions.Commands
{
    public class CreateTaskStepOptionCommand : ICommand<ServiceResult>
    {
        public CreateTaskStepOptionCommand(CreateTaskStepOptionCommandDTO input)
        {
            Input = input;
        }

        public CreateTaskStepOptionCommandDTO Input { get; }
    }
    public class CreateTaskStepOptionCommandHandler : ICommandHandler<CreateTaskStepOptionCommand, ServiceResult>
    {
        private readonly IRepository<StepOption> _stepOptionRepository;

        public CreateTaskStepOptionCommandHandler(IRepository<StepOption> stepOptionRepository)
        {
            _stepOptionRepository = stepOptionRepository;
        }

        public async Task<ServiceResult> Handle(CreateTaskStepOptionCommand request, CancellationToken cancellationToken)
        {
            var input = request.Input;

            var taskStepOption = new TaskStepOption
            {
                StepId = input.StepId,
                Title = input.Title,
                Description = input.Description,
                Order = input.Order,
                DeadlineDays = input.DeadlineDays,
                AssigneeRole = input.AssigneeRole,
                TaskInstructions = input.TaskInstructions,
                EstimatedMinutes = input.EstimatedMinutes
            };

            await _stepOptionRepository.AddAsync(taskStepOption, cancellationToken);

            return ServiceResult.Ok();
        }
    }

}
