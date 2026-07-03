using Common.Shemas;
using Entities.Warehouseing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Warehouseing
{
    public class WarehouseZoneConfiguration : IEntityTypeConfiguration<WarehouseZone>
    {
        public void Configure(EntityTypeBuilder<WarehouseZone> builder)
        {
            builder.ToTable(nameof(WarehouseZone), Schema.WareHouse);

            builder.Property(z => z.Name).IsRequired().HasMaxLength(50);
            builder.Property(z => z.Code).IsRequired().HasMaxLength(20);
            builder.HasIndex(z => new { z.WarehouseId, z.Code }).IsUnique();

            builder.HasMany(z => z.Inventories)
                .WithOne(i => i.Zone)
                .HasForeignKey(i => i.ZoneId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
