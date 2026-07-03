using Common.Shemas;
using Entities.Addresses.ECommerce.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Addresses
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable(nameof(Address), Schema.Location);

            builder.HasOne(t => t.User)
                .WithMany(t => t.Addresses)
                .HasForeignKey(t => t.UserId);

            builder.HasOne(t => t.Country)
                .WithMany(t => t.Addresses)
                .HasForeignKey(t => t.CountryId);

            builder.HasOne(t => t.Province)
                .WithMany(t => t.Addresses)
                .HasForeignKey(t => t.ProvinceId);

            builder.HasOne(t => t.City)
                .WithMany(t => t.Addresses)
                .HasForeignKey(t => t.CityId);

            builder.HasMany(t => t.Baskets)
                .WithOne(t => t.Address)
                .HasForeignKey(t => t.AddressId);

            builder.HasMany(t => t.Orders)
               .WithOne(t => t.Address)
               .HasForeignKey(t => t.AddressId);

            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
