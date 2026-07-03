using Common.Roles;
using Data.Contracts;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Services.NotificationServices;
public class CheckUserPlansJob
{
    private readonly IRepository<UserPlan> _userPlanRepository;
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<CheckUserPlansJob> _logger;
    private readonly ISendSystemNotification _sendSystemNotification;

    public CheckUserPlansJob(
        IRepository<UserPlan> userPlanRepository,
        ISendSystemNotification sendSystemNotification,
        IRepository<User> userRepository,
        ILogger<CheckUserPlansJob> logger)
    {
        _userPlanRepository = userPlanRepository;
        _sendSystemNotification = sendSystemNotification;
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("CheckUserPlansJob started at {Time}", DateTime.UtcNow);

            var checkDate = DateTime.UtcNow.AddDays(-7);
            var overduePlans = await _userPlanRepository.TableNoTracking
                .Where(up => !up.IsCompleted &&
                             (DateTime?)up.Answers.Max(a => a.CreatedAt) < checkDate)
                .SelectMany(up => up.Experts.Select(e => new
                {
                    up.UserId,
                    ExpertId = e.ExpertId,
                    PlanTitle = up.Plan.Title
                }))
                .ToListAsync(cancellationToken);

            if (overduePlans.Count == 0)
            {
                _logger.LogInformation("No overdue plans found");
                return;
            }

            var groupedNotifications = overduePlans
                .GroupBy(x => x.ExpertId)
                .ToList();
            var adminId = await _userRepository.Table.Include(t => t.UserRoles).ThenInclude(t => t.Role).Where(t => t.UserRoles.Any(t => t.Role.Name == $"{UserRoleNames.Admin}")).Select(t => t.Id).FirstOrDefaultAsync(); ;

            foreach (var group in groupedNotifications)
            {
                var userPlans = group.ToList();
                var description = $"کاربر {userPlans[0].UserId} بیش از ۷ روز است که مرحله جدیدی در «{userPlans[0].PlanTitle}» را پاسخ نداده است.";

                await _sendSystemNotification.SendNotification(
                    senderId: adminId,
                    subject: "یادآوری پلن کاربر",
                    description: description,
                    allUsers: false,
                    users: new List<long> { group.Key },
                    isFromSystem: true
                );
            }


            _logger.LogInformation("CheckUserPlansJob completed successfully. Sent {Count} notifications at {Time}",
                groupedNotifications.Count, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ارسال یادآوری پلن‌های عقب‌افتاده");
            throw;
        }
    }
}