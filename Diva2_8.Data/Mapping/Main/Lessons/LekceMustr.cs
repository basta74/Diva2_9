using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Core.Main.Lessons;

namespace Diva2.Data.Mapping.Main.Lessons
{
    class LekceMustrMap : IEntityTypeConfiguration<LekceMustr>
    {

        public void Configure(EntityTypeBuilder<LekceMustr> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(d => d.Id).HasColumnName("ID");

            builder.Property(d => d.Aktivni).HasColumnName("aktivni");
            builder.Property(d => d.PobockaId).HasColumnName("pobocka");
            builder.Property(d => d.Zdroj).HasColumnName("zdroj");
            builder.Property(d => d.Den).HasColumnName("denVTydnu");
            builder.Property(d => d.Hodina).HasColumnName("hodina");
            builder.Property(d => d.Cas).HasColumnName("hodinaZacatek");
            builder.Property(d => d.MinutaKey).HasColumnName("minuta");
            builder.Property(d => d.Nazev).HasColumnName("nazev");
            builder.Property(d => d.Kredit).HasColumnName("kredit");
            builder.Property(d => d.Minuty).HasColumnName("minuty");
            builder.Property(d => d.Lektor).HasColumnName("lektor");
            builder.Property(d => d.Lektor2).HasColumnName("lektor2");
            builder.Property(d => d.Typ).HasColumnName("typ");
            builder.Property(d => d.PocetMist).HasColumnName("pocetM");
            builder.Property(d => d.PocetNahradniku).HasColumnName("pocetNahr");


            builder.ToTable("spinhod_sablona");
        }
    }

    class LekceMustrTypMap : IEntityTypeConfiguration<LekceMustrTyp>
    {

        public void Configure(EntityTypeBuilder<LekceMustrTyp> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(d => d.Id).HasColumnName("ID");

            builder.Property(d => d.Aktivni).HasColumnName("aktivni");
            builder.Property(d => d.Nazev).HasColumnName("nazev");
            builder.Property(d => d.Kredit).HasColumnName("kredit");
            builder.Property(d => d.Minuty).HasColumnName("minuty");
            builder.Property(d => d.Lektor).HasColumnName("lektor");
            builder.Property(d => d.Lektor2).HasColumnName("lektor2");
            builder.Property(d => d.Typ).HasColumnName("typ");
            builder.Property(d => d.PocetMist).HasColumnName("pocetM");
            builder.Property(d => d.PocetNahradniku).HasColumnName("pocetNahr");
            builder.ToTable("spinhod_sablona_typ");
        }
    }
}
