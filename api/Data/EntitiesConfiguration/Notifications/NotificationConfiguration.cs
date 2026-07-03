using Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Notifications
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable(nameof(Notification), nameof(Notification));
            builder.HasKey(x => x.Id);
            builder.HasMany(t => t.NotificationReceivers).WithOne(t => t.Notification).HasForeignKey(t => t.NotificationId);
            builder.HasOne(t => t.Sender).WithMany(t => t.NotificationSenders).HasForeignKey(t => t.SenderId);
        }
    }
}
