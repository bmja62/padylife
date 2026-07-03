using Application.Cqrs.Queris;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Reports.Queries
{
    public record GetUserActivityReportQuery(long UserId)
        : IQuery<ServiceResult<UserActivityReportDTO>>;

    public class GetUserActivityReportQueryHandler(
        IRepository<UserExercise> exerciseRepo,
        IRepository<UserPlan> planRepo,
        IRepository<PointTransaction> pointRepo,
        IRepository<User> userRepo
    ) : IQueryHandler<GetUserActivityReportQuery, ServiceResult<UserActivityReportDTO>>
    {
        public async Task<ServiceResult<UserActivityReportDTO>> Handle(
            GetUserActivityReportQuery request,
            CancellationToken cancellationToken)
        {
            var user = await userRepo.TableNoTracking
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
                return ServiceResult.NotFound<UserActivityReportDTO>("کاربر یافت نشد");

            // محاسبه آمار تمرینات
            var totalExercises = await exerciseRepo.TableNoTracking
                .CountAsync(e => e.UserId == request.UserId && !e.IsDeleted,
                          cancellationToken);

            var completedExercises = await exerciseRepo.TableNoTracking
                .CountAsync(e => e.UserId == request.UserId &&
                                e.IsCompleted &&
                                !e.IsDeleted,
                          cancellationToken);

            // محاسبه آمار پلن‌ها
            var activePlans = await planRepo.TableNoTracking
                .CountAsync(p => p.UserId == request.UserId &&
                               !p.IsDeleted &&
                               p.EndDate >= DateTime.UtcNow,
                          cancellationToken);

            // محاسبه امتیازات
            var totalPoints = await pointRepo.TableNoTracking
                .Where(p => p.UserId == request.UserId &&
                          p.TransactionType == PointTransactionType.Earn &&
                          !p.IsReverted)
                .SumAsync(p => p.Amount, cancellationToken);

            // مقایسه با سایر کاربران
            var allUsersCount = await userRepo.TableNoTracking.CountAsync(cancellationToken);
            var usersWithMorePoints = await pointRepo.TableNoTracking
                .Where(p => p.TransactionType == PointTransactionType.Earn && !p.IsReverted)
                .GroupBy(p => p.UserId)
                .Where(g => g.Sum(x => x.Amount) > totalPoints)
                .CountAsync(cancellationToken);

            var leaderPercentile = allUsersCount > 0 ?
                (int)((double)usersWithMorePoints / allUsersCount * 100) : 0;

            var result = new UserActivityReportDTO
            {
                UserId = user.Id,
                UserName = user.FullName,
                TotalExercises = totalExercises,
                CompletedExercises = completedExercises,
                ExerciseCompletionRate = totalExercises > 0 ?
                    (int)((double)completedExercises / totalExercises * 100) : 0,
                ActivePlans = activePlans,
                TotalPoints = totalPoints,
                LeaderPercentile = leaderPercentile,
                HealthStatus = CalculateHealthStatus(totalExercises, completedExercises, activePlans)
            };

            return ServiceResult.Ok(result);
        }

        private string CalculateHealthStatus(int totalExercises, int completedExercises, int activePlans)
        {
            if (activePlans == 0) return "شروع نشده";

            var completionRate = (double)completedExercises / totalExercises;

            if (completionRate >= 0.8) return "عالی";
            if (completionRate >= 0.5) return "خوب";
            if (completionRate >= 0.3) return "متوسط";

            return "نیاز به بهبود";
        }
    }

    public class UserActivityReportDTO
    {
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TotalExercises { get; set; }
        public int CompletedExercises { get; set; }
        public int ExerciseCompletionRate { get; set; }
        public int ActivePlans { get; set; }
        public int TotalPoints { get; set; }
        public int LeaderPercentile { get; set; }
        public string HealthStatus { get; set; } = string.Empty;
    }
}