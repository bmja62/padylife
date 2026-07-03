using Entities.StepOprions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.StepOptions
{
    public class TaskStepOptionConfiguration : IEntityTypeConfiguration<TaskStepOption>
    {
        public void Configure(EntityTypeBuilder<TaskStepOption> builder)
        {
            builder.Property(x => x.AssigneeRole)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.TaskInstructions)
                .HasMaxLength(2000);

            builder.Property(x => x.EstimatedMinutes)
                .HasDefaultValue(30)
                .HasConversion<int>();

            builder.Property(x => x.DeadlineDays);
        }
    }
}
