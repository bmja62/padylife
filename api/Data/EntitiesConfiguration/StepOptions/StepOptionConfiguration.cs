using Entities.Excersies;
using Entities.StepOprions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.StepOptions
{
    public class StepOptionConfiguration : IEntityTypeConfiguration<StepOption>
    {
        public void Configure(EntityTypeBuilder<StepOption> builder)
        {
            builder.ToTable(nameof(StepOption), nameof(Step));

            builder
                .HasDiscriminator<StepOptionType>("Type")
                .HasValue<MultipleChoiceStepOption>(StepOptionType.MultipleChoice)
                .HasValue<VideoStepOption>(StepOptionType.Video)
                .HasValue<TaskStepOption>(StepOptionType.Task)
                .HasValue<ActionStepOption>(StepOptionType.Action)
                .HasValue<TextStepOption>(StepOptionType.Text)
                .HasValue<ImageStepOption>(StepOptionType.Image);

            builder.HasOne(t => t.Step).WithMany(t => t.StepOptions).HasForeignKey(t => t.StepId);


            builder.HasOne(t => t.CreatedByUser).WithMany().HasForeignKey(t => t.CreatedByUserId);

            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
