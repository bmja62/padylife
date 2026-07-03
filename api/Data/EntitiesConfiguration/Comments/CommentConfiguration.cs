using Entities.Comments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Comments
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable(nameof(Comment), nameof(Comment));

            builder.HasOne(t => t.CreatedByUser)
                .WithMany(t => t.Comments)
                .HasForeignKey(t => t.CreatedByUserId);

            builder.HasMany(t => t.Replies)
                .WithOne(t => t.ParentComment)
                .HasForeignKey(t => t.ParentCommentId);

            builder.HasMany(t => t.Reactions)
                .WithOne(t => t.Comment)
                .HasForeignKey(t => t.CommentId);

            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
    public class CommentReactionConfiguration : IEntityTypeConfiguration<CommentReaction>
    {
        public void Configure(EntityTypeBuilder<CommentReaction> builder)
        {
            builder.ToTable(nameof(CommentReaction), nameof(Comment));

            builder.HasOne(t => t.Comment)
                .WithMany(t => t.Reactions)
                .HasForeignKey(t => t.CommentId);

            builder.HasOne(t => t.User)
                .WithMany(t => t.CommentReactions)
                .HasForeignKey(t => t.UserId);
        }
    }
}
