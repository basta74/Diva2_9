using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Core.Main.Pobocky;

namespace Diva2.Data.Mapping.Main.Pobocky
{
    public class CasZacatekMap : IEntityTypeConfiguration<CasZacatek>
    {

        public void Configure(EntityTypeBuilder<CasZacatek> builder)
        {
            builder.ToTable("spin_ini_zacatek");
            builder.HasKey(x => x.Id);
            builder.Property(d => d.PobockaId).HasColumnName("Pobocka");
            builder.Property(d => d.Value).HasColumnName("Zacatek").HasColumnType("timespan");


        }
    }
}
