namespace Application.Blogs.Query.ForWeb.DTOs
{
    public class GetAllForWebDTO
    {
        public long Id { get; internal set; }
        public string Title { get; internal set; }
        public string ShortDescription { get; internal set; }
        public string Image { get; internal set; }
        public string SeoURL { get; internal set; }
        public long BlogCategoryId { get; internal set; }
        public string BlogCategoryTitle { get; internal set; }
    }
}
