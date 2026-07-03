using Application.Cqrs.Queris;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Reports.Queries
{
    public record GetWeeklyCommitmentReportQuery(long UserId, int Weeks = 4)
        : IQuery<ServiceResult<WeeklyCommitmentReportDTO>>;

    public class GetWeeklyCommitmentReportQueryHandler(
        IRepository<UserPlanAnswer> answerRepo,
        IRepository<UserExercise> exerciseRepo
    ) : IQueryHandler<GetWeeklyCommitmentReportQuery, ServiceResult<WeeklyCommitmentReportDTO>>
    {
        public async Task<ServiceResult<WeeklyCommitmentReportDTO>> Handle(
            GetWeeklyCommitmentReportQuery request,
            CancellationToken cancellationToken)
        {
            var endDate = DateTime.UtcNow.Date.AddDays(1);
            var startDate = endDate.AddDays(-7 * request.Weeks);

            // گروه‌بندی بر اساس هفته
            var weeklyData = new List<WeeklyCommitmentDataDTO>();

            for (int i = 0; i < request.Weeks; i++)
            {
                var weekStart = endDate.AddDays(-7 * (i + 1));
                var weekEnd = endDate.AddDays(-7 * i);

                var answersCount = await answerRepo.TableNoTracking
                    .CountAsync(a => a.UserPlan.UserId == request.UserId &&
                                   a.CreatedAt >= weekStart &&
                                   a.CreatedAt < weekEnd &&
                                   !a.IsDeleted,
                              cancellationToken);

                var exercisesCount = await exerciseRepo.TableNoTracking
                    .CountAsync(e => e.UserId == request.UserId &&
                                   e.AssignedDate >= weekStart &&
                                   e.AssignedDate < weekEnd &&
                                   !e.IsDeleted,
                              cancellationToken);

                var completedExercises = await exerciseRepo.TableNoTracking
                    .CountAsync(e => e.UserId == request.UserId &&
                                   e.CompletedDate >= weekStart &&
                                   e.CompletedDate < weekEnd &&
                                   e.IsCompleted &&
                                   !e.IsDeleted,
                              cancellationToken);

                weeklyData.Add(new WeeklyCommitmentDataDTO
                {
                    WeekNumber = i + 1,
                    WeekLabel = $"هفته {i + 1}",
                    AnswersCount = answersCount,
                    ExercisesAssigned = exercisesCount,
                    ExercisesCompleted = completedExercises,
                    CompletionRate = exercisesCount > 0 ?
                        (int)((double)completedExercises / exercisesCount * 100) : 0
                });
            }

            // محاسبه میانگین‌ها
            var avgAnswers = weeklyData.Average(w => w.AnswersCount);
            var avgCompletion = weeklyData.Average(w => w.CompletionRate);

            var result = new WeeklyCommitmentReportDTO
            {
                Weeks = request.Weeks,
                WeeklyData = weeklyData.OrderBy(w => w.WeekNumber).ToList(),
                AverageAnswersPerWeek = (int)avgAnswers,
                AverageCompletionRate = (int)avgCompletion
            };

            return ServiceResult.Ok(result);
        }
    }

    public class WeeklyCommitmentReportDTO
    {
        public int Weeks { get; set; }
        public List<WeeklyCommitmentDataDTO> WeeklyData { get; set; } = new();
        public int AverageAnswersPerWeek { get; set; }
        public int AverageCompletionRate { get; set; }
    }

    public class WeeklyCommitmentDataDTO
    {
        public int WeekNumber { get; set; }
        public string WeekLabel { get; set; } = string.Empty;
        public int AnswersCount { get; set; }
        public int ExercisesAssigned { get; set; }
        public int ExercisesCompleted { get; set; }
        public int CompletionRate { get; set; }
    }
}