using Common.Shemas;
using Entities.Visits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Visits
{
    public class DailyVisitStatsConfiguration : IEntityTypeConfiguration<DailyVisitStats>
    {
        public void Configure(EntityTypeBuilder<DailyVisitStats> builder)
        {
            builder.ToTable(nameof(DailyVisitStats), Schema.Visit);
        }
    }
}
