using Entities.Blogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Blogs;

public class RelatedBlogConfiguration : IEntityTypeConfiguration<RelatedBlog>
{
    public void Configure(EntityTypeBuilder<RelatedBlog> builder)
    {
        //Name & shema
        builder.ToTable(nameof(RelatedBlog), "RelatedBlog");

        //Id
        builder.HasKey(x => new { x.BlogId, x.RelatedBlogId });

        //Navigations
        builder.HasOne(t => t.Relatedblog).WithMany(t => t.RelatedBlogs).HasForeignKey(t => t.RelatedBlogId);
        builder.HasOne(t => t.Blog).WithMany(t => t.Blogs).HasForeignKey(t => t.BlogId);

    }
}