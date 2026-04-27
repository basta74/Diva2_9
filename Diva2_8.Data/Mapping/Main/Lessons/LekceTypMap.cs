using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Core.Main.Lessons;

namespace Diva2.Data.Mapping.Main.Lessons
{
    class LekceTypMap : IEntityTypeConfiguration<LekceTyp>
    {

        public void Configure(EntityTypeBuilder<LekceTyp> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(d => d.Id).HasColumnName("ID");
            builder.Property(d => d.PobockaId).HasColumnName("pobockaID");
            builder.Property(d => d.Nazev).HasColumnName("nazev");
            builder.Property(d => d.Zkratka).HasColumnName("zkratka");
            builder.Property(d => d.JednotkaPocet).HasColumnName("jednPocet");
            builder.Property(d => d.JednotkaPocetDo).HasColumnName("jednPocetDo");
            builder.Property(d => d.JednotkaMinut).HasColumnName("jednMinut");
            builder.Property(d => d.Kredit).HasColumnName("kredit");
            builder.Property(d => d.Mista).HasColumnName("mista");
            builder.Property(d => d.Popis).HasColumnName("popis");
            builder.Ignore(d => d.NazevAdmin);

            builder.ToTable("spinhod_typ");
        }
    }
}
