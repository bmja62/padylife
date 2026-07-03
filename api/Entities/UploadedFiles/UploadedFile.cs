
using Entities.Common;
using Entities.Users;

namespace Entities.UploadedFiles;

public class UploadedFile : BaseEntity<long>
{
    public UploadedFile()
    {

    }
    public UploadedFile(long userId, string imageUrl, string imagePath, UploadType fileType, string title, string alt, string description, string fileContentType, long length, string imageGuid, bool isSecure)
    {
        CreatedByUserId = userId;
        ImageUrl = imageUrl;
        ImagePath = imagePath;
        FileType = fileType;
        Title = title;
        Alt = alt;
        Description = description;
        FileContent = fileContentType;
        FileSize = length;
        Guid = imageGuid;
        CreateAt = DateTime.Now;
        IsSecure = isSecure;
    }

    //FKs
    public long CreatedByUserId { get; set; }

    //Props
    public string Guid { get; set; }
    public string ImageUrl { get; set; }
    public string ImagePath { get; set; }
    public UploadType FileType { get; set; }
    public DateTime CreateAt { get; set; }
    public string Title { get; set; }
    public string Alt { get; set; }
    public string Description { get; set; }
    public string FileContent { get; set; }
    public long FileSize { get; set; }
    public bool IsSecure { get; set; }

    //Navigations
    public User CreatedByUser { get; set; }


    public static UploadedFile Create(long userId, string imageUrl, string imagePath, UploadType fileType, string title, string alt, string description, string fileContentType, long length, string imageGuid, bool isSecure)
    {
        UploadedFile uploadedFile = new(userId, imageUrl, imagePath, fileType, title, alt, description, fileContentType, length, imageGuid, isSecure);
        return uploadedFile;
    }
}

public enum UploadType
{
    [UploadFileFormat(Formats = ".png,.jpg,.webp,.jpeg", Optimize = true, WithWatermark = false, SaveToFolder = "Shop", IsSecure = false)]
    Shop = 1,
    [UploadFileFormat(Formats = ".png,.jpg,.webp,.jpeg", Optimize = true, WithWatermark = false, SaveToFolder = "Documents", IsSecure = true)]
    Documents = 2,
    [UploadFileFormat(Formats = ".png,.jpg,.webp,.jpeg", Optimize = true, WithWatermark = false, SaveToFolder = "CapchaImages", IsSecure = false)]
    CapchaImages = 3,
    [UploadFileFormat(Formats = ".png,.jpg,.webp,.jpeg", Optimize = true, WithWatermark = false, SaveToFolder = "MarketBanners", IsSecure = false)]
    MarketBanner = 4,
    [UploadFileFormat(Formats = ".gif", Optimize = false, WithWatermark = false, SaveToFolder = "MarketGifs", ResizeFolder = "MarketResizeGifs", IsSecure = false, HasDifferentSize = true, SizeLimitation = "small,medium,large")]
    BannerGif = 5,
    [UploadFileFormat(Formats = ".png,.jpg,.webp,.jpeg", Optimize = true, WithWatermark = false, SaveToFolder = "Shop", IsSecure = false)]
    Blog = 6,
    [UploadFileFormat(Formats = ".png,.jpg,.webp,.jpeg,.mp4,.pdf,.doc", Optimize = false, WithWatermark = false, SaveToFolder = "ExcersieDocuments", IsSecure = false)]
    ExcersieDocument = 7,
    [UploadFileFormat(Formats = ".png,.jpg,.webp,.jpeg,.mp4,.pdf,.doc", Optimize = false, WithWatermark = false, SaveToFolder = "StepOption", IsSecure = false)]
    StepOption = 8,
    [UploadFileFormat(Formats = ".png,.jpg,.webp,.jpeg", Optimize = false, WithWatermark = false, SaveToFolder = "ProductMain", IsSecure = false)]
    ProductMain = 9,
    [UploadFileFormat(Formats = ".png,.jpg,.webp,.jpeg", Optimize = false, WithWatermark = false, SaveToFolder = "ProductMain", IsSecure = false)]
    ProductGallery = 10,
    [UploadFileFormat(Formats = ".png,.jpg,.webp,.jpeg", Optimize = false, WithWatermark = false, SaveToFolder = "ProductVariantMain", IsSecure = false)]
    ProductVariantMain = 11,
    [UploadFileFormat(Formats = ".png,.jpg,.webp,.jpeg", Optimize = false, WithWatermark = false, SaveToFolder = "ProductVariantGallery", IsSecure = false)]
    ProductVariantGallery = 12,
    [UploadFileFormat(Formats = ".mp3", Optimize = false, WithWatermark = false, SaveToFolder = "DailyFeelingVoices", IsSecure = false)]
    DailyFeelingVoice = 13,
    [UploadFileFormat(Formats = ".png,.jpg,.webp,.jpeg", Optimize = false, WithWatermark = false, SaveToFolder = "Challenges", IsSecure = false)]
    Challenge = 14,
    [UploadFileFormat(Formats = ".png,.jpg,.webp,.jpeg", Optimize = false, WithWatermark = false, SaveToFolder = "BlogCategories", IsSecure = false)]
    BlogCategory = 15,
    [UploadFileFormat(Formats = ".png,.jpg,.webp,.jpeg", Optimize = false, WithWatermark = false, SaveToFolder = "ProfileImages", IsSecure = false)]
    ProfileImage = 16,
    [UploadFileFormat(Formats = ".png,.jpg,.webp,.jpeg", Optimize = false, WithWatermark = false, SaveToFolder = "Excersies", IsSecure = false)]
    Excersie = 17,
    [UploadFileFormat(Formats = ".png,.jpg,.webp,.jpeg", Optimize = false, WithWatermark = false, SaveToFolder = "Plans", IsSecure = false)]
    Plan = 18,
    [UploadFileFormat(Formats = ".png,.jpg,.webp,.jpeg", Optimize = false, WithWatermark = false, SaveToFolder = "ProductCategories", IsSecure = false)]
    ProductCategory = 19,
    [UploadFileFormat(Formats = ".png", Optimize = false, WithWatermark = false, SaveToFolder = "Medals", IsSecure = false)]
    Medal = 20,
    [UploadFileFormat(Formats = ".mp4,.png,.jpg,.webp,.jpeg", Optimize = true, WithWatermark = false, SaveToFolder = "CkEditors", IsSecure = false)]
    CkEditor = 21,
    [UploadFileFormat(Formats = ".png,.jpg,.webp,.jpeg", Optimize = true, WithWatermark = false, SaveToFolder = "Motivation", IsSecure = false)]
    Motivation = 22




}

public enum SizeType
{
    Small,
    Medium,
    Large,
}