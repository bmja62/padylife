using Entities.Challange;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Challanges
{
    public class ChallangeLogConfiguration : IEntityTypeConfiguration<ChallangeLog>
    {
        public void Configure(EntityTypeBuilder<ChallangeLog> builder)
        {
            builder.ToTable(nameof(ChallangeLog), nameof(Challange));

            builder.HasKey(x => new { x.ChallengId, x.UserId });

            builder.HasOne(t => t.User).WithMany(t => t.ChallangeLogs).HasForeignKey(t => t.UserId);
            builder.HasOne(t => t.Challange).WithMany(t => t.Logs).HasForeignKey(t => t.ChallengId);
        }
    }
}
