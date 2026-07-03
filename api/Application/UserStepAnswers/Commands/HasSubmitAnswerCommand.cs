using Application.Cqrs.Commands;
using Application.UserStepAnswers.DTOs;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.UserStepAnswers.Commands
{
    public record HasSubmitAnswerCommand(CreateUserStepAnswerCommandDTO input) : ICommand<ServiceResult>;

    public class HasSubmitAnswerCommandHandler(IRepository<UserPlanExcersiesAnswer> answerRepo) : ICommandHandler<HasSubmitAnswerCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(HasSubmitAnswerCommand request, CancellationToken cancellationToken)
        {
            if (await answerRepo.Table.AnyAsync(t => t.UserPlanId == request.input.UserPlanId && t.ExcersieId == request.input.ExcersieId && t.StepId == request.input.StepId))
            {
                return ServiceResult.BadRequest("شما قبلا در این مرحله شرکت کرده اید");
            }
            return ServiceResult.Ok();
        }
    }
}
