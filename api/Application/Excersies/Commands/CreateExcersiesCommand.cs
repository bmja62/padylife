using Application.Cqrs.Commands;
using Application.Excersies.DTOs;
using Common.Utilities;
using Data.Contracts;
using Entities.Excersies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Excersies.Commands
{
    public class CreateExcersiesCommand(CreateExcersiesCommandDTO input) : ICommand<ServiceResult>
    {
        public CreateExcersiesCommandDTO Input { get; } = input;
    }

    public class CreateExcersiesCommandHandler(IRepository<Exercise> exerciseRepository, IRepository<ExerciseCategory> exerciseCategoryRepository, IRepository<Step> stepRepository,IHttpContextAccessor httpContextAccessor) : ICommandHandler<CreateExcersiesCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CreateExcersiesCommand request, CancellationToken cancellationToken)
        {
            var creator = httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();
            var title = request.Input.Title;
            var imageUrl = request.Input.ImageUrl;
            var exerciseType = request.Input.ExerciseType;
            var documentLink = request.Input.DocumentLink;
            var exerciseGoal = request.Input.ExerciseGoal;
            var practiceMethod = request.Input.PracticeMethod;
            var exerciseCount = request.Input.ExerciseCount;
            var exerciseEstimate = request.Input.ExerciseEstimate;
            var exerciseCategoryInDb = await exerciseCategoryRepository.Table.Where(t => t.Id == request.Input.ExerciseCategoryId).FirstOrDefaultAsync();

            if (exerciseCategoryInDb is null)
                return ServiceResult.NotFound("دسته بندی تمرین یافت نشد");

            var exerciseCategoryId = exerciseCategoryInDb.Id;

            var stepsInDb = await stepRepository.Table.Where(t => request.Input.StepIds.Contains(t.Id)).ToListAsync();
            if (stepsInDb is null && stepsInDb.Count == 0)
                return ServiceResult.NotFound("مرحله ای یافت نشد");

            var exerciseSteps = stepsInDb.Select(t => ExerciseStep.CreateByStepId(t.Id)).ToList();

            await exerciseRepository.AddAsync(
                Exercise.CreateDefault(
                        title,
                        imageUrl,
                        exerciseType,
                        documentLink,
                        exerciseGoal,
                        practiceMethod,
                        exerciseCount,
                        exerciseEstimate,
                        exerciseCategoryId,
                        creator,
                        exerciseSteps
                    )
                , cancellationToken);
            return ServiceResult.Ok();
        }
    }
}
