using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Commands
{
    public record CompleteUserPlanCommand(long userPlanId) : ICommand<ServiceResult>;

    public class CompleteUserPlanCommandHandler(IRepository<UserPlan> repository) : ICommandHandler<CompleteUserPlanCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CompleteUserPlanCommand request, CancellationToken cancellationToken)
        {
            var userPlanInDb = await repository.Table.FirstOrDefaultAsync(t => t.Id == request.userPlanId);

            if (userPlanInDb == null)
                return ServiceResult.NotFound("یافت نشد");

            if (userPlanInDb == null && userPlanInDb.IsCompleted)
                return ServiceResult.BadRequest("این پلن قبلا به پایان رسیده است");

            userPlanInDb.SetIsCompleted();
            await repository.UpdateAsync(userPlanInDb, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
