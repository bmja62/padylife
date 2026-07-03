using Application.Cqrs.Commands;
using Application.Cqrs.Queris;
using Application.UploadedFiles.Commands.UploadAsWebp;
using Application.UploadedFiles.Queries.GetUploadedFiles;
using Application.Uploaders.Command;
using Application.Uploaders.Command.UploadWithoutObjectId;
using Application.Uploaders.DTOs;
using Asp.Versioning;
using Common.Roles;
using Entities.Medias;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Uploader.DTO;
using WebFramework.Api;

namespace PadyLife.Api.Controllers.v1;

/// <summary>
/// کنترلر آپاود
/// </summary>
[ApiVersion("1")]
public class UploadersController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : BaseController
{
    /// <summary>
    /// آپلود
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    public async Task<ApiResult<UploadResultDTO>> Upload([FromForm] UploadV2DTO input) =>
     (await commandDispatcher.SendAsync(new UploadAsWebpCommand(input.File, input.FileType, input.Title, input.Alt, input.Description))).ToApiResult();

    /// <summary>
    /// دریافت فایل
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("{guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(string guid, [FromQuery] long userId)
                => await queryDispatcher.SendAsync(new GetUploadedFileQuery(guid, userId));


    /// <summary>
    /// حذف فایل
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    [HttpDelete("{guid}")]
    [Authorize(Roles = $"{UserRoleNames.Admin}")]
    public async Task<ApiResult> Delete(string guid)
                => (await commandDispatcher.SendAsync(new DeleteFileCommand(guid))).ToApiResult();

    /// <summary>
    /// آپلود عکس بدون ذخیره سازی در مدیا
    /// </summary>
    /// <param name="inputDTO"></param>
    /// <returns></returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    [IgnoreApiResultFilter]
    public async Task<ActionResult> UploadCkEditor([FromForm] UploadCkEditorDTO inputDTO)
    {
        var result = (await commandDispatcher.SendAsync(new UploadWithoutObjectIdCommand(inputDTO.File, MediaTypes.CKEditor, inputDTO.WithWatermark, Entities.UploadedFiles.UploadType.CkEditor))).ToApiResult();
        if (result.IsSuccess)
        {
            var ts = result.CastLinkToString(result.Data);
            return Ok(new { Link = result.CastLinkToString(result.Data) });
        }
        else
        {
            var ts = result.CastLinkToString(result.Data);
            return BadRequest(new { Error = result.Message });
        }



    }

}
