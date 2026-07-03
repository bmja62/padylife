namespace Application.Blogs.DTO
{
    public class GetBlogRelatedDTO
    {
        public long Id { get; set; }
        public string MainImageUrl { get; internal set; }
        public string SeoURL { get; internal set; }
        public string Title { get; internal set; }
        public string SeoDescription { get; internal set; }
    }
}
