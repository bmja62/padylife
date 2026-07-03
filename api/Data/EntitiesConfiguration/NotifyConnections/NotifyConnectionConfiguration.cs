using Entities.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.NotifyConnections
{
    public class NotifyConnectionConfiguration : IEntityTypeConfiguration<NotifyConnection>
    {
        public void Configure(EntityTypeBuilder<NotifyConnection> builder)
        {
            builder.ToTable("NotifyConnections", "Hub");
            builder.HasKey(x => x.Id);
            builder.HasOne(t => t.User).WithMany(t => t.NotifyConnections).HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
