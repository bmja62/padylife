using Entities.Calendar;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.CalendarConfigurations
{
    public class CalendarModelConfiguration : IEntityTypeConfiguration<CalendarModel>
    {
        public void Configure(EntityTypeBuilder<CalendarModel> builder)
        {
            builder.ToTable(nameof(CalendarModel), nameof(CalendarModel));

            builder.HasQueryFilter(t => !t.IsDeleted);

        }
    }
}
