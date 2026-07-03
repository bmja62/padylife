using Common.Shemas;
using Entities.Chats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Chats
{
    public class ChatMessageReactionConfiguration : IEntityTypeConfiguration<ChatMessageReaction>
    {
        public void Configure(EntityTypeBuilder<ChatMessageReaction> builder)
        {
            builder.ToTable(nameof(ChatMessageReaction), Schema.Chat);

            builder.HasKey(r => r.Id);

            builder.HasIndex(r => new { r.ChatMessageId, r.UserId }).IsUnique();

            builder.Property(r => r.ReactionType).IsRequired();
            builder.Property(r => r.CreatedAt).IsRequired();

            builder.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);
        }
    }

}
