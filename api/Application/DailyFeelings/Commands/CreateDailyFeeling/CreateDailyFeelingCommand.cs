using Application.Cqrs.Commands;
using Application.DailyFeelings.DTOs;
using Common.Utilities;
using Data.Contracts;
using Entities.DailyFeelings;
using Microsoft.AspNetCore.Http;
using Services;

namespace Application.DailyFeelings.Commands.CreateDailyFeeling
{
    public class CreateDailyFeelingCommand : ICommand<ServiceResult>
    {
        public CreateDailyFeelingCommand(CreateDailyFeelingCommandDTO input)
        {
            Input = input;
        }

        public CreateDailyFeelingCommandDTO Input { get; }
    }

    public class CreateDailyFeelingCommandHandler(IRepository<DailyFeeling> dailyFeelingRepository, IHttpContextAccessor accessor) : ICommandHandler<CreateDailyFeelingCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CreateDailyFeelingCommand request, CancellationToken cancellationToken)
        {
            var userId = accessor.HttpContext.User.Identity.GetUserId<long>();
            if (userId <= 0)
                return ServiceResult.Fail("شناسه کاربری فرد یافت نشد");
            await dailyFeelingRepository.AddAsync(
                DailyFeeling.CreateDefault(request.Input.Type, request.Input.Description, request.Input.VoiceUrl, userId)
                , cancellationToken);
            return ServiceResult.Ok();
        }
    }
}
