using Entities.Blogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Blogs
{
    public class BlogCategoryConfiguration : IEntityTypeConfiguration<BlogCategory>
    {
        public void Configure(EntityTypeBuilder<BlogCategory> builder)
        {
            builder.ToTable(nameof(BlogCategory), nameof(Blog));

            builder.HasMany(t => t.Blogs).WithOne(t => t.BlogCategory).HasForeignKey(t => t.BlogCategoryId);

            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
