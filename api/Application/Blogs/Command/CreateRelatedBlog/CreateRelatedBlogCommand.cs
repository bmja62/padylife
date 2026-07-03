using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Blogs;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Blogs.Command.CreateRelatedBlog;

public class CreateRelatedBlogCommand : ICommand<ServiceResult>
{
    public CreateRelatedBlogCommand(long blogId, List<long> relatedIds)
    {
        BlogId = blogId;
        RelatedIds = relatedIds;
    }

    public long BlogId { get; }
    public List<long> RelatedIds { get; }
}

public class CreateRelatedBlogHandler(IRepository<RelatedBlog> relatedBlogRepository, IRepository<Blog> blogRepository) : ICommandHandler<CreateRelatedBlogCommand, ServiceResult>
{

    public async Task<ServiceResult> Handle(CreateRelatedBlogCommand request, CancellationToken cancellationToken)
    {
        //یافتن بلاگی که میخواهیم به ان بلاگ های مشابه را نسبت دهیم
        var relatedBlogsInDb = await relatedBlogRepository.Table.Where(x => x.BlogId == request.BlogId).ToListAsync();

        //بررسی بلاگ های موجود در بلاگاصلی و حذف انها
        if (relatedBlogsInDb != null)
        {
            await relatedBlogRepository.DeleteRangeAsync(relatedBlogsInDb, cancellationToken);
        }

        var relatedBlogs = request.RelatedIds.Select(t => new RelatedBlog
        {
            BlogId = request.BlogId,
            RelatedBlogId = t
        }).ToList();

        if (relatedBlogs is not null && relatedBlogs.Count > 0)
            await relatedBlogRepository.AddRangeAsync(relatedBlogs, cancellationToken);

        return ServiceResult.Ok();
    }
}