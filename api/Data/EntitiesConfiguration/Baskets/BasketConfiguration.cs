using Entities.Baskets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Baskets
{
    public class BasketConfiguration : IEntityTypeConfiguration<Basket>
    {
        public void Configure(EntityTypeBuilder<Basket> builder)
        {
            builder.ToTable(nameof(Basket), nameof(Basket));

            builder.HasMany(t => t.Items).WithOne(t => t.Basket).HasForeignKey(t => t.BasketId);
            builder.HasMany(t => t.HistoryLogs).WithOne(t => t.Basket).HasForeignKey(t => t.BasketId);
            builder.HasOne(t => t.User).WithMany(t => t.Baskets).HasForeignKey(t => t.UserId);
            builder.HasOne(t => t.Address).WithMany(t => t.Baskets).HasForeignKey(t => t.AddressId);

            builder.HasQueryFilter(t => !t.IsDeleted);
            builder.HasQueryFilter(t => t.Status == BasketStatus.Active);


        }
    }

    public class BasketItemConfiguration : IEntityTypeConfiguration<BasketItem>
    {
        public void Configure(EntityTypeBuilder<BasketItem> builder)
        {
            builder.ToTable(nameof(BasketItem), nameof(Basket));

            builder.HasOne(t => t.Basket).WithMany(t => t.Items).HasForeignKey(t => t.BasketId);
        }
    }

    public class BasketHistoryConfiguration : IEntityTypeConfiguration<BasketHistory>
    {
        public void Configure(EntityTypeBuilder<BasketHistory> builder)
        {
            builder.ToTable(nameof(BasketHistory), nameof(BasketHistory));

            builder.HasOne(t => t.Basket).WithMany(t => t.HistoryLogs).HasForeignKey(t => t.BasketId);
        }
    }
}
