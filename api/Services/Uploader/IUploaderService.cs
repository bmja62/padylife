// Ignore Spelling: Uploader

using Microsoft.AspNetCore.Mvc;
using Services.Uploader.DTO;

namespace Services.Uploader
{
    public interface IUploaderService
    {
        Task<ServiceResult<UploadResultDTO>> UploadAsWebpV2(UploadV2DTO input);
        Task<IActionResult> GetBy(string guid, long userId);
        Task<ServiceResult> DeleteFile(string guid);
    }
}
