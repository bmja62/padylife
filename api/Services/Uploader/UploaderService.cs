// Ignore Spelling: Uploader

using Common;
using Common.Utilities;
using Data.Contracts;
using Entities.Medias;
using Entities.UploadedFiles;
using Entities.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.Uploader.DTO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;


namespace Services.Uploader
{
    /// <summary>
    /// سرویس آپلود فایل
    /// </summary>
    /// <param name="uploadedFileRepository"></param>
    /// <param name="userRepository"></param>
    /// <param name="userManager"></param>
    /// <param name="webHost"></param>
    /// <param name="configuration"></param>
    /// <param name="httpContextAccessor"></param>
    /// <param name="logger"></param>
    public class UploaderService
        (
        IRepository<UploadedFile> uploadedFileRepository,
        IRepository<Media> mediaRepository,
        IRepository<User> userRepository,
        UserManager<User> userManager,
        IWebHostEnvironment webHost,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        ILogger<UploaderService> logger
        ) : IScopedDependency, IUploaderService
    {


        public async Task<ServiceResult<UploadResultDTO>> UploadAsWebpV2(UploadV2DTO input)
        {
            try
            {
                var userId = httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();
                if (userId == 0)
                    return ServiceResult.BadRequest<UploadResultDTO>("لطفا ابتدا لاگین کنید");

                var wwwrootPath = Directory.GetCurrentDirectory();

                var configAttr = input.FileType.GetAttribute<UploadFileFormatAttribute>();
                var validFormats = configAttr.Formats.Split(',').ToList();
                var needOptimize = configAttr.Optimize;
                var withWatermark = configAttr.WithWatermark;
                var saveToFolder = configAttr.SaveToFolder;
                var resizeFolder = configAttr.ResizeFolder;
                var isSecure = configAttr.IsSecure;
                var hasDiffrentSize = configAttr.HasDifferentSize;
                var sizeLimitation = configAttr.SizeLimitation;
                var notImageExtentions = GetNotImageExtentions();

                var fileName = Path.GetFileName(input.File.FileName);
                var fileExtention = Path.GetExtension(input.File.FileName);

                var saveDirectory = saveToFolder.GetUploadFileDirectory();

                if (!Directory.Exists(saveDirectory))
                    Directory.CreateDirectory(saveDirectory);
                if (!validFormats.Contains(fileExtention.ToLower()))
                    return ServiceResult.BadRequest<UploadResultDTO>($"فرمت ارسالی اشتباه میباشد لطفا از این فرمت ها استفاده نمایید: {configAttr.Formats}");

                string imageGuid = Guid.NewGuid().ToString();


                string imageResultWithExtentionDefault = imageGuid + "_" + Path.GetFileNameWithoutExtension(input.File.FileName) + fileExtention;
                string imageResultDefaultWithEnCoder = imageGuid + "_" + Path.GetFileNameWithoutExtension(input.File.FileName) + ".webp";

                string imagePath = Path.Combine(saveDirectory, imageResultWithExtentionDefault);
                if (needOptimize && !notImageExtentions.Contains(fileExtention))
                    imagePath = Path.Combine(saveDirectory, imageResultDefaultWithEnCoder);

                var imageUrl = configuration.GetValue<string>("BaseUrl") + "/api/v1/Uploaders/Get/" + imageGuid;



                using (FileStream fs = new(imagePath, FileMode.Create))
                {
                    if (!notImageExtentions.Contains(fileExtention))
                    {
                        //var image = SixLabors.ImageSharp.Image.Load(input.File.OpenReadStream());
                        var decoderOptions = new SixLabors.ImageSharp.Formats.DecoderOptions
                        {
                            SkipMetadata = true,
                        };
                        // Load the image with decoder options to ignore metadata (for any format)
                        var image = SixLabors.ImageSharp.Image.Load(decoderOptions, input.File.OpenReadStream());



                        if (withWatermark)
                        {
                            var waterMarkLoc = Path.Combine(wwwrootPath, "Media", "Gallery", "Watermarks", "simagar-light.png");
                            if (File.Exists(waterMarkLoc))
                            {
                                var waterMark = SixLabors.ImageSharp.Image.Load(waterMarkLoc);

                                image.Mutate(o =>
                                {
                                    o.DrawImage(waterMark, new SixLabors.ImageSharp.Point(image.Width - waterMark.Width - 10, image.Height - waterMark.Height - 10), 0.5f);
                                });
                            }
                        }
                        if (needOptimize && !notImageExtentions.Contains(fileExtention))
                        {

                            image.Save(fs, new WebpEncoder()
                            {
                                Quality = 70,
                                FileFormat = WebpFileFormatType.Lossy,
                            });

                        }
                        if (hasDiffrentSize && fileExtention == ".gif")
                        {
                            var saveResizeDirectory = Path.Combine(saveDirectory, resizeFolder);
                            if (!Directory.Exists(saveResizeDirectory))
                                Directory.CreateDirectory(saveResizeDirectory);

                            var sizes = new (int Width, int Height)[]
                                {
                                (1272, 128),
                                (636, 128),
                                (250, 80)
                                };
                            image.Save(fs, new GifEncoder());

                            await ResizeImageAsync(image, saveResizeDirectory, imageGuid.ToString(), fileExtention, sizes);

                        }

                        await input.File.CopyToAsync(fs);
                    }
                    else
                    {
                        await input.File.CopyToAsync(fs);
                    }



                    new FileExtensionContentTypeProvider().TryGetContentType(imagePath, out string FileContentType);

                    await uploadedFileRepository.AddAsync(UploadedFile.Create(userId, imageUrl, imagePath, input.FileType, input.Title, input.Alt, input.Description, FileContentType, new FileInfo(imagePath).Length, imageGuid, isSecure), CancellationToken.None);
                }

                return ServiceResult.Ok(new UploadResultDTO
                {
                    Url = imageUrl
                });
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail<UploadResultDTO>(null, ex.Message, ApiResultStatusCode.LogicError);
            }

        }

        private List<string> GetNotImageExtentions() =>
            new List<string>
            {".mp4",".pdf",".doc",".mp3"};


        public async Task ResizeImageAsync(SixLabors.ImageSharp.Image image, string imagePath, string imageGuid, string fileExtention, params (int Width, int Height)[] sizes)
        {
            List<string> outPuts = new();
            foreach (var size in sizes)
            {
                using (SixLabors.ImageSharp.Image resizedImage = image.Clone(ctx => ctx.Resize(size.Width, size.Height)))
                {
                    string imageResult = imageGuid + "_" + $"size_{size.Width}x{size.Height}{fileExtention}";
                    string outputImagePath = Path.Combine(imagePath, imageResult);
                    await resizedImage.SaveAsync(outputImagePath, new GifEncoder());
                }
            }

        }


        public async Task<IActionResult> GetBy(string guid, long userId)
        {
            var uploadedFileInDb = await uploadedFileRepository.Table.Where(t => t.Guid == guid).FirstOrDefaultAsync();

            if (uploadedFileInDb is null)
                throw new FileNotFoundException("FileNotFound");

            if (uploadedFileInDb.IsSecure)
            {
                var user = await userRepository.Table.FirstOrDefaultAsync(t => t.Id == userId);
                bool isAdmin = await userManager.IsInRoleAsync(user, "Admin");
                if (!isAdmin)
                {
                    if (uploadedFileInDb.CreatedByUserId != userId)
                        throw new FileNotFoundException("FileNotFound");
                }
            }
            // Get proper content type
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(uploadedFileInDb.ImagePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            // Read the file
            var fileBytes = await File.ReadAllBytesAsync(uploadedFileInDb.ImagePath);

            // Return as FileContentResult
            return new FileContentResult(fileBytes, contentType)
            {
                FileDownloadName = Path.GetFileName(uploadedFileInDb.ImagePath)
            };
            //return new FileContentResult(await File.ReadAllBytesAsync(uploadedFileInDb.ImagePath), uploadedFileInDb.FileContent);
        }
        public async Task<ServiceResult> DeleteFile(string guid)
        {
            try
            {
                // Get the file record from database
                var uploadedFileInDb = await uploadedFileRepository.Table
                    .Where(t => t.Guid == guid)
                    .FirstOrDefaultAsync();

                if (uploadedFileInDb is null)
                {
                    return ServiceResult.Fail("File record not found in database");
                }

                // Delete the physical file
                try
                {
                    if (File.Exists(uploadedFileInDb.ImagePath))
                    {
                        File.Delete(uploadedFileInDb.ImagePath);
                    }
                    else
                    {
                        return ServiceResult.Fail("Physical file not found on disk");
                    }
                }
                catch (Exception ex)
                {
                    return ServiceResult.Fail($"Error deleting physical file: {ex.Message}"); // Return early if we can't delete the file
                }

                // Delete related media records if they exist
                if (!string.IsNullOrEmpty(uploadedFileInDb.ImageUrl))
                {
                    var mediaInDb = await mediaRepository.Table
                        .Where(t => t.URL != null && t.URL.Contains(uploadedFileInDb.ImageUrl))
                        .SingleOrDefaultAsync();

                    if (mediaInDb is not null)
                    {
                        await mediaRepository.DeleteAsync(mediaInDb, CancellationToken.None);
                    }
                }
                // Delete the file record from database
                await uploadedFileRepository.DeleteAsync(uploadedFileInDb, CancellationToken.None);

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Unexpected error: {ex.Message}");
            }
        }
    }
}
