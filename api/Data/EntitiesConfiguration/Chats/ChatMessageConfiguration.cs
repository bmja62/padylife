using Common.Shemas;
using Entities.Chats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Chats
{
    public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.ToTable(nameof(ChatMessage), Schema.Chat);

            builder.HasKey(m => m.Id);

            builder.Property(m => m.EncryptedContent)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(m => m.Type).IsRequired();
            builder.Property(m => m.Status).IsRequired();
            builder.Property(m => m.CreatedAt).IsRequired();

            builder.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId);

            builder.HasMany(t => t.Statuses)
                .WithOne(t => t.Message)
                .HasForeignKey(t => t.MessageId);

            builder.HasMany(m => m.Reactions)
                .WithOne(r => r.Message)
                .HasForeignKey(r => r.ChatMessageId);



        }
    }

}
