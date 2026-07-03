using Application.Cqrs.Commands;
using Common.Utilities;
using Data.Contracts;
using Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Commands
{
    public record DeleteUserPlanCompanionCommand(long UserPlanCompanionId)
     : ICommand<ServiceResult>;
    public class DeleteUserPlanCompanionHandler(
    IRepository<UserPlanCompanion> companionRepo,
    IRepository<UserPlan> planRepo,
    IHttpContextAccessor accessor
) : ICommandHandler<DeleteUserPlanCompanionCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(DeleteUserPlanCompanionCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = accessor.HttpContext.User.Identity.GetUserId<long>();

            var companion = await companionRepo.Table
                .Include(c => c.UserPlan)
                .FirstOrDefaultAsync(c => c.Id == request.UserPlanCompanionId, cancellationToken);

            if (companion == null)
                return ServiceResult.NotFound("همراه مورد نظر یافت نشد.");


            if (companion.UserPlan.UserId != currentUserId)
                return ServiceResult.BadRequest("شما مجاز به حذف این همراه نیستید.");

            await companionRepo.SoftDeleteAsync(companion, cancellationToken);


            return ServiceResult.Ok("همراه با موفقیت حذف شد.");
        }
    }

}
