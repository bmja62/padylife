using Application.Blogs.DTO;
using Application.Cqrs.Queris;
using Data.Contracts;
using Entities.Blogs;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Blogs.Query
{
    public class GetBlogByIdQuery(long Id) : IQuery<ServiceResult<GetBlogByIdQueryResponseDTO>>
    {
        public long Id { get; } = Id;


        public class GetBlogByIdQueryHandler(IRepository<Blog> blogRepository, IRepository<RelatedBlog> relatedBlogRepository) : IQueryHandler<GetBlogByIdQuery, ServiceResult<GetBlogByIdQueryResponseDTO>>
        {
            public async Task<ServiceResult<GetBlogByIdQueryResponseDTO>> Handle(GetBlogByIdQuery request, CancellationToken cancellationToken)
            {
                var repo = blogRepository.Table.Include(x => x.RelatedBlogs).Where(z => z.Id == request.Id);
                var blog = await repo.Select(z => new GetBlogByIdQueryResponseDTO
                {
                    ShortDescription = z.ShortDescription,
                    OGURL = z.OGURL,
                    OGTitle = z.OGTitle,
                    Content = z.Content,
                    Id = z.Id,
                    SeoURL = z.SeoURL,
                    BlogCategoryId = z.BlogCategoryId,
                    BlogCategoryTitle = z.BlogCategory.Title,
                    Title = z.Title,
                    SeoTitle = z.SeoTitle,
                    SeoDescription = z.SeoDescription,
                    MainImageUrl = z.MainImageUrl,
                    SpendTimeForRead = z.SpendTimeToRead,
                    Status = z.Status,
                    TableOfContent = z.TableOfContent,
                    CreatedAt = z.CreatedAt,
                    CanonicalLink = z.CanonicalLink,
                    ScriptContent = z.ScriptContent,
                    MetaContent = z.MetaContent,
                    MetaAuthor = z.MetaAuthor,
                    MetaKeywords = z.MetaKeywords,
                    VideoThumbnailImageUrl = z.VideoThumbnailImageUrl,
                    RelatedBlogs = relatedBlogRepository.Table.Where(b => b.BlogId == z.Id).Select(x => new GetBlogRelatedDTO
                    {
                        Id = x.RelatedBlogId,
                        MainImageUrl = x.Relatedblog.MainImageUrl,
                        SeoURL = x.Relatedblog.SeoURL,
                        Title = x.Relatedblog.Title,
                        SeoDescription = x.Relatedblog.SeoDescription,
                    }).ToList()

                }).FirstOrDefaultAsync();

                var metaTags = Blog.CreateBlogMetaTags(await repo.FirstOrDefaultAsync());
                blog.MetaTags = metaTags;

                return ServiceResult.Ok(blog);
            }
        }
    }


}
