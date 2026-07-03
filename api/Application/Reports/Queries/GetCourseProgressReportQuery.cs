using Application.Cqrs.Queris;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Reports.Queries
{
    public record GetCourseProgressReportQuery(long UserId)
        : IQuery<ServiceResult<CourseProgressReportDTO>>;

    public class GetCourseProgressReportQueryHandler(
        IRepository<UserPlan> userPlanRepo,
        IRepository<UserExercise> userExerciseRepo
    ) : IQueryHandler<GetCourseProgressReportQuery, ServiceResult<CourseProgressReportDTO>>
    {
        public async Task<ServiceResult<CourseProgressReportDTO>> Handle(
            GetCourseProgressReportQuery request,
            CancellationToken cancellationToken)
        {
            // دریافت تمام پلن‌های کاربر
            var userPlans = await userPlanRepo.TableNoTracking
                .Include(up => up.Plan)
                .Where(up => up.UserId == request.UserId && !up.IsDeleted)
                .ToListAsync(cancellationToken);

            // محاسبه پیشرفت برای هر پلن
            var planProgress = new List<PlanProgressDTO>();

            foreach (var userPlan in userPlans)
            {
                var totalExercises = await userExerciseRepo.TableNoTracking
                    .CountAsync(ue => ue.UserPlanId == userPlan.Id && !ue.IsDeleted,
                              cancellationToken);

                var completedExercises = await userExerciseRepo.TableNoTracking
                    .CountAsync(ue => ue.UserPlanId == userPlan.Id &&
                                     ue.IsCompleted &&
                                     !ue.IsDeleted,
                              cancellationToken);

                var progress = totalExercises > 0 ?
                    (int)((double)completedExercises / totalExercises * 100) : 0;

                planProgress.Add(new PlanProgressDTO
                {
                    PlanId = userPlan.PlanId,
                    PlanTitle = userPlan.Plan?.Title ?? "بدون عنوان",
                    ProgressPercentage = progress,
                    TotalExercises = totalExercises,
                    CompletedExercises = completedExercises
                });
            }

            // محاسبه میانگین پیشرفت کلی
            var overallProgress = planProgress.Any() ?
                (int)planProgress.Average(p => p.ProgressPercentage) : 0;

            var result = new CourseProgressReportDTO
            {
                OverallProgress = overallProgress,
                PlansProgress = planProgress
            };

            return ServiceResult.Ok(result);
        }
    }

    public class CourseProgressReportDTO
    {
        public int OverallProgress { get; set; }
        public List<PlanProgressDTO> PlansProgress { get; set; } = new();
    }

    public class PlanProgressDTO
    {
        public long PlanId { get; set; }
        public string PlanTitle { get; set; } = string.Empty;
        public int ProgressPercentage { get; set; }
        public int TotalExercises { get; set; }
        public int CompletedExercises { get; set; }
    }
}