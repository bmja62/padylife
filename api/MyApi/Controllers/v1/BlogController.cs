using Application.Blogs.Command.Create;
using Application.Blogs.Command.CreateRelatedBlog;
using Application.Blogs.Command.Delete;
using Application.Blogs.Command.Update;
using Application.Blogs.DTO;
using Application.Blogs.Query;
using Application.Blogs.Query.ForWeb.DTOs;
using Application.Blogs.Query.ForWeb.GetAllBlogForWeb;
using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Asp.Versioning;
using Common.GridResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// کنترلر بلاگ
    /// </summary>
    [ApiVersion("1")]
    public class BlogController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : BaseController
    {

        /// <summary>
        /// ساخت بلاگ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult<GetBlogByIdQueryResponseDTO>> Create([FromBody] CreateBlogCommandDTO input) =>
           await (await commandDispatcher
            .SendAsync(new CreateBlogCommand(input)))
            .ToApiResult()
            .ExecuteOrReturn((r) => GetBySeoURL(r.SeoURL));


        /// <summary>
        /// ویرایش بلاگ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [AllowAnonymous]
        public async Task<ApiResult> Update([FromBody] UpdateBlogCommandDTO input) =>
            await (await commandDispatcher
            .SendAsync(new UpdateBlogCommand(input)))
            .ToApiResult()
            .ExecuteOrReturn((r) => GetBySeoURL(r.SeoURL));


        /// <summary>
        /// حذف بلاگ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        [HttpDelete]
        [AllowAnonymous]
        public async Task<ApiResult> Delete(long id, string title) => (await commandDispatcher.SendAsync(new DeleteBlogCommand(id, title))).ToApiResult();


        /// <summary>
        /// گرفتن بلاگ
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet("[action]/{id}")]
        [AllowAnonymous]
        public async Task<ApiResult<GetBlogByIdQueryResponseDTO>> GetBy(long id) => (await queryDispatcher.SendAsync(new GetBlogByIdQuery(id))).ToApiResult();


        /// <summary>
        /// لیست بلاگ ها
        /// </summary>
        /// <param name="input"></param>
        /// <param name="searchByTitle"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResult<GlobalGridResult<GetAllQueryResponseDTO>>> GetAll([FromQuery] GlobalGrid input, string searchByTitle, long? blogCategoryId) =>
            await queryDispatcher.SendAsync(new GetAllBlogsQuery(input.PageNumber.Value, input.Count.Value, searchByTitle, blogCategoryId));


        /// <summary>
        /// گرفتن بلاگ با سو یوآرال
        /// </summary>
        /// <param name="input"></param>
        /// <param name="seoURL"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResult<GetBlogByIdQueryResponseDTO>> GetBySeoURL(string seoURL) => (await queryDispatcher.SendAsync(new GetBlogBySeoURLQuery(seoURL))).ToApiResult();




        /// <summary>
        ///  لیست بلاگ ها برای کلاینت
        /// </summary>
        /// <param name="input"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResult<GlobalGridResult<GetAllForWebDTO>>> GetAllForWeb([FromQuery] GlobalGrid input, string search, long? blogCategoryId) =>
            (await queryDispatcher.SendAsync(new GetAllBlogsForWebQuery(input.PageNumber.Value, input.Count.Value, search, blogCategoryId))).ToApiResult();

        /// <summary>
        /// ساخت یا ویرایش بلگ ها مرتبط
        /// </summary>
        /// <param name="blogId"></param>
        /// <param name="relatedIds"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResult> CreateOrUpdateRelatedBlog(long blogId, List<long> relatedIds) =>
           (await commandDispatcher.SendAsync(new CreateRelatedBlogCommand(blogId, relatedIds))).ToApiResult();

    }
}
