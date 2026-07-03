using Application.Cqrs.Commands;
using Application.StepOptions.DTOs;
using Data.Contracts;
using Entities.StepOprions;
using Services;

namespace Application.StepOptions.Commands
{
    public class CreateVideoStepOptionCommand : ICommand<ServiceResult>
    {
        public CreateVideoStepOptionCommand(CreateVideoStepOptionCommandDTO input)
        {
            Input = input;
        }

        public CreateVideoStepOptionCommandDTO Input { get; }
    }

    public class CreateVideoStepOptionCommandHandler : ICommandHandler<CreateVideoStepOptionCommand, ServiceResult>
    {
        private readonly IRepository<StepOption> _stepOptionRepository;

        public CreateVideoStepOptionCommandHandler(IRepository<StepOption> stepOptionRepository)
        {
            _stepOptionRepository = stepOptionRepository;
        }

        public async Task<ServiceResult> Handle(CreateVideoStepOptionCommand request, CancellationToken cancellationToken)
        {
            var input = request.Input;

            var videoStepOption = new VideoStepOption
            {
                StepId = input.StepId,
                Title = input.Title,
                Description = input.Description,
                Order = input.Order,
                VideoUrl = input.VideoUrl,
                ThumbnailUrl = input.ThumbnailUrl,
                Duration = input.Duration,
                AllowDownload = input.AllowDownload
            };

            await _stepOptionRepository.AddAsync(videoStepOption, cancellationToken);

            return ServiceResult.Ok();
        }
    }


}
