using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Core.Main;
using Diva2.Core.Main.Lessons;

namespace Diva2.Data.Mapping.Main
{
    public class LekceMap : IEntityTypeConfiguration<Lekce>
    {

        public void Configure(EntityTypeBuilder<Lekce> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(d => d.Verze).HasColumnName("verze");
            builder.Property(d => d.MustrId).HasColumnName("spinTID");
            builder.Property(d => d.PobockaId).HasColumnName("pobocka");
            builder.Property(d => d.Zdroj).HasColumnName("zdroj");
            builder.Property(d => d.Rok).HasColumnName("rok");
            builder.Property(d => d.Tyden).HasColumnName("tyden");
            builder.Property(d => d.Den).HasColumnName("denVTydnu");
            builder.Property(d => d.Datum).HasColumnName("datum");
            builder.Property(d => d.HodinaPoradi).HasColumnName("hodina");
            builder.Property(d => d.MinutaKey).HasColumnName("minuta");
            builder.Property(d => d.Cas).HasColumnName("hodinaZacatek");
            builder.Property(d => d.Nazev).HasColumnName("nazev");
            builder.Property(d => d.Kredit).HasColumnName("kredit");
            builder.Property(d => d.Minuty).HasColumnName("minuty");
            builder.Property(d => d.PocetJednotek).HasColumnName("pocetJednotek");
            builder.Property(d => d.Lektor1).HasColumnName("lektor");
            builder.Property(d => d.Lektor2).HasColumnName("lektor2");
            builder.Property(d => d.TypHodiny).HasColumnName("typ");
            builder.Property(d => d.PocetMist).HasColumnName("pocetM");
            builder.Property(d => d.PocetZakazniku).HasColumnName("pocetZ");
            builder.Property(d => d.PocetNahradniku).HasColumnName("pocNahr");
            builder.Property(d => d.Zauctovano).HasColumnName("zauctovano");
            builder.Property(d => d.ZauctovalUserId).HasColumnName("zauctovalId");
            builder.Property(d => d.ZauctovalUserName).HasColumnName("zauctovalJmeno");
            builder.Property(d => d.ZauctovanoDate).HasColumnName("zauctovalDt");
            builder.Ignore(d => d.ForTable).Ignore(d => d.DatumHodina);
            builder.ToTable("spinhod");
        }
    }
}
