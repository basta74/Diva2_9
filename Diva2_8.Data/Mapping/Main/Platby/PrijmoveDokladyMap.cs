using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Core.Main.Platby;

namespace Diva2.Data.Mapping.Main.Platby
{
    class PrijmoveDokladyMap : IEntityTypeConfiguration<PrijmoveDoklady>
    {

        public void Configure(EntityTypeBuilder<PrijmoveDoklady> builder)
        {
            builder.HasKey(d => d.Rok);
            builder.Property(d => d.Rok).HasColumnName("rok");
            builder.Property(d => d.Hodnota).HasColumnName("poradi");
            builder.ToTable("prijmove_doklady");
            builder.Ignore(d => d.Id);
        }
    }
}

