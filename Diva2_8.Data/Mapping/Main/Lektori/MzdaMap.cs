using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Core.Main.Lektori;

namespace Diva2.Data.Mapping.Main.Lektori
{
    class MzdaMap : IEntityTypeConfiguration<Mzda>
    {

        public void Configure(EntityTypeBuilder<Mzda> builder)
        {
            builder.ToTable("spin_mzdy");

            builder.HasKey(x => x.PokladnaId);

            builder.Property(d => d.PokladnaId).HasColumnName("pokladnaID");
            builder.Property(d => d.Mesic).HasColumnName("mesic");
            builder.Property(d => d.Den).HasColumnName("den");
            builder.Property(d => d.Hodina).HasColumnName("hodina");
            builder.Property(d => d.Zakaznik).HasColumnName("zakaznik");
            builder.Property(d => d.Pro100).HasColumnName("pro100");
            builder.Property(d => d.Zak01).HasColumnName("zak01");
            builder.Property(d => d.Zak02).HasColumnName("zak02");
            builder.Property(d => d.Zak03).HasColumnName("zak03");
            builder.Property(d => d.Zak04).HasColumnName("zak04");
            builder.Property(d => d.Zak05).HasColumnName("zak05");
            builder.Property(d => d.Zak06).HasColumnName("zak06");
            builder.Property(d => d.Zak07).HasColumnName("zak07");
            builder.Property(d => d.Zak08).HasColumnName("zak08");
            builder.Property(d => d.Zak09).HasColumnName("zak09");
            builder.Property(d => d.Zak10).HasColumnName("zak10");
            builder.Property(d => d.Zak11).HasColumnName("zak11");
            builder.Property(d => d.Zak12).HasColumnName("zak12");
            builder.Property(d => d.Zak13).HasColumnName("zak13");
            builder.Property(d => d.Zak14).HasColumnName("zak14");
            builder.Property(d => d.Zak15).HasColumnName("zak15");
            builder.Property(d => d.Zak16).HasColumnName("zak16");
            builder.Property(d => d.Zak17).HasColumnName("zak17");
            builder.Property(d => d.Zak18).HasColumnName("zak18");
            builder.Property(d => d.Zak19).HasColumnName("zak19");
            builder.Property(d => d.Zak20).HasColumnName("zak20");

            builder.Ignore(d => d.Id);
        }
    }
}
