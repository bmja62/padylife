using Entities.Blogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Data.EntitiesConfiguration.Blogs
{
    public class BlogConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.ToTable(nameof(Blog), nameof(Blog));
            builder.HasOne(z => z.User).WithMany(x => x.Blogs).HasForeignKey(z => z.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasQueryFilter(z => !z.IsDeleted);
        }
    }
}
