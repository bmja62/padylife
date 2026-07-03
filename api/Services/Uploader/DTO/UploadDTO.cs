// Ignore Spelling: Uploader DTO

using Entities.UploadedFiles;
using Microsoft.AspNetCore.Http;

namespace Services.Uploader.DTO
{
    public class UploadDTO
    {
        public string Path { get; set; }
        public IFormFile File { get; set; }
        public bool WithWatermark { get; set; }
    }

    public class UploadV2DTO
    {
        public IFormFile File { get; set; }
        public UploadType FileType { get; set; }
        public string Title { get; set; }
        public string Alt { get; set; }
        public string Description { get; set; }
    }
    public class UploadResultDTO
    {
        public string Url { get; internal set; }
    }
}
