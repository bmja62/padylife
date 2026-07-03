using Application.Cqrs.Queris;
using Application.Plans.DTOs;
using Data.Contracts;
using Entities.Plans;
using Entities.Questions;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Queries
{
    public record GetAnswerDistributionQuery(long PlanQuestionId) : IQuery<ServiceResult<List<AnswerDistributionDTO>>>;

    public class GetAnswerDistributionQueryHandler(
    IRepository<UserPlanAnswer> answerRepo,
    IRepository<PlanQuestion> planQuestionRepo,
    IRepository<QuestionOption> optionRepo
) : IQueryHandler<GetAnswerDistributionQuery, ServiceResult<List<AnswerDistributionDTO>>>
    {
        public async Task<ServiceResult<List<AnswerDistributionDTO>>> Handle(GetAnswerDistributionQuery request, CancellationToken cancellationToken)
        {
            var planQuestion = await planQuestionRepo
                .TableNoTracking
                .Include(pq => pq.Question)
                .FirstOrDefaultAsync(pq => pq.Id == request.PlanQuestionId, cancellationToken);

            if (planQuestion == null)
                return ServiceResult.NotFound<List<AnswerDistributionDTO>>("سوال مورد نظر یافت نشد.");

            var questionId = planQuestion.QuestionId;

            var options = await optionRepo
                .TableNoTracking
                .Where(o => o.QuestionId == questionId)
                .ToListAsync(cancellationToken);

            var totalAnswers = await answerRepo
                .TableNoTracking
                .Where(a => a.PlanQuestionId == request.PlanQuestionId)
                .CountAsync(cancellationToken);

            var counts = await answerRepo
                .TableNoTracking
                .Where(a => a.PlanQuestionId == request.PlanQuestionId)
                .GroupBy(a => a.SelectedQuestionOptionId)
                .Select(g => new { OptionId = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var result = options.Select(option =>
            {
                var match = counts.FirstOrDefault(c => c.OptionId == option.Id);
                var percent = totalAnswers > 0 ? (int)Math.Round((match?.Count ?? 0) * 100.0 / totalAnswers) : 0;

                return new AnswerDistributionDTO
                {
                    OptionId = option.Id,
                    OptionText = option.Text,
                    Percentage = percent
                };
            }).ToList();

            return ServiceResult.Ok(result);
        }
    }
}
