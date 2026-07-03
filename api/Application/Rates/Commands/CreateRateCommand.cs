using Application.Cqrs.Commands;
using Application.Cqrs.Events;
using Application.Rates.Events;
using Common.Utilities;
using Data.Contracts;
using Entities.Users;
using Microsoft.AspNetCore.Http;
using Services;
using Services.Services.CommentServices.DTOs;
using Services.Services.RateServices;

namespace Application.Rates.Commands
{
    public class CreateRateCommand(CreateRateDTO input) : ICommand<ServiceResult>
    {
        public CreateRateDTO Input { get; } = input;
    }

    public class CreateRateCommandHandler(IRateService rateService, IHttpContextAccessor accessor, IRepository<UserPlan> UserPlanReposiory, IDomainEventDispatcher eventDispatcher) : ICommandHandler<CreateRateCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CreateRateCommand request, CancellationToken cancellationToken)
        {
            var userId = accessor.HttpContext.User.Identity.GetUserId<long>();
            if (userId <= 0)
                return ServiceResult.Fail("شناسه کاربری یافت نشد");

            var domainEvent = new RateAddedEvent(userId, request.Input);
            await eventDispatcher.DispatchAsync(new[] { domainEvent }, cancellationToken);

            return ServiceResult.Ok();
        }

    }
}
