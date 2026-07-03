using Application.Blogs.Query.ForWeb.DTOs;
using Application.Cqrs.Queris;
using Common.GridResults;
using Data.Contracts;
using Entities.Blogs;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Blogs.Query.ForWeb.GetAllBlogForWeb
{
    public class GetAllBlogsForWebQuery : GlobalGrid, IQuery<ServiceResult<GlobalGridResult<GetAllForWebDTO>>>
    {
        public GetAllBlogsForWebQuery(int pageNumber, int count, string search, long? blogCategoryId)
        {
            PageNumber = pageNumber;
            Count = count;
            Search = search;
            BlogCategoryId = blogCategoryId;
        }
        public string Search { get; set; }
        public long? BlogCategoryId { get; }
    }

    public class GetAllBlogsForWebQueryHandler(IRepository<Blog> blogRepository) : IQueryHandler<GetAllBlogsForWebQuery, ServiceResult<GlobalGridResult<GetAllForWebDTO>>>
    {
        public async Task<ServiceResult<GlobalGridResult<GetAllForWebDTO>>> Handle(GetAllBlogsForWebQuery request, CancellationToken cancellationToken)
        {
            var query = blogRepository.Table.Where(a => a.Status == BlogStatus.Publish && !a.IsDeleted);


            if (request.BlogCategoryId.HasValue)
                query = query.Where(t => t.BlogCategoryId == request.BlogCategoryId.Value);

            if (!string.IsNullOrEmpty(request.Search))
                query = query.Where(t =>
                t.Title.Contains(request.Search) ||
                t.SeoURL.Contains(request.Search) ||
                t.SeoTitle.Contains(request.Search) ||
                t.ShortDescription.Contains(request.Search)
                );
            var data = await query.Select(z => new GetAllForWebDTO
            {
                Id = z.Id,
                Title = z.Title,
                SeoURL = z.SeoURL,
                BlogCategoryId = z.BlogCategoryId,
                BlogCategoryTitle = z.BlogCategory.Title,
                ShortDescription = z.ShortDescription,
                Image = z.MainImageUrl,
            }).Skip(request.Skip).Take(request.Take).ToListAsync();
            var totalCount = await query.CountAsync();

            var result = ServiceResult.Ok(new GlobalGridResult<GetAllForWebDTO>
            {
                Data = data,
                TotalCount = totalCount
            });
            return result;
        }
    }
}
