using System.ComponentModel.DataAnnotations;

namespace Entities.Medias
{
    public enum MediaTypes
    {
        OurTeam,
        Projects,
        CKEditor,
        Podcast,
        SiteBanner,
        DynamicPageBanner,
        Video,
        BlogMainImage,
        BlogVideoThumbnail,
        BlogVideo,
        BlogAudio,
        ProjectMainPicture,
        ProjectContractorPicture,
        ProjectGallery,
        [Display(Name = "تصویر اصلی محصول")]
        ProductMain,
        [Display(Name = "گالری محصول")]
        ProductGallery,
        [Display(Name = "تصویر اصلی محصول متغییر")]
        ProductVariantMain,
        [Display(Name = "گالری محصول متغییر")]
        ProductVariantGallery
    }
}
