using Entities.Common;
using Entities.Users;

namespace Entities.Medals
{
    public class UserMedal : BaseEntity<long>
    {
        public long UserId { get; set; }
        public long MedalId { get; set; }
        public DateTime AwardedAt { get; set; }

        public Medal Medal { get; set; }
        public User User { get; set; }
    }

}
