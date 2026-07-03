using Application.Cqrs.Queris;
using Application.Reports.DTOs;
using Common.Utilities;
using Data.Contracts;
using Entities.Excersies;
using Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Reports.Queries
{
    public record GetUserActivityForAllQuestionsReportQuery() : IQuery<ServiceResult<GetUserActivityForAllQuestionsReportDTO>>;

    public class GetUserActivityForAllQuestionsReportQueryHandler
        (
            IHttpContextAccessor accessor,
            IRepository<UserPlan> userPlanRepository,
            IRepository<UserExercise> userExerciseRepository,
            IRepository<Exercise> exerciseRepository,
            IRepository<UserPlanExcersiesAnswer> userPlanExcersiesAnswerRepository
        ) : IQueryHandler<GetUserActivityForAllQuestionsReportQuery, ServiceResult<GetUserActivityForAllQuestionsReportDTO>>

    {
        public async Task<ServiceResult<GetUserActivityForAllQuestionsReportDTO>> Handle(GetUserActivityForAllQuestionsReportQuery request, CancellationToken cancellationToken)
        {
            var userId = accessor.HttpContext.User.Identity.GetUserId<long>();

            // 1. دریافت اطلاعات کاربر و تمرینات
            var userPlans = await userPlanRepository.Table
                .Where(t => t.UserId == userId)
                .ToListAsync(cancellationToken);

            var userPlansIds = userPlans.Select(t => t.Id).ToList();

            var userExercises = await userExerciseRepository.Table
                .Where(t => t.UserId == userId)
                .ToListAsync(cancellationToken);

            var exerciseIds = userExercises.Select(t => t.ExerciseId).ToList();

            // 2. دریافت ساختار تمرینات (مراحل و گزینه‌ها)
            var exercises = await exerciseRepository.Table
                .Where(t => exerciseIds.Contains(t.Id))
                .Include(t => t.ExerciseSteps)
                    .ThenInclude(t => t.Step)
                        .ThenInclude(t => t.StepOptions)
                .ToListAsync(cancellationToken);

            // 3. دریافت پاسخ‌های کاربر
            var answers = await userPlanExcersiesAnswerRepository.Table
                .Where(t => userPlansIds.Contains(t.UserPlanId) && exerciseIds.Contains(t.ExcersieId))
                .Select(t => new UserAnswerDto
                {
                    UserId = userId,
                    ExcersieId = t.ExcersieId,
                    StepId = t.StepId,
                    SelectedStepOptionId = t.SelectedStepOptionId,
                    SelectedChoices = t.SelectedChoices.Select(sc => new UserSelectedChoiceDto
                    {
                        OptionChoiceId = sc.OptionChoiceId,
                        StepOptionId = sc.Choice.StepOptionId
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            // 4. محاسبه آمار برای کاربر جاری
            var currentUserStats = CalculateUserStatistics(exercises, answers, userId);

            // 5. محاسبه آمار برای سایر کاربران (میانگین)
            var allUsersAnswers = await userPlanExcersiesAnswerRepository.Table
                .Where(t => exerciseIds.Contains(t.ExcersieId))
                .Select(t => new OtherUserAnswerDto
                {
                    UserId = t.UserPlan.UserId,
                    ExcersieId = t.ExcersieId,
                    StepId = t.StepId,
                    SelectedStepOptionId = t.SelectedStepOptionId,
                    SelectedChoices = t.SelectedChoices.Select(sc => new OtherUserSelectedChoiceDto
                    {
                        OptionChoiceId = sc.OptionChoiceId,
                        StepOptionId = sc.Choice.StepOptionId
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            var averageCompletionPercentage = CalculateAllUsersStatistics(exercises, allUsersAnswers, userId);

            // 6. ایجاد DTO نتیجه
            var result = new GetUserActivityForAllQuestionsReportDTO
            {
                UserId = userId,
                TotalExercises = exercises.Count,
                TotalSteps = exercises.Sum(e => e.ExerciseSteps.Count),
                CompletedSteps = currentUserStats.CompletedSteps,
                CompletionPercentage = currentUserStats.CompletionPercentage,
                AverageCompletionPercentage = averageCompletionPercentage,
                ExerciseStats = currentUserStats.ExerciseStats,
                ComparisonWithOthers = currentUserStats.CompletionPercentage.CompareTo(averageCompletionPercentage)
            };

            return ServiceResult.Ok(result);
        }

        private (int CompletedSteps, double CompletionPercentage, List<ExerciseStatDTO> ExerciseStats)
            CalculateUserStatistics(List<Exercise> exercises, List<UserAnswerDto> answers, long userId)
        {
            var exerciseStats = new List<ExerciseStatDTO>();
            int totalSteps = 0;
            int completedSteps = 0;

            foreach (var exercise in exercises)
            {
                var exerciseStepCount = exercise.ExerciseSteps.Count;
                totalSteps += exerciseStepCount;

                var exerciseCompletedSteps = answers
                    .Where(a => a.ExcersieId == exercise.Id)
                    .Select(a => a.StepId)
                    .Distinct()
                    .Count();

                completedSteps += exerciseCompletedSteps;

                exerciseStats.Add(new ExerciseStatDTO
                {
                    ExerciseId = exercise.Id,
                    ExerciseTitle = exercise.Title,
                    TotalSteps = exerciseStepCount,
                    CompletedSteps = exerciseCompletedSteps,
                    CompletionPercentage = exerciseStepCount > 0 ?
                        (double)exerciseCompletedSteps / exerciseStepCount * 100 : 0
                });
            }

            double completionPercentage = totalSteps > 0 ?
                (double)completedSteps / totalSteps * 100 : 0;

            return (completedSteps, completionPercentage, exerciseStats);
        }

        private double CalculateAllUsersStatistics(List<Exercise> exercises, List<OtherUserAnswerDto> allUsersAnswers, long currentUserId)
        {
            var userStats = new Dictionary<long, (int TotalSteps, int CompletedSteps)>();

            foreach (var answerGroup in allUsersAnswers.GroupBy(a => a.UserId))
            {
                var userId = answerGroup.Key;
                var userExercises = exercises
                    .Where(e => answerGroup.Any(a => a.ExcersieId == e.Id))
                    .ToList();

                int totalSteps = userExercises.Sum(e => e.ExerciseSteps.Count);
                int completedSteps = answerGroup
                    .Select(a => a.StepId)
                    .Distinct()
                    .Count();

                userStats[userId] = (totalSteps, completedSteps);
            }

            // محاسبه میانگین بدون در نظر گرفتن کاربر جاری
            var relevantStats = userStats
                .Where(kvp => kvp.Key != currentUserId)
                .Select(kvp => kvp.Value.TotalSteps > 0 ?
                    (double)kvp.Value.CompletedSteps / kvp.Value.TotalSteps * 100 : 0)
                .ToList();

            return relevantStats.Any() ? relevantStats.Average() : 0;
        }
    }
}
