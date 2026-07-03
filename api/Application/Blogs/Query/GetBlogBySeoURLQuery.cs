using Application.Blogs.DTO;
using Application.Cqrs.Queris;
using Data.Contracts;
using Entities.Blogs;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Blogs.Query
{
    public class GetBlogBySeoURLQuery(string SeoURL) : IQuery<ServiceResult<GetBlogByIdQueryResponseDTO>>
    {
        public string SeoURL { get; } = SeoURL;

        public class GetBlogByTitleQueryHandler(IRepository<Blog> blogRepository, IRepository<RelatedBlog> relatedBlogRepository, IRepository<User> userRepository) : IQueryHandler<GetBlogBySeoURLQuery, ServiceResult<GetBlogByIdQueryResponseDTO>>
        {
            public async Task<ServiceResult<GetBlogByIdQueryResponseDTO>> Handle(GetBlogBySeoURLQuery request, CancellationToken cancellationToken)
            {
                var blogDB = await blogRepository.TableNoTracking.Where(z => z.SeoURL.ToLower() == request.SeoURL.ToLower()).FirstOrDefaultAsync();
                var blog = await blogRepository.TableNoTracking.Where(z => z.SeoURL.ToLower() == request.SeoURL.ToLower()).Include(x => x.RelatedBlogs).Select(z => new GetBlogByIdQueryResponseDTO
                {
                    ShortDescription = z.ShortDescription,
                    OGURL = z.OGURL,
                    OGTitle = z.OGTitle,
                    Content = z.Content,
                    Id = z.Id,
                    SeoURL = z.SeoURL,
                    Title = z.Title,
                    SeoTitle = z.SeoTitle,
                    SeoDescription = z.SeoDescription,
                    MainImageUrl = z.MainImageUrl,
                    SpendTimeForRead = z.SpendTimeToRead,
                    Status = z.Status,
                    TableOfContent = z.TableOfContent,
                    CreatedAt = z.CreatedAt,
                    BlogCategoryId = z.BlogCategoryId,
                    BlogCategoryTitle = z.BlogCategory.Title,
                    CanonicalLink = z.CanonicalLink,
                    ScriptContent = z.ScriptContent,
                    MetaContent = z.MetaContent,
                    MetaAuthor = z.MetaAuthor,
                    MetaKeywords = z.MetaKeywords,
                    VideoThumbnailImageUrl = z.VideoThumbnailImageUrl,
                    Author = new AuthorDTO
                    {
                        FullName = z.User.FullName,
                    },
                    RelatedBlogs = relatedBlogRepository.Table.Where(b => b.BlogId == z.Id).Select(x => new GetBlogRelatedDTO
                    {
                        Id = x.RelatedBlogId,
                        MainImageUrl = x.Relatedblog.MainImageUrl,
                        SeoURL = x.Relatedblog.SeoURL,
                        Title = x.Relatedblog.Title,
                        SeoDescription = x.Relatedblog.SeoDescription,
                    }).ToList()
                }).FirstOrDefaultAsync();

                blog.MetaTags = Blog.CreateBlogMetaTags(blogDB);
                return ServiceResult.Ok(blog);
            }
        }
    }
}
