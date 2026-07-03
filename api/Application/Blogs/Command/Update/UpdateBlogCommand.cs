using Application.Blogs.DTO;
using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Blogs;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Blogs.Command.Update
{
    public class UpdateBlogCommand(UpdateBlogCommandDTO input) : ICommand<ServiceResult<GetBlogByIdQueryResponseDTO>>
    {
        public UpdateBlogCommandDTO Input { get; } = input;

        public class UpdateBlogCommandHandler(IRepository<Blog> blogRepository, ICommandDispatcher commandDispatcher) : ICommandHandler<UpdateBlogCommand, ServiceResult<GetBlogByIdQueryResponseDTO>>
        {
            public async Task<ServiceResult<GetBlogByIdQueryResponseDTO>> Handle(UpdateBlogCommand request, CancellationToken cancellationToken)
            {

                var blog = await blogRepository.Table.Where(z => z.Id == request.Input.Id).FirstOrDefaultAsync();

                blog.SetSeoURL(request.Input.SeoURL);
                blog.SetTime(request.Input.SpendTimeForRead);
                blog.SetMetaAuthor(request.Input.MetaAuthor);
                blog.SetMetaKeywords(request.Input.MetaKeywords);
                blog.SetOGOGURL(request.Input.OGURL);
                blog.SetContent(request.Input.Content);
                blog.SetDesc(request.Input.Description);
                blog.SetShortDesc(request.Input.ShortDescription);
                blog.SetOGMainPicUrl(request.Input.OGMainPicUrl);
                blog.SetTitle(request.Input.Title);
                blog.SetCanonicalLink(request.Input.CanonicalLink);
                blog.SetBlogStatus(request.Input.Status);
                blog.SetMetacontent(request.Input.Metacontent);
                blog.SetScriptContent(request.Input.ScriptContent);
                blog.SetSEOTitle(request.Input.SeoTitle);
                blog.SetSEODescription(request.Input.SeoDescription);
                blog.SetUpdatedAt();
                blog.SetTableOfContent(request.Input.TableOfContent);
                blog.SetMainImage(request.Input.MainImageFile);
                blog.SetBlogCategoryId(request.Input.BlogCategoryId);

                await blogRepository.UpdateAsync(blog, cancellationToken);
                return ServiceResult.Ok(new GetBlogByIdQueryResponseDTO
                {
                    Id = blog.Id,
                    SeoURL = blog.SeoURL
                });
            }
        }
    }
}
