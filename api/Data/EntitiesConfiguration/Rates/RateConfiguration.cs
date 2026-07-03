using Entities.Rates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Rates
{
    public class RateConfiguration : IEntityTypeConfiguration<Rate>
    {
        public void Configure(EntityTypeBuilder<Rate> builder)
        {
            builder.ToTable(nameof(Rate), nameof(Rate));

            builder.HasOne(t => t.CreatedByUser)
                .WithMany(t => t.Rates)
                .HasForeignKey(t => t.CreatedByUserId);
        }
    }
}
