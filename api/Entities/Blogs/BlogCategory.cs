using Entities.Common;

namespace Entities.Blogs
{
    public class BlogCategory : BaseEntity<long>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public ICollection<Blog> Blogs { get; set; }
    }
}
