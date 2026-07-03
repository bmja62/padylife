using Entities.DynamicSiteSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.DynamicSiteSettings
{
    public class DynamicSiteSettingConfiguration : IEntityTypeConfiguration<DynamicSiteSetting>
    {
        public void Configure(EntityTypeBuilder<DynamicSiteSetting> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
