using Entities.StepOprions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.StepOptions
{
    public class VideoStepOptionConfiguration : IEntityTypeConfiguration<VideoStepOption>
    {
        public void Configure(EntityTypeBuilder<VideoStepOption> builder)
        {
            builder.Property(x => x.VideoUrl)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.ThumbnailUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.Duration).IsRequired();
            builder.Property(x => x.AllowDownload);
        }
    }
}
