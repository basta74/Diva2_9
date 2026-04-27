using Diva2.Core.Main.Pobocky;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Data.Mapping.Main.Pobocky
{
    class PobockaIniMap : IEntityTypeConfiguration<PobockaIni>
    {

        public void Configure(EntityTypeBuilder<PobockaIni> builder)
        {
            builder.ToTable("spin_ini");
            builder.HasKey(x => new { x.PobockaId, x.Name });

            builder.Property(d => d.PobockaId).HasColumnName("PobockaID");
            builder.Property(d => d.Name).HasColumnName("Nazev");
            builder.Property(d => d.Value).HasColumnName("Hodnota");

            builder.Ignore(d => d.TypeString).Ignore(d => d.Desc).Ignore(d => d.Default);
            builder.Ignore(d => d.Id).Ignore(d => d.Type);

        }
    }
}
