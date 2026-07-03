using Entities.Common;

namespace Entities.Blogs;

public class RelatedBlog : IEntity
{
    //Fks
    public long BlogId { get; set; }
    public long RelatedBlogId { get; set; }

    //Navigations
    public Blog Blog { get; set; }
    public Blog Relatedblog { get; set; }
}