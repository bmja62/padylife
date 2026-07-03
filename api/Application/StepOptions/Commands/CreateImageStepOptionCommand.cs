using Application.Cqrs.Commands;
using Application.StepOptions.DTOs;
using Data.Contracts;
using Entities.StepOprions;
using Services;

namespace Application.StepOptions.Commands
{
    public class CreateImageStepOptionCommand : ICommand<ServiceResult>
    {
        public CreateImageStepOptionCommand(CreateImageStepOptionCommandDTO input)
        {
            Input = input;
        }

        public CreateImageStepOptionCommandDTO Input { get; }
    }
    public class CreateImageStepOptionCommandHandler : ICommandHandler<CreateImageStepOptionCommand, ServiceResult>
    {
        private readonly IRepository<StepOption> _stepOptionRepository;

        public CreateImageStepOptionCommandHandler(IRepository<StepOption> stepOptionRepository)
        {
            _stepOptionRepository = stepOptionRepository;
        }

        public async Task<ServiceResult> Handle(CreateImageStepOptionCommand request, CancellationToken cancellationToken)
        {
            var input = request.Input;

            var imageStepOption = new ImageStepOption
            {
                StepId = input.StepId,
                Title = input.Title,
                Description = input.Description,
                Order = input.Order,
                ImageUrl = input.ImageUrl,
                AltText = input.AltText,
                Caption = input.Caption,
            };

            await _stepOptionRepository.AddAsync(imageStepOption, cancellationToken);

            return ServiceResult.Ok();
        }
    }

}
