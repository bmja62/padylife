using Application.Blogs.DTO;
using Application.Cqrs.Queris;
using Common.GridResults;
using Data.Contracts;
using Entities.Blogs;
using Microsoft.EntityFrameworkCore;

namespace Application.Blogs.Query
{
    public class GetAllBlogsQuery : GlobalGrid, IQuery<GlobalGridResult<GetAllQueryResponseDTO>>
    {

        public string SerachByTitle { get; set; }
        public long? BlogCategoryId { get; set; }
        public GetAllBlogsQuery(int pageNumber, int count, string serachByTitle, long? blogCategoryId)
        {

            PageNumber = pageNumber;
            Count = count;
            SerachByTitle = serachByTitle;
            BlogCategoryId = blogCategoryId;
        }



        public class GetAllBlogsQueryHandler(IRepository<Blog> blogRepo) : IQueryHandler<GetAllBlogsQuery, GlobalGridResult<GetAllQueryResponseDTO>>
        {
            public async Task<GlobalGridResult<GetAllQueryResponseDTO>> Handle(GetAllBlogsQuery request, CancellationToken cancellationToken)
            {
                request.DefaultPagination();
                var query = blogRepo.TableNoTracking;
                if (!string.IsNullOrEmpty(request.SerachByTitle))
                    query = query.Where(a => a.Title.Contains(request.SerachByTitle));
                if (request.BlogCategoryId.HasValue)
                    query = query.Where(t => t.BlogCategoryId == request.BlogCategoryId.Value);
                var data = await query.OrderByDescending(t => t.UpdatedAt).Select(z => new GetAllQueryResponseDTO
                {
                    Content = z.Content,
                    SeoURL = z.SeoURL,
                    BlogCategoryId = z.BlogCategoryId,
                    BlogCategoryTitle = z.BlogCategory.Title,
                    ShortDescription = z.ShortDescription,
                    MainImageUrl = z.MainImageUrl,
                    Id = z.Id,
                    Title = z.Title,
                    CreatedAt = z.CreatedAt,
                    Status = z.Status
                }).Skip(request.Skip).Take(request.Take).ToListAsync();
                var totalCount = await query.CountAsync();
                return new GlobalGridResult<GetAllQueryResponseDTO> { TotalCount = totalCount, Data = data };
            }
        }
    }
}
