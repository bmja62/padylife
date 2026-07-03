using Entities.Common;
using Entities.Users;
using HtmlAgilityPack;
using System.Text;

namespace Entities.Blogs
{
    public class Blog : BaseEntity<long>
    {

        //FKs
        public long UserId { get; set; }
        public long BlogCategoryId { get; set; }

        //Props
        public string Title { get; private set; }
        public string SpendTimeToRead { get; set; }
        public string Content { get; private set; }
        public BlogType Type { get; set; }
        public string SeoURL { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string TableOfContent { get; set; }
        public string MainImageUrl { get; set; }
        public string VideoThumbnailImageUrl { get; set; }
        public string Description { get; private set; }
        public string ShortDescription { get; private set; }//use for description tag
        public string MetaKeywords { get; private set; }//use for keyword
        public string MetaAuthor { get; private set; }// use for author of blog
        public string OGTitle { get; private set; }//use for title in social media
        public string OGMainPicUrl { get; private set; } // use for social media pic
        public string CanonicalLink { get; set; }// use for meta tag CanonicalLink
        public string OGURL { get; private set; }
        public string ScriptContent { get; set; }
        public string MetaContent { get; set; }
        public BlogStatus Status { get; set; }


        //navigations

        public User User { get; set; }
        public List<RelatedBlog> RelatedBlogs { get; set; } = new();
        public List<RelatedBlog> Blogs { get; set; } = new();
        public BlogCategory BlogCategory { get; set; }

        //factory methodes
        public static Blog CreateBlog(
            long blogCategoryId,
            string canonicalLink,
            string time,
            string title,
            string Content,
            string shortdesc,
            string metakeywords,
            string metaauthor,
            string OGTitle,
            string OGURL,
            string OGPicURL,
            long userId,
            string seoURL,
            string seoTitle,
            string seoDescription,
            string tableOfContent,
            string scriptContent,
            string metacontent,
            string mainImageFile,
            BlogType type,
            BlogStatus status
            ) => new()
            {
                BlogCategoryId = blogCategoryId,
                SpendTimeToRead = time,
                Type = type,
                Content = Content,
                Description = EXTRACT_TEXTs_FROM_HTML_AND_SET_DESCRIPTION(Content),
                MetaAuthor = metaauthor,
                MetaKeywords = metakeywords,
                OGMainPicUrl = OGPicURL,
                OGTitle = OGTitle,
                OGURL = OGURL,
                ShortDescription = shortdesc,
                Title = title,
                CanonicalLink = canonicalLink,
                UserId = userId,
                SeoURL = seoURL,
                SeoTitle = seoTitle,
                SeoDescription = seoDescription,
                TableOfContent = tableOfContent,
                MainImageUrl = mainImageFile,
                ScriptContent = scriptContent,
                MetaContent = metacontent,
                Status = status,
            };

        public static string CreateBlogMetaTags(Blog blog)
        {
            StringBuilder metaTags = new StringBuilder();
            metaTags.AddMetaAuthor(blog.MetaAuthor);
            metaTags.AddMetaDescription(blog.ShortDescription);
            metaTags.AddMetaKeywords(blog.MetaKeywords);
            metaTags.AddMetaRobots();
            metaTags.AddOgDescription(blog.ShortDescription);
            metaTags.AddCanonicalLink(blog.CanonicalLink);
            metaTags.AddOgImage(blog.OGMainPicUrl);
            metaTags.AddOgTitle(blog.OGTitle);
            metaTags.AddOgUrl(blog.OGURL);
            return metaTags.ToString();
        }

        //reach domain
        public void SetBlogCategoryId(long blogCategoryId) => BlogCategoryId = blogCategoryId;
        public void SetTime(string time) => SpendTimeToRead = time;
        public void SetCanonicalLink(string canonicalLink) => CanonicalLink = canonicalLink;
        public void SetContent(string content) => Content = content;
        public void SetTitle(string title) => Title = title;
        public void SetShortDesc(string desc) => ShortDescription = desc;
        public void SetDesc(string desc) => Description = desc;
        public void SetMetaKeywords(string keyword) => MetaKeywords = keyword;
        public void SetMetaAuthor(string metaAuthor) => MetaAuthor = metaAuthor;
        public void SetOGTitle(string ogTitle) => OGTitle = ogTitle;
        public void SetOGMainPicUrl(string url) => OGMainPicUrl = url;
        public void SetOGOGURL(string url) => OGURL = url;
        public void SetSEOTitle(string seoTitle) => SeoTitle = seoTitle;
        public void SetSEODescription(string seoDescription) => SeoDescription = seoDescription;
        public void SetTableOfContent(string tableOfContent) => TableOfContent = tableOfContent;
        public void SetBlogStatus(BlogStatus status) => Status = status;
        public void SetMainBlogImage(string mainBlogImage) => MainImageUrl = mainBlogImage;
        public void SetMainBlogVideoThumbnailImage(string videoThumbnailImageUrl) => VideoThumbnailImageUrl = videoThumbnailImageUrl;

        public static string EXTRACT_TEXTs_FROM_HTML_AND_SET_DESCRIPTION(string content)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(content);
            var texts = document.DocumentNode.InnerText;
            return texts;
        }


        public void SetMetacontent(string metacontent)
        {
            MetaContent = metacontent;
        }

        public void SetScriptContent(string scriptContent)
        {
            ScriptContent = scriptContent;
        }

        public void SetUpdatedAt()
        {
            UpdatedAt = DateTime.Now;
        }

        public void SetSeoURL(string seoURL)
        {
            SeoURL = seoURL;
        }

        public void SetMainImage(string mainImageFile)
        {
            MainImageUrl = mainImageFile;
        }
    }

    public enum BlogStatus
    {
        Publish = 1,
        PreRelease = 2,
    }
    public enum BlogType
    {
        Blog = 1,
        Article
    }
}
