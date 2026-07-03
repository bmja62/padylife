using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.Blogs;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Blogs.Command.Delete
{
    public class DeleteBlogCommand(long Id, string Title) : ICommand<ServiceResult>
    {
        public long Id { get; } = Id;
        public string Title { get; } = Title;

        public class DeleteBlogCommandHandler(IRepository<Blog> blogRepository) : ICommandHandler<DeleteBlogCommand, ServiceResult>
        {
            public async Task<ServiceResult> Handle(DeleteBlogCommand request, CancellationToken cancellationToken)
            {
                var blog = await blogRepository.Table.Where(z => z.Id == request.Id).FirstOrDefaultAsync();

                await blogRepository.SoftDeleteAsync(blog, cancellationToken);
                return ServiceResult.Ok();
            }
        }
    }
}
