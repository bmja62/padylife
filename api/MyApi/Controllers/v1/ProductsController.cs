using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Application.Products.Commands;
using Application.Products.DTOs;
using Application.Products.Queries;
using Asp.Versioning;
using Common.GridResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1
{
    /// <summary>
    /// محصولات
    /// </summary>
    /// <param name="commandDispatcher"></param>
    /// <param name="queryDispatcher"></param>
    [ApiVersion("1")]
    public class ProductsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : BaseController
    {
        ///// <summary>
        ///// ساخت محصول
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Authorize]
        //public async Task<ApiResult> Create([FromForm] CreateProductCommandDTO input) =>
        //   (await commandDispatcher
        //    .SendAsync(new CreateProductCommand(input)))
        //    .ToApiResult();

        /// <summary>
        /// تغییر عکس اصلی محصول
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> ChangeMainImage([FromForm] ChangeMainImageDTO input) =>
           (await commandDispatcher
            .SendAsync(new ChangeMainImageCommand(input)))
            .ToApiResult();

        /// <summary>
        /// افزودن عکس به گالری محصول
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ApiResult> AddImageToGallery([FromForm] AddImageToGalleryDTO input) =>
           (await commandDispatcher
            .SendAsync(new AddImageToGalleryCommand(input)))
            .ToApiResult();

        /// <summary>
        /// ویرایش محصول
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ApiResult> Update(long id, [FromBody] UpdateProductCommandDTO input) =>
           (await commandDispatcher
            .SendAsync(new UpdateProductCommand(id, input)))
            .ToApiResult();

        /// <summary>
        ///  حذف محصول
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ApiResult> Delete(long id) =>
           (await commandDispatcher
            .SendAsync(new DeleteProductCommand(id)))
            .ToApiResult();


        /// <summary>
        /// دریافت محصول
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ApiResult<GetProductByIdDTO>> GetById(long id) =>
           (await queryDispatcher
            .SendAsync(new GetProductByIdQuery(id)))
            .ToApiResult();

        /// <summary>
        /// دریافت محصولات
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<GlobalGridResult<GetAllProductsDTO>>> GetAll([FromQuery] GetAllProductsQueryDTO input) =>
           (await queryDispatcher
            .SendAsync(new GetAllProductsQuery(input)))
            .ToApiResult();



        /// <summary>
        /// افزودن ویژگی‌های عمومی محصول
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult> AddProductAttributeValue([FromBody] AddProductAttributeValueDTO input) =>
           (await commandDispatcher
            .SendAsync(new AddProductAttributeValueCommand(input)))
            .ToApiResult();

        /// <summary>
        /// حذف ویژگی‌های عمومی محصول ساده
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult> RemoveProductAttributeValue([FromBody] RemoveProductAttributeValueDTO input) =>
           (await commandDispatcher
            .SendAsync(new RemoveProductAttributeValueCommand(input)))
            .ToApiResult();

        /// <summary>
        ///  حذف ویژگی‌های عمومی محصول متغییر
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("variants/attributes/remove")]
        public async Task<ApiResult> RemoveVariantAttributeValue([FromBody] RemoveVariantAttributeValueDTO input) =>
   (await commandDispatcher
    .SendAsync(new RemoveVariantAttributeValueCommand(input)))
    .ToApiResult();

        /// <summary>
        /// مرحله 1: ایجاد محصول پایه
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("basic")]
        [Authorize]
        public async Task<ApiResult<GetProductByIdDTO>> CreateBasic([FromBody] CreateProductBasicInfoDTO input) =>
           await (await commandDispatcher
            .SendAsync(new CreateProductBasicCommand(input)))
                 .ToApiResult()
            .ExecuteOrReturn((r) => GetById(r.Id));

        /// <summary>
        /// مرحله 2: افزودن ویژگی‌ها
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("{productId}/attributes")]
        [Authorize]
        public async Task<ApiResult> AddAttributes(long productId, [FromBody] List<ProductAttributeValueDTO> input) =>
           (await commandDispatcher
            .SendAsync(new AddProductAttributesCommand(productId, input)))
            .ToApiResult();

        /// <summary>
        /// مرحله 3: قیمت‌گذاری
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("{productId}/pricing")]
        [Authorize]
        public async Task<ApiResult> SetPricing(long productId, [FromBody] SetProductPricingDTO input) =>
           (await commandDispatcher
            .SendAsync(new SetProductPricingCommand(productId, input)))
            .ToApiResult();


        /// <summary>
        /// مرحله 4: افزودن واریانت‌ها (برای محصولات متغیر)
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("{productId}/variants")]
        [Authorize]
        public async Task<ApiResult> AddVariants(long productId, [FromBody] List<CreateProductVariantDTO> input) =>
           (await commandDispatcher
            .SendAsync(new AddProductVariantsCommand(productId, input)))
            .ToApiResult();



    }
}
