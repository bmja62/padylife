using Common.Shemas;
using Entities.Chats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Chats
{
    public class MessageStatusConfiguration : IEntityTypeConfiguration<MessageStatus>
    {
        public void Configure(EntityTypeBuilder<MessageStatus> builder)
        {
            builder.ToTable(nameof(MessageStatus), Schema.Chat);

            builder.HasKey(s => s.Id);

            builder.HasIndex(s => new { s.MessageId, s.ReceiverId }).IsUnique();

            builder.Property(s => s.Status)
                   .HasConversion<string>() // enum to string
                   .IsRequired();

            builder.Property(s => s.UpdatedAt)
                   .IsRequired();

            builder.HasOne(s => s.Receiver)
                   .WithMany()
                   .HasForeignKey(s => s.ReceiverId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
