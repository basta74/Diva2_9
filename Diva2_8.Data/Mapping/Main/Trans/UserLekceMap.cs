using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Core.Main.Trans;

namespace Diva2.Data.Mapping.Main.Trans
{
    class UserLekceMap : IEntityTypeConfiguration<LekceUser>
    {

        public void Configure(EntityTypeBuilder<LekceUser> builder)
        {
            builder.ToTable("spinuser");
            builder.HasKey(x => x.Id );

            builder.Property(d => d.LekceId).HasColumnName("spinID");
            builder.Property(d => d.UserId).HasColumnName("userID");
            builder.Property(d => d.PobockaId).HasColumnName("pobockaID");
            builder.Property(d => d.PokladnaId).HasColumnName("pokladnaID");
            builder.Property(d => d.Poradi).HasColumnName("poradi");
            builder.Property(d => d.KontCislo).HasColumnName("kontCislo");
            builder.Property(d => d.Aktivni).HasColumnName("aktivni");
            builder.Property(d => d.BylTam).HasColumnName("bylTam");
            builder.Property(d => d.Nahradnik).HasColumnName("nahradnik");
            builder.Property(d => d.NahradnikJa).HasColumnName("nahradnikJa");
            builder.Property(d => d.Premiera).HasColumnName("premiera");
            builder.Property(d => d.ZbytekTyp).HasColumnName("zbytekTyp");
            builder.Property(d => d.Zbytek).HasColumnName("zbytek");
            builder.Property(d => d.ZbyvaDni).HasColumnName("zbyvaDni");
            builder.Property(d => d.DoMzdy).HasColumnName("doMzdy");
            builder.Property(d => d.Unix).HasColumnName("unix");
            builder.Property(d => d.Datum).HasColumnName("datum");

        }
    }
}