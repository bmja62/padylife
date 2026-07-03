using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Commands
{
    public record CompletedUserExerciseCommand(long userId, long exerciseId, long userPlanId) : ICommand<ServiceResult>;
    public class CompletedUserExerciseCommandHandler(IRepository<UserExercise> repository) : ICommandHandler<CompletedUserExerciseCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CompletedUserExerciseCommand request, CancellationToken cancellationToken)
        {
            var userExcerciseInDb = await repository.Table
                .Where(t =>
                t.UserId == request.userId &&
                t.ExerciseId == request.exerciseId &&
                t.UserPlanId == request.userPlanId
                ).FirstOrDefaultAsync(cancellationToken);

            if (userExcerciseInDb is null)
                return ServiceResult.NotFound("یافت نشد");

            if (userExcerciseInDb.IsCompleted)
                return ServiceResult.BadRequest("قبلا به اتمام رسیده");

            userExcerciseInDb.SetIsCompleted();

            await repository.UpdateAsync(userExcerciseInDb, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
