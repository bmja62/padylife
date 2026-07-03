using Application.Blogs.Query;
using Entities.Blogs;

namespace Application.Blogs.DTO
{
    public class GetBlogByIdQueryResponseDTO
    {
        public long Id { get; set; }
        public string MainId { get; set; }
        public string SeoURL { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ShortDescription { get; set; }//use for description tag
        public string MetaKeywords { get; set; }//use for keyword
        public string MetaAuthor { get; set; }// use for author of blog
        public string OGTitle { get; set; }//use for title in social media
        public string OGMainPicUrl { get; set; } // use for social media pic
        public string CanonicalLink { get; set; }// use for meta tag CanonicalLink
        public string OGURL { get; set; }

        public List<MetaDTO> Metas { get; set; }
        public string MetaTags { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string MainImageUrl { get; set; }
        public string SpendTimeForRead { get; set; }
        public BlogStatus Status { get; set; }
        public string TableOfContent { get; set; }
        public DateTime CreatedAt { get; set; }
        public string VideoThumbnailImageUrl { get; set; }
        public string MetaContent { get; set; }
        public string ScriptContent { get; set; }
        public List<GetBlogRelatedDTO> RelatedBlogs { get; set; }
        public AuthorDTO Author { get; set; }
        public long BlogCategoryId { get; set; }
        public string BlogCategoryTitle { get; set; }
    }

    public class MetaDTO
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }
}
