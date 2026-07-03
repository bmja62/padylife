using Common.Shemas;
using Entities.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Location
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable(nameof(Country), Schema.Location);
            builder.HasMany(t => t.Provinces)
                .WithOne(t => t.Country)
                .HasForeignKey(t => t.CountryId);

            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }


}
