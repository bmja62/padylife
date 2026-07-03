using Application.Blogs.DTO;
using Application.Cqrs.Commands;
using Common.Utilities;
using Data.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Uploader;

namespace Application.Blogs.Command.Create
{
    public class CreateBlogCommand(CreateBlogCommandDTO input) : ICommand<ServiceResult<GetBlogByIdQueryResponseDTO>>
    {
        public CreateBlogCommandDTO Input { get; } = input;

        //handler
        public class CreateBlogCommandHandler(IRepository<Entities.Blogs.Blog> blogRepository, IHttpContextAccessor httpContextAccessor, ICommandDispatcher commandDispatcher, IUploaderService uploaderService) : ICommandHandler<CreateBlogCommand, ServiceResult<GetBlogByIdQueryResponseDTO>>
        {
            public async Task<ServiceResult<GetBlogByIdQueryResponseDTO>> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
            {
                var userId = httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();
                var exist = await blogRepository.TableNoTracking.AnyAsync(z => z.Title == request.Input.Title);

                if (exist)
                    return ServiceResult.BadRequest<GetBlogByIdQueryResponseDTO>("بلاگی با این تایتل ذخیره می‌باشد");

                var duplicatedSeoUrl = await blogRepository.TableNoTracking.AnyAsync(z => z.SeoURL == request.Input.SeoURL);
                if (duplicatedSeoUrl)
                    return ServiceResult.BadRequest<GetBlogByIdQueryResponseDTO>("بلاگی با این تایتل ذخیره می‌باشد");

                var blog = Entities.Blogs.Blog.CreateBlog(
                    request.Input.BlogCategoryId,
                    request.Input.CanonicalLink,
                    request.Input.SpendTimeForRead,
                    request.Input.Title,
                    request.Input.Content,
                    request.Input.ShortDescription,
                    request.Input.MetaKeywords,
                    request.Input.MetaAuthor,
                    request.Input.OGTitle,
                    request.Input.OGURL,
                    request.Input.OGMainPicUrl,
                    userId,
                    request.Input.SeoURL,
                    request.Input.SeoTitle,
                    request.Input.SeoDescription,
                    request.Input.TableOfContent,
                    request.Input.ScriptContent,
                    request.Input.Metacontent,
                    request.Input.MainImageFile,
                    request.Input.Type,
                    request.Input.Status
                    );

                await blogRepository.AddAsync(blog, cancellationToken);

                return ServiceResult.Ok(new GetBlogByIdQueryResponseDTO
                {
                    Id = blog.Id,
                    SeoURL = blog.SeoURL
                });
            }
        }
    }
}
