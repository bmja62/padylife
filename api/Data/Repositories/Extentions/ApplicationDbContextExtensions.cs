using Entities.Common;
using Entities.Common.Events;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Extentions
{
    public static class ApplicationDbContextExtensions
    {
        public static List<IDomainEvent> GetDomainEvents(this DbContext context)
        {
            return context.ChangeTracker
                .Entries<BaseEntity<long>>() // هر چیزی که BaseEntity باشه و دامین ایونت داشته باشه
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();
        }

        public static void ClearDomainEvents(this DbContext context)
        {
            var entities = context.ChangeTracker
                .Entries<BaseEntity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            foreach (var entityEntry in entities)
            {
                entityEntry.Entity.ClearDomainEvents();
            }
        }
    }
}
