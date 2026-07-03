using Application.Cqrs.Commands;
using Application.Excersies.DTOs;
using Data.Contracts;
using Entities.Excersies;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Excersies.Commands
{
    public class AddStepToExcersiesCommand(AddOrRemoveStepToOrFromExcersiesCommandDTO input) : ICommand<ServiceResult>
    {
        public AddOrRemoveStepToOrFromExcersiesCommandDTO Input { get; } = input;
    }

    public class AddStepToExcersieCommandHandler(IRepository<Exercise> exerciseRepository, IRepository<Step> stepRepository) : ICommandHandler<AddStepToExcersiesCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(AddStepToExcersiesCommand request, CancellationToken cancellationToken)
        {
            var exerciseInDb = await exerciseRepository.Table.Include(t => t.ExerciseSteps).Where(t => request.Input.ExcersiesId == t.Id).FirstOrDefaultAsync();
            var stepInDb = await stepRepository.Table.Where(t => request.Input.StepId == t.Id).FirstOrDefaultAsync();

            if (exerciseInDb is null)
                return ServiceResult.NotFound("تمرین یافت نشد");

            if (stepInDb is null)
                return ServiceResult.NotFound("مرحله یافت نشد");

            if (exerciseInDb.ExerciseSteps != null && exerciseInDb.ExerciseSteps.Any() && exerciseInDb.ExerciseSteps.Count > 0)
                if (exerciseInDb.ExerciseSteps.Count >= exerciseInDb.ExerciseCount)
                    return ServiceResult.BadRequest("تعداد تمرین ها از پیش فرض تعریف شده نباید بیشتر شود");


            if (exerciseInDb.ExerciseSteps.Any(t => t.StepId == stepInDb.Id))
                return ServiceResult.BadRequest("این مرحله در این تمرین وجود دارد");

            exerciseInDb.AddStep(ExerciseStep.CreateByStepId(stepInDb.Id));
            await exerciseRepository.UpdateAsync(exerciseInDb, cancellationToken);

            return ServiceResult.Ok();
        }

    }


}
