using Common.GridResults;

namespace Application.DynamicSiteSettings.DTO
{
    public class GetAllDynamicSiteSettingsByTypeRequestDto : GlobalGrid
    {
        public string Search { get; set; }
        public string Type { get; set; }

    }
}
