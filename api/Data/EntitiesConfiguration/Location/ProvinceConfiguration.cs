using Common.Shemas;
using Entities.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Location
{
    public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
    {
        public void Configure(EntityTypeBuilder<Province> builder)
        {
            builder.ToTable(nameof(Province), Schema.Location);
            builder.HasMany(t => t.Cities)
             .WithOne(t => t.Province)
             .HasForeignKey(t => t.ProvinceId);

            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }


}
