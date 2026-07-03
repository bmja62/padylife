using Entities.Challange;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Challanges
{
    public class ChallangeConfiguration : IEntityTypeConfiguration<Challange>
    {
        public void Configure(EntityTypeBuilder<Challange> builder)
        {
            builder.ToTable(nameof(Challange), nameof(Challange));

            builder.HasMany(t => t.Logs).WithOne(t => t.Challange).HasForeignKey(t => t.ChallengId);

            builder.HasQueryFilter(t => !t.IsDeleted);
            builder.HasOne(t => t.CreatedByUser).WithMany().HasForeignKey(t => t.CreatedByUserId);
        }
    }
}
