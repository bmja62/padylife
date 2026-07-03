using Common;
using Data.Contracts;
using Entities.Plans;
using Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Services.Services.UserServices
{
    public interface IUserSimilarityService
    {
        Task<List<(long UserId, double Similarity)>> CalculateSimilarityAsync(long baseUserId, int topCount = 10, CancellationToken cancellationToken = default);
        Task<List<(long UserId, double Similarity)>> CalculateSimilarityAsync(
      long baseUserId,
      int topCount = 10,
      long? planId = null,
      long? questionCategoryId = null,
      CancellationToken cancellationToken = default);
    }

    public class UserSimilarityService : IUserSimilarityService, IScopedDependency
    {
        private readonly IRepository<UserPlanAnswer> _answerRepo;
        private readonly IRepository<PlanQuestion> _planQuestionRepo;
        public UserSimilarityService(IRepository<UserPlanAnswer> answerRepo, IRepository<PlanQuestion> planQuestionRepo)
        {
            _answerRepo = answerRepo;
            _planQuestionRepo = planQuestionRepo;
        }

        public async Task<List<(long UserId, double Similarity)>> CalculateSimilarityAsync(long baseUserId, int topCount = 10, CancellationToken cancellationToken = default)
        {
            var baseAnswers = await _answerRepo.TableNoTracking
       .Where(a => a.UserPlan.UserId == baseUserId)
       // مرتب سازی بر اساس تاریخ ایجاد نزولی
       .OrderByDescending(a => a.CreatedAt)
       // گروه‌بندی بر اساس سوال و انتخاب آخرین پاسخ
       .GroupBy(a => a.PlanQuestionId)
       .Select(g => new
       {
           PlanQuestionId = g.Key,
           SelectedQuestionOptionId = g.First().SelectedQuestionOptionId
       })
       .ToListAsync(cancellationToken);

            if (!baseAnswers.Any())
                return new();

            var baseMap = baseAnswers.ToDictionary(a => a.PlanQuestionId, a => a.SelectedQuestionOptionId);

            var otherAnswers = await _answerRepo.TableNoTracking
                .Where(a => a.UserPlan.UserId != baseUserId)
                .Select(a => new
                {
                    UserId = a.UserPlan.UserId,
                    a.PlanQuestionId,
                    a.SelectedQuestionOptionId
                }).ToListAsync(cancellationToken);

            var grouped = otherAnswers
                .GroupBy(a => a.UserId)
                .Select(g =>
                {
                    var otherMap = g.GroupBy(x => x.PlanQuestionId)
                                    .ToDictionary(gr => gr.Key, gr => gr.First().SelectedQuestionOptionId);

                    var shared = baseMap.Count(kvp =>
                        otherMap.TryGetValue(kvp.Key, out var v) && v == kvp.Value);

                    var similarity = shared * 100.0 / baseMap.Count;
                    return (UserId: g.Key, Similarity: Math.Round(similarity, 2));
                })
                .Where(x => x.Similarity > 0)
                .OrderByDescending(x => x.Similarity)
                .Take(topCount)
                .ToList();

            return grouped;
        }
        public async Task<List<(long UserId, double Similarity)>> CalculateSimilarityAsync(
       long baseUserId,
       int topCount = 10,
       long? planId = null,
       long? questionCategoryId = null,
       CancellationToken cancellationToken = default)
        {
            // فیلتر کردن PlanQuestionIds بر اساس PlanId یا Category
            List<long>? validPlanQuestionIds = null;

            if (planId.HasValue || questionCategoryId.HasValue)
            {
                var query = _planQuestionRepo.TableNoTracking.Include(pq => pq.Question).AsQueryable();

                if (planId.HasValue)
                    query = query.Where(pq => pq.PlanId == planId.Value);

                if (questionCategoryId.HasValue)
                    query = query.Where(pq => pq.Question.QuestionCategoryId == questionCategoryId.Value);

                validPlanQuestionIds = await query.Select(pq => pq.Id).ToListAsync(cancellationToken);
            }

            // گرفتن پاسخ‌های کاربر مبنا
            var baseAnswersQuery = _answerRepo.TableNoTracking
                .Where(a => a.UserPlan.UserId == baseUserId);

            if (validPlanQuestionIds is { Count: > 0 })
                baseAnswersQuery = baseAnswersQuery.Where(a => validPlanQuestionIds.Contains(a.PlanQuestionId));

            var baseAnswers = await baseAnswersQuery
                .Select(a => new { a.PlanQuestionId, a.SelectedQuestionOptionId })
                .ToListAsync(cancellationToken);

            if (!baseAnswers.Any())
                return new();

            var baseMap = baseAnswers.ToDictionary(a => a.PlanQuestionId, a => a.SelectedQuestionOptionId);

            // گرفتن پاسخ‌های سایر کاربران
            var otherAnswersQuery = _answerRepo.TableNoTracking
                .Where(a => a.UserPlan.UserId != baseUserId);

            if (validPlanQuestionIds is { Count: > 0 })
                otherAnswersQuery = otherAnswersQuery.Where(a => validPlanQuestionIds.Contains(a.PlanQuestionId));

            var otherAnswers = await otherAnswersQuery
                .Select(a => new
                {
                    UserId = a.UserPlan.UserId,
                    a.PlanQuestionId,
                    a.SelectedQuestionOptionId
                }).ToListAsync(cancellationToken);

            var grouped = otherAnswers
                .GroupBy(a => a.UserId)
                .Select(g =>
                {
                    var otherMap = g.GroupBy(x => x.PlanQuestionId)
                        .ToDictionary(gr => gr.Key, gr => gr.First().SelectedQuestionOptionId);

                    var shared = baseMap.Count(kvp =>
                        otherMap.TryGetValue(kvp.Key, out var v) && v == kvp.Value);

                    var similarity = shared * 100.0 / baseMap.Count;
                    return (g.Key, Math.Round(similarity, 2));
                })
                .Where(x => x.Item2 > 0)
                .OrderByDescending(x => x.Item2)
                .Take(topCount)
                .ToList();

            return grouped;
        }
    }
}
