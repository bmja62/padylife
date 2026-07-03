using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Application.Excersies.Commands;
using Application.Plans.DTOs;
using Application.Plans.Helpers;
using Common.Utilities;
using Data.Contracts;
using Entities.Plans;
using Entities.Questions;
using Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Commands
{
    public class CreateUserPlanAnswerCommand(CreateUserPlanAnswerDTO input) : ICommand<ServiceResult<GetPlanFlowDTO>>
    {
        public CreateUserPlanAnswerDTO Input { get; } = input;
    }

    public class CreateUserPlanAnswerCommandHandler(
        IHttpContextAccessor accessor,
        IRepository<UserPlanAnswer> userPlanAnswerRepository,
        IRepository<UserPlan> userPlanRepository,
        IRepository<PlanQuestion> planQuestionRepository,
        IRepository<QuestionLinked> questionLinkedRepository,
        IQueryDispatcher queryDispatcher,
        ICommandDispatcher commandDispatcher) : ICommandHandler<CreateUserPlanAnswerCommand, ServiceResult<GetPlanFlowDTO>>
    {
        public async Task<ServiceResult<GetPlanFlowDTO>> Handle(CreateUserPlanAnswerCommand request, CancellationToken cancellationToken)
        {
            var newUserAnswer = UserPlanAnswer.CreateDefault(request.Input.UserPlanId, request.Input.PlanQuestionId, request.Input.SelectedQuestionOptionId);

            await userPlanAnswerRepository.AddAsync(newUserAnswer, cancellationToken);

            var planId = await userPlanAnswerRepository.Table.Where(t => t.UserPlanId == request.Input.UserPlanId).Select(t => t.UserPlan.PlanId).FirstOrDefaultAsync();

            var currentUserId = accessor.HttpContext.User.Identity.GetUserId<long>();

            var unansweredQuestion = PlanFlowService.GetNextUnansweredQuestion(
                userPlanRepository.Table,
                userPlanAnswerRepository.Table,
                questionLinkedRepository.Table,
                planQuestionRepository.Table,
                planId,
                currentUserId
                );

            if (unansweredQuestion is null)
            {
                var questionLinkedId = questionLinkedRepository.Table
                    .Where(t =>
                    t.PlanId == planId &&
                    t.PlanQuestionId == newUserAnswer.PlanQuestionId &&
                    t.QuestionOptionId == newUserAnswer.SelectedQuestionOptionId
                    ).Select(t => t.Id).FirstOrDefault();

                if (questionLinkedId > 0)
                    await commandDispatcher.SendAsync(new AssginExcersieToUserCommand(questionLinkedId, newUserAnswer.UserPlanId));
            }

            return ServiceResult.Ok(GetPlanFlowDTO.SetPlanFlow(planId, newUserAnswer.PlanQuestionId, request.Input.SelectedQuestionOptionId));
        }
    }
}
