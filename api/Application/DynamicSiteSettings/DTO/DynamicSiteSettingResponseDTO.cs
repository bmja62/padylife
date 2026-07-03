namespace Application.DynamicSiteSettings.DTO
{
    public record DynamicSiteSettingResponseDTO(long Id, string Key, string Type, string JsonValue, DateTime CreateDate, DateTime UpdateDate);

}
