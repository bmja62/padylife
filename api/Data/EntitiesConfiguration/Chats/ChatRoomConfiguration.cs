using Common.Shemas;
using Entities.Chats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Chats
{
    public class ChatRoomConfiguration : IEntityTypeConfiguration<ChatRoom>
    {
        public void Configure(EntityTypeBuilder<ChatRoom> builder)
        {
            builder.ToTable(nameof(ChatRoom), Schema.Chat);

            builder.HasKey(c => c.Id);

            builder.HasMany(c => c.Participants)
                .WithOne(p => p.ChatRoom)
                .HasForeignKey(p => p.ChatRoomId);

            builder.HasMany(c => c.Messages)
                .WithOne(m => m.ChatRoom)
                .HasForeignKey(m => m.ChatRoomId);

            builder.HasOne(cr => cr.UserPlan)
             .WithMany(up => up.Chats)
             .HasForeignKey(cr => cr.UserPlanId);
        }
    }
}
