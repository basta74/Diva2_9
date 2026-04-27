using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Core.Main;

namespace Diva2.Data.Mapping.Main
{
    public class PobockaMap : IEntityTypeConfiguration<Pobocka>
    {

        public void Configure(EntityTypeBuilder<Pobocka> builder)
        {
            builder.ToTable("spin_pobocka");
            builder.HasKey(x => x.Id);
            builder.Property(d => d.Name).HasColumnName("Nazev");
            builder.Property(d => d.Description).HasColumnName("Popis");
            builder.Property(d => d.PocetMist).HasColumnName("pocetM");
            builder.Property(d => d.Kredity).HasColumnName("kredit");
            builder.Property(d => d.Minuty).HasColumnName("minuty");
            builder.Property(d => d.PokladnaId).HasColumnName("pokladnaId");
            builder.Property(d => d.Visible).HasColumnName("visible").HasColumnType("bit");
            builder.Property(d => d.Order).HasColumnName("poradi");
            builder.Property(d => d.Color1).HasColumnName("color1");
            builder.Property(d => d.Color2).HasColumnName("color2");
            builder.Property(d => d.MotivHlavni).HasColumnName("motivHlavni");
            builder.Property(d => d.MotivKontra).HasColumnName("motivKontra");
            builder.Property(d => d.Typ).HasColumnName("pobockaType");

            builder.Ignore(d => d.CalendarSettingObj);
            //builder.Property(d => d).HasColumnName("");

        }
    }
}
