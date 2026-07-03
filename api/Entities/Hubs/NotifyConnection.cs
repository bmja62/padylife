using Entities.Common;
using Entities.Users;

namespace Entities.Hubs
{
    public class NotifyConnection : BaseEntity<long>
    {
        public NotifyConnection(long userId, string connectionId)
        {
            UserId = userId;
            ConnectionId = connectionId;
        }

        private NotifyConnection()
        {

        }

        public long UserId { get; set; }
        public string ConnectionId { get; set; }
        public User User { get; set; }

        public static NotifyConnection Create(long userId, string connectionId)
        {
            NotifyConnection connection = new(userId, connectionId);
            return connection;
        }
    }
}
