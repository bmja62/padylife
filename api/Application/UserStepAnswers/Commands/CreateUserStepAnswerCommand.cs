using Application.Cqrs.Commands;
using Application.Points.Events;
using Application.Points.Extentions;
using Application.UserStepAnswers.DTOs;
using Application.UserStepAnswers.Events;
using Common.Utilities;
using Data.Contracts;
using Entities.Common;
using Entities.Excersies;
using Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.UserStepAnswers.Commands
{
    public class CreateUserStepAnswerCommand : ICommand<ServiceResult>
    {
        public CreateUserStepAnswerCommand(CreateUserStepAnswerCommandDTO input)
        {
            Input = input;
        }

        public CreateUserStepAnswerCommandDTO Input { get; }
    }

    public class CreateUserStepAnswerCommandHandler(
        IRepository<UserPlanExcersiesAnswer> answerRepo,
        IRepository<UserSelectedChoice> selectedChoiceRepo,
        IRepository<OptionChoice> choiceRepo,
        IHttpContextAccessor accessor
        ) : ICommandHandler<CreateUserStepAnswerCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CreateUserStepAnswerCommand request, CancellationToken cancellationToken)
        {
            var userId = accessor.HttpContext.User.Identity.GetUserId<long>();
            if (userId <= 0)
                return ServiceResult.Fail("کاربر شناسایی نشد");

            var input = request.Input;

            var answer = new UserPlanExcersiesAnswer
            {
                UserPlanId = input.UserPlanId,
                ExcersieId = input.ExcersieId,
                StepId = input.StepId,
                SelectedStepOptionId = input.SelectedStepOptionId,
                Text = input.Text,
                ImageUrl = input.ImageUrl
            };

            if (await answerRepo.Table.AnyAsync(t => t.UserPlanId == input.UserPlanId && t.ExcersieId == input.ExcersieId && t.StepId == input.StepId))
            {
                return ServiceResult.BadRequest("شما قبلا در این مرحله شرکت کرده اید");
            }

            await answerRepo.AddAsync(answer, cancellationToken);

            if (input.SelectedChoiceIds?.Any() == true)
            {
                var selectedChoices = input.SelectedChoiceIds.Select(choiceId => new UserSelectedChoice
                {
                    UserPlanExcersiesAnswerId = answer.Id,
                    OptionChoiceId = choiceId
                });

                await selectedChoiceRepo.AddRangeAsync(selectedChoices, cancellationToken);
            }


            int earnPoints = PointsConfiguration.GetEarnPoints(EntityType.UserPlanExcersiesAnswer);

            answer.AddDomainEvent(new IncreaseUserPointByUserIdEvent(userId, earnPoints, answer.Id, EntityType.UserPlanExcersiesAnswer));
            answer.AddDomainEvent(new PointsEarnedEvent(userId, earnPoints, PointMessages.GetIncresePointMessgeForAnsweringUserStepAnswer(answer.Id), answer.Id));
            answer.AddDomainEvent(new CallUserCompanionEvent(userId, answer.Id, EntityType.UserPlanExcersiesAnswer, input.UserPlanId));


            return ServiceResult.Ok();
        }
    }
}
