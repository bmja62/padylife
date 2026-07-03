using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.StepOprions;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.StepOptions.Commands
{
    public record DeleteStepOptionsCommand(long id, bool confrim) : ICommand<ServiceResult>;
    public class DeleteStepOptionsCommandHandler(IRepository<UserPlanExcersiesAnswer> userPlanExcersiesAnswerRepository, IRepository<StepOption> stepOptionRepository) : ICommandHandler<DeleteStepOptionsCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(DeleteStepOptionsCommand request, CancellationToken cancellationToken)
        {
            var stepOptionInDb = await stepOptionRepository.Table.Where(t => t.Id == request.id).FirstOrDefaultAsync(cancellationToken);
            if (stepOptionInDb == null)
                return ServiceResult.NotFound("یافت نشد");

            if (await userPlanExcersiesAnswerRepository.Table.AnyAsync(t => t.SelectedStepOptionId == request.id) && !request.confrim)
                return ServiceResult.BadRequest("شما قادر به حذف این مورد نیستید زیرا کاربرانی این گزینه را انتخاب کرده اند");


            var answers = await userPlanExcersiesAnswerRepository.Table.Where(t => t.SelectedStepOptionId == request.id).ToListAsync(cancellationToken);
            if (answers != null && answers.Count > 0) 
            {
                answers.ForEach(t => t.IsDeleted = true);
                await userPlanExcersiesAnswerRepository.UpdateRangeAsync(answers,cancellationToken);
            }
                


            await stepOptionRepository.SoftDeleteAsync(stepOptionInDb, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
