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
    public class UpdateExcersiesCommand(UpdateExcersiesCommandDTO input) : ICommand<ServiceResult>
    {
        public UpdateExcersiesCommandDTO Input { get; } = input;
    }

    public class UpdateExcersiesCommandHandler(IRepository<Exercise> exerciseRepository, IHttpContextAccessor httpContextAccessor) : ICommandHandler<UpdateExcersiesCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(UpdateExcersiesCommand request, CancellationToken cancellationToken)
        {
            var exerciseInDb = await exerciseRepository.Table.Where(t => t.Id == request.Input.Id).FirstOrDefaultAsync(cancellationToken);
            var creator = httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();
            exerciseInDb.SetTitle(request.Input.Title);
            exerciseInDb.SetExerciseType(request.Input.ExerciseType);
            exerciseInDb.SetDocumentLink(request.Input.DocumentLink);
            exerciseInDb.SetExerciseGoal(request.Input.ExerciseGoal);
            exerciseInDb.SetPracticeMethod(request.Input.PracticeMethod);
            exerciseInDb.SetExerciseCount(request.Input.ExerciseCount);
            exerciseInDb.SetExerciseEstimate(request.Input.ExerciseEstimate);
            exerciseInDb.SetExerciseCategoryInDb(request.Input.ExerciseCategoryId);
            exerciseInDb.SetImageUrl(request.Input.ImageUrl);
            exerciseInDb.SetCreateByUserId(creator);

            await exerciseRepository.UpdateAsync(exerciseInDb, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
