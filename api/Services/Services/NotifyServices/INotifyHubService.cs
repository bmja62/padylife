using Common;
using Data.Contracts;
using Entities.Hubs;
using Microsoft.EntityFrameworkCore;

namespace Services.Services.NotifyServices
{
    public interface INotifyHubService
    {
        Task CreateNotifyConnection(long userId, string connectionId);
        Task DeleteNotifyConnectionByConnectionId(string connectionId);
    }

    public class NotifyHubService : INotifyHubService, IScopedDependency
    {
        private readonly IRepository<NotifyConnection> _notifyConnectionRepository;

        public NotifyHubService(IRepository<NotifyConnection> notifyConnectionRepository)
        {
            _notifyConnectionRepository = notifyConnectionRepository;
        }

        public async Task CreateNotifyConnection(long userId, string connectionId) => await _notifyConnectionRepository.AddAsync(NotifyConnection.Create(userId, connectionId), CancellationToken.None);
        public async Task DeleteNotifyConnectionByConnectionId(string connectionId)
        {
            var connectionInDb = await _notifyConnectionRepository.Table.Where(a => a.ConnectionId == connectionId).FirstOrDefaultAsync();
            if (connectionInDb != null)
            {
                await _notifyConnectionRepository.DeleteAsync(connectionInDb, CancellationToken.None);
            }
        }
    }
}
