using Common.Shemas;
using Entities.Wallets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Wallets
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.ToTable(nameof(Wallet), Schema.Wallet);
            builder.Metadata.FindNavigation(nameof(Wallet.Transactions)).SetPropertyAccessMode(PropertyAccessMode.Field);


        }
    }
}
