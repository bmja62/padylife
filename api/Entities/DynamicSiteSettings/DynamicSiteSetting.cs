using Entities.Common;

namespace Entities.DynamicSiteSettings
{
    public class DynamicSiteSetting : BaseEntity<long>
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public string JsonValue { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }

}
