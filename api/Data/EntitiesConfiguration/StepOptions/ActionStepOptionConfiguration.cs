using Entities.StepOprions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.StepOptions
{
    public class ActionStepOptionConfiguration : IEntityTypeConfiguration<ActionStepOption>
    {
        public void Configure(EntityTypeBuilder<ActionStepOption> builder)
        {
            builder.Property(x => x.ActionCommand)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.ActionParameters)
                .HasMaxLength(500);

            builder.Property(x => x.RequiresConfirmation);
        }
    }
}
