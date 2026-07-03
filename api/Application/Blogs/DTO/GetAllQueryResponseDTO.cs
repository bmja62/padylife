
using Entities.Blogs;

namespace Application.Blogs.DTO
{
    public class GetAllQueryResponseDTO
    {
        public long Id { get; set; }
        public long BlogCategoryId { get; set; }
        public string BlogCategoryTitle { get; set; }

        public string Title { get; set; }
        public string SeoURL { get; set; }
        public string Content { get; set; }

        public DateTime CreatedAt { get; internal set; }
        public BlogStatus Status { get; internal set; }
        public string ShortDescription { get; internal set; }
        public string MainImageUrl { get; internal set; }
    }
}
