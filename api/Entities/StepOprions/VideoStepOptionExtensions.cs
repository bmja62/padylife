namespace Entities.StepOprions
{
    public static class VideoStepOptionExtensions
    {
        public static VideoStepOption EnableDownload(this VideoStepOption option)
        {
            option.AllowDownload = true;
            return option;
        }

        public static VideoStepOption WithThumbnail(this VideoStepOption option, string thumbnailUrl)
        {
            option.ThumbnailUrl = thumbnailUrl;
            return option;
        }
    }

}
