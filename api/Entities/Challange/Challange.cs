using Entities.Common;
using Entities.Users;
using System.ComponentModel.DataAnnotations;

namespace Entities.Challange
{
    public class Challange : BaseEntity<long>
    {
        public Challange()
        {
            RecalculateParticipantCount();
        }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ParticipantCount { get; private set; }
        public string ImageUrl { get; set; }
        public ChallengeType Type { get; set; }
        
        public long? CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }

        private readonly List<ChallangeLog> _logs = new();
        public IReadOnlyCollection<ChallangeLog> Logs => _logs.AsReadOnly();

        public void ActivityLog(long userId)
        {
            var log = new ChallangeLog
            {
                UserId = userId
            };

            _logs.Add(log);

            RecalculateParticipantCount();
        }

        private void RecalculateParticipantCount()
        {
            ParticipantCount = _logs.Count();
        }
    }
    public enum ChallengeType
    {
        [Display(Name = "تک نفره")]
        Single,
        [Display(Name = "گروهی")]
        Group
    }
    public class ChallangeLog : IEntity
    {
        public long ChallengId { get; set; }
        public long UserId { get; set; }

        public User User { get; set; }
        public Challange Challange { get; set; }
    }
}
