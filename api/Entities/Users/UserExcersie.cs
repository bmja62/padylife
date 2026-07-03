using Entities.Common;
using Entities.Excersies;

namespace Entities.Users
{
    public class UserExercise : BaseEntity<long>
    {
        public UserExercise(long userId, long exerciseId, long userPlanId)
        {
            UserId = userId;
            ExerciseId = exerciseId;
            UserPlanId = userPlanId;
        }

        public long UserId { get; set; }
        public long ExerciseId { get; set; }
        public long UserPlanId { get; set; }
        public DateTime AssignedDate { get; set; } = DateTime.Now;
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedDate { get; set; }

        // Navigations
        public User User { get; set; }
        public Exercise Exercise { get; set; }
        public UserPlan UserPlan { get; set; }

        public static UserExercise Create(long userId, long excersieId, long userPlanId)
        => new(userId, excersieId, userPlanId);

        public void SetIsCompleted()
        {
            IsCompleted = true;
            CompletedDate = DateTime.Now;
        }
    }
}
