using Entities.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Tickets
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("Tickets", "Ticket");
            builder.HasKey(x => x.Id);

            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }

    public class TicketDetailConfiguration : IEntityTypeConfiguration<TicketDetail>
    {
        public void Configure(EntityTypeBuilder<TicketDetail> builder)
        {
            builder.ToTable(nameof(TicketDetail), "Ticket");
            builder.HasKey(x => x.Id);

            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }

}
