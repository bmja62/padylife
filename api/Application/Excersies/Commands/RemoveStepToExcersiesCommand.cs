using Application.Cqrs.Commands;
using Application.Excersies.DTOs;
using Data.Contracts;
using Entities.Excersies;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Excersies.Commands
{
    public class RemoveStepToExcersiesCommand(AddOrRemoveStepToOrFromExcersiesCommandDTO input) : ICommand<ServiceResult>
    {
        public AddOrRemoveStepToOrFromExcersiesCommandDTO Input { get; } = input;
    }

    public class RemoveStepToExcersiesCommandHandler(IRepository<ExerciseStep> exerciseStepRepository) : ICommandHandler<RemoveStepToExcersiesCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(RemoveStepToExcersiesCommand request, CancellationToken cancellationToken)
        {
            var exerciseStepInDb = await exerciseStepRepository.Table.Where(t => t.StepId == request.Input.StepId && t.ExerciseId == request.Input.ExcersiesId).FirstOrDefaultAsync();
            if (exerciseStepInDb == null)
                return ServiceResult.NotFound("این مرحله به این تمرین تعلق ندارد");

            await exerciseStepRepository.DeleteAsync(exerciseStepInDb, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
