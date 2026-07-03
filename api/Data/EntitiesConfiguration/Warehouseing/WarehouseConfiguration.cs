using Common.Shemas;
using Entities.Warehouseing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Warehouseing
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.ToTable(nameof(Warehouse), Schema.WareHouse);

            builder.Property(w => w.Name).IsRequired().HasMaxLength(100);
            builder.Property(w => w.Code).IsRequired().HasMaxLength(20);
            builder.Property(w => w.Address).IsRequired().HasMaxLength(200);
            builder.Property(w => w.ContactPhone).HasMaxLength(20);
            builder.Property(w => w.ManagerName).HasMaxLength(50);
            builder.HasIndex(w => w.Code).IsUnique();

            builder.HasMany(w => w.Zones)
                .WithOne(z => z.Warehouse)
                .HasForeignKey(z => z.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(w => w.Inventories)
                .WithOne(i => i.Warehouse)
                .HasForeignKey(i => i.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
