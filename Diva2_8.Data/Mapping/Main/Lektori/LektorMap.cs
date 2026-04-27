using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Core.Main.Lektori;

namespace Diva2.Data.Mapping.Main.Lektori
{
    class LektorMap : IEntityTypeConfiguration<Lektor>
    {

        public void Configure(EntityTypeBuilder<Lektor> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(d => d.Id).HasColumnName("ID");
            builder.Property(d => d.Nick).HasColumnName("nick");
            builder.Property(d => d.Jmeno).HasColumnName("jmeno");
            builder.Property(d => d.Titul).HasColumnName("titul");
            builder.Property(d => d.Kredity).HasColumnName("kredity");
            builder.Property(d => d.Email).HasColumnName("email");
            builder.Property(d => d.Tel).HasColumnName("tel");
            builder.Property(d => d.Popis).HasColumnName("popis");
            builder.Property(d => d.Poradi).HasColumnName("poradi");
            builder.Property(d => d.Platnost).HasColumnName("platnost");
            builder.Property(d => d.Viditelnost).HasColumnName("viditelnost");
            builder.Property(d => d.UserId).HasColumnName("userID");
            builder.Property(d => d.Koeficient).HasColumnName("koeficient");

            /*
            builder.Ignore(d => d.Platnost);
            builder.Ignore(d => d.Viditelnost);
            /**/
            builder.ToTable("spinlektori");
        }
    }
}
