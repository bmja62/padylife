using Common.Shemas;
using Entities.Visits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.EntitiesConfiguration.Visits
{
    public class PageVisitConfiguration : IEntityTypeConfiguration<PageVisit>
    {
        public void Configure(EntityTypeBuilder<PageVisit> builder)
        {
            builder.ToTable(nameof(PageVisit), Schema.Visit);

        }
    }
}
