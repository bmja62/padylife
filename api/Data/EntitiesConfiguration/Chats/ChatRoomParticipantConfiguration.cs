using Common.Shemas;
using Entities.Chats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Chats
{
    public class ChatRoomParticipantConfiguration : IEntityTypeConfiguration<ChatRoomParticipant>
    {
        public void Configure(EntityTypeBuilder<ChatRoomParticipant> builder)
        {
            builder.ToTable(nameof(ChatRoomParticipant), Schema.Chat);

            builder.HasKey(p => p.Id);

            builder.HasIndex(p => new { p.ChatRoomId, p.UserId }).IsUnique();

            builder.Property(p => p.JoinedAt).IsRequired();

            builder.HasOne(p => p.User)
           .WithMany(u => u.ChatRoomParticipants)
           .HasForeignKey(p => p.UserId);
        }
    }
}
