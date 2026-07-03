using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Data.Contracts;
using Entities.Plans;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public class GetSignUpPlanQuery : IQuery<ServiceResult<GetSignUpPlanDTO>>
    {
    }

    public class GetSignUpPlanQueryHandler(IRepository<Plan> planRepository) : IQueryHandler<GetSignUpPlanQuery, ServiceResult<GetSignUpPlanDTO>>
    {
        public async Task<ServiceResult<GetSignUpPlanDTO>> Handle(GetSignUpPlanQuery request, CancellationToken cancellationToken)
          => ServiceResult.Ok(await planRepository.Table.Where(t => t.IsSignUpPlan && t.Status == Plan.PlanStatus.Active).Select(t =>
            GetSignUpPlanDTO.CreateDefault
            (
                t.Id,
                t.PlanCategoryId,
                t.PlanCategory.Name,
                t.Description,
                t.Status,
                t.IsSignUpPlan,
                t.PlanQuestions.Where(a => a.IsMain)
                .Select(tt =>
                GetSignUpPlanMainQuestionDTO.CreateDefault(
                    tt.Id,
                    tt.QuestionId,
                    tt.Question.Text,
                    tt.IsMain,
                    tt.Question.QuestionOptions.Select(ttt => GetSignUpPlanMainQuestionQuestioOptionDTO.CreateDefaut(
                        ttt.Id,
                        ttt.Text
                        )
                    ).ToList()
                    )
                ).FirstOrDefault()
                )
            ).FirstOrDefaultAsync());

    }
}
