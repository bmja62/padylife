using Entities.Calendar;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.EntitiesConfiguration.CalendarConfigurations
{
    public class CalendarEventConfiguration : IEntityTypeConfiguration<CalendarEvent>
    {
        public void Configure(EntityTypeBuilder<CalendarEvent> builder)
        {
            builder.ToTable(nameof(CalendarEvent), nameof(CalendarEvent));
            builder.HasOne(t => t.User).WithMany(t => t.CalendarEvents).HasForeignKey(t => t.UserId);
            builder.HasOne(t => t.Calendar).WithMany(t => t.CalendarEvents).HasForeignKey(t => t.CalendarId);
            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
