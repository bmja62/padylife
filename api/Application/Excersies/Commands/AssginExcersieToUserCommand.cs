using Application.Cqrs.Commands;
using Common.Utilities;
using Data.Contracts;
using Entities.Questions;
using Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Excersies.Commands
{
    public class AssginExcersieToUserCommand(long questionLinkedId, long userPlanId) : ICommand<ServiceResult>
    {
        public long QuestionLinkedId { get; } = questionLinkedId;
        public long UserPlanId { get; } = userPlanId;
    }

    public class AssginExcersieToUserCommandHandler(IRepository<User> userRepository, IRepository<QuestionLinked> questionLinkedRepository, IHttpContextAccessor accessor) : ICommandHandler<AssginExcersieToUserCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(AssginExcersieToUserCommand request, CancellationToken cancellationToken)
        {

            var userId = accessor.HttpContext.User.Identity.GetUserId<long>();
            var userInDb = await userRepository.Table
                .Include(t => t.UserExercises)
                .Where(t =>
                t.Id == userId &&
                t.UserPlans.Any(t => t.Id == request.UserPlanId)
                ).FirstOrDefaultAsync();

            if (userInDb is null)
                return ServiceResult.NotFound("یافت نشد");

            var excersies = await questionLinkedRepository.Table.Include(t => t.ExerciseLinks).ThenInclude(t => t.Exercise)
                .Where(t => t.Id == request.QuestionLinkedId)
                .SelectMany(t => t.ExerciseLinks)
                .Select(t => t.Exercise)
                .ToListAsync(cancellationToken);

            foreach (var excersie in excersies)
            {
                userInDb.AddExcersie(
                UserExercise.Create(userId, excersie.Id, request.UserPlanId)
                );
            }

            await userRepository.UpdateAsync(userInDb, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
