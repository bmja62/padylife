using Entities.StepOprions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.StepOptions
{
    public class ImageStepOptionConfiguration : IEntityTypeConfiguration<ImageStepOption>
    {
        public void Configure(EntityTypeBuilder<ImageStepOption> builder)
        {
            builder.Property(x => x.ImageUrl)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.AltText).HasMaxLength(200);
            builder.Property(x => x.Caption).HasMaxLength(500);
        }
    }
}
