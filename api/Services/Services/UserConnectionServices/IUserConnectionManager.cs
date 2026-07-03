using Common;

namespace Services.Services.UserConnectionServices
{
    public interface IUserConnectionManager
    {
        void AddUserConnection(string userId, string connectionId);
        void RemoveUserConnection(string userId, string connectionId);
        IEnumerable<string> GetConnections(string userId);
    }

    public class UserConnectionManager : IUserConnectionManager, IScopedDependency
    {
        private static readonly Dictionary<string, HashSet<string>> _connections = new();

        public void AddUserConnection(string userId, string connectionId)
        {
            lock (_connections)
            {
                if (!_connections.ContainsKey(userId))
                    _connections[userId] = new HashSet<string>();

                _connections[userId].Add(connectionId);
            }
        }

        public void RemoveUserConnection(string userId, string connectionId)
        {
            lock (_connections)
            {
                if (_connections.ContainsKey(userId))
                {
                    _connections[userId].Remove(connectionId);
                    if (_connections[userId].Count == 0)
                        _connections.Remove(userId);
                }
            }
        }

        public IEnumerable<string> GetConnections(string userId)
        {
            lock (_connections)
            {
                return _connections.ContainsKey(userId)
                    ? _connections[userId]
                    : Enumerable.Empty<string>();
            }
        }
    }

}
