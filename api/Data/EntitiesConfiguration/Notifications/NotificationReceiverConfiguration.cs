using Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Notifications
{
    public class NotificationReceiverConfiguration : IEntityTypeConfiguration<NotificationReceiver>
    {
        public void Configure(EntityTypeBuilder<NotificationReceiver> builder)
        {
            builder.ToTable(nameof(NotificationReceiver), nameof(Notification));
            builder.HasKey(x => x.Id);
            builder.HasOne(t => t.Notification).WithMany(t => t.NotificationReceivers).HasForeignKey(t => t.NotificationId);
            builder.HasOne(t => t.Receiver).WithMany(t => t.NotificationReceivers).HasForeignKey(t => t.ReceiverId);
        }
    }
}
