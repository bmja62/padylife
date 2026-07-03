using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Common.GridResults;
using Data.Contracts;
using Entities.Plans;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public class GetPlanQuestionsQuery(long planId, GlobalGrid globalGrid) : IQuery<ServiceResult<GlobalGridResult<PlanQuestionDTO>>>
    {
        public long PlanId { get; } = planId;
        public GlobalGrid GlobalGrid { get; set; } = globalGrid;
    }

    public class GetPlanQuestionsQueryHandler(IRepository<Plan> planRepository) : IQueryHandler<GetPlanQuestionsQuery, ServiceResult<GlobalGridResult<PlanQuestionDTO>>>
    {
        public async Task<ServiceResult<GlobalGridResult<PlanQuestionDTO>>> Handle(GetPlanQuestionsQuery request, CancellationToken cancellationToken)
        {
            var query = planRepository.Table
                .Where(t => t.Id == request.PlanId)
                .SelectMany(t => t.PlanQuestions).AsQueryable();

            query = query.Where(t => !t.PlanQuestionLinks.Any());

            var data = await query
                .Skip(request.GlobalGrid.Skip)
                .Take(request.GlobalGrid.Take)
                .Select(t => PlanQuestionDTO.CreateDefault(
                    t.Id,
                    t.QuestionId,
                    t.Question.Text,
                    t.Question.DisplayText,
                    t.IsMain,
                    null,
                    null))
                .ToListAsync();

            var totalCount = await query.CountAsync();

            return ServiceResult.Ok(new GlobalGridResult<PlanQuestionDTO>
            {
                Data = data,
                TotalCount = totalCount
            });
        }
    }
}
