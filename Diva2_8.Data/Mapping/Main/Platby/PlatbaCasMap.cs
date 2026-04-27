using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Core.Main.Platby;

namespace Diva2.Data.Mapping.Main.Platby
{
    class PlatbaCasMap : IEntityTypeConfiguration<PlatbaCas>
    {

        public void Configure(EntityTypeBuilder<PlatbaCas> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(d => d.Id).HasColumnName("ID");

            builder.Property(d => d.Verze).HasColumnName("verze");
            builder.Property(d => d.Kategorie).HasColumnName("kategorie");
            builder.Property(d => d.Castka).HasColumnName("platba");
            builder.Property(d => d.Popis).HasColumnName("popis");
            builder.Property(d => d.Platnost).HasColumnName("platnost");
            builder.Property(d => d.Visible).HasColumnName("visible");
            builder.Property(d => d.AutorId).HasColumnName("autorID");
            builder.Property(d => d.Unix).HasColumnName("unix");


            builder.ToTable("spin_platba_cas");
        }
    }
}
