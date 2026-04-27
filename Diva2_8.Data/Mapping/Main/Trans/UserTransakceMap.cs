using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Core.Main.Trans;

namespace Diva2.Data.Mapping.Main.Trans
{
    class UserTransakceMap : IEntityTypeConfiguration<UserTransakce>
    {

        public void Configure(EntityTypeBuilder<UserTransakce> builder)
        {
            builder.ToTable("spin_user_transakce");
            builder.HasKey(x => x.Id);

            builder.Property(d => d.UserId).HasColumnName("userID");
            builder.Property(d => d.PokladnaId).HasColumnName("pokladnaID");
            builder.Property(d => d.ProvedlId).HasColumnName("provedlID");
            builder.Property(d => d.Status).HasColumnName("status");
            builder.Property(d => d.Typ).HasColumnName("typ");
            builder.Property(d => d.Kredit).HasColumnName("kredit");
            builder.Property(d => d.Castka).HasColumnName("castka");
            builder.Property(d => d.DoPokladny).HasColumnName("doPokladny");
            builder.Property(d => d.LekceId).HasColumnName("spinID");
            builder.Property(d => d.VideoId).HasColumnName("videoID");
            builder.Property(d => d.Doklad).HasColumnName("doklad");
            builder.Property(d => d.Zbytek).HasColumnName("zbytek");
            builder.Property(d => d.ZbyvaDni).HasColumnName("zbyvaDni");
            builder.Property(d => d.Increment).HasColumnName("increment");
            builder.Property(d => d.Datum).HasColumnName("datum");
            builder.Property(d => d.Timestamp).HasColumnName("timestamp");
            builder.Property(d => d.UnixTime).HasColumnName("unixTime");
            builder.Property(d => d.PlatnostOd).HasColumnName("platnostOd");
            builder.Property(d => d.PlatnostDo).HasColumnName("platnostDo");
            builder.Property(d => d.PlatnostDoUnix).HasColumnName("platnostOdUnix");
            builder.Property(d => d.PlatnostDoUnix).HasColumnName("platnostDoUnix");

            builder.Ignore(d => d.PlatnostDo).Ignore(d => d.PlatnostOd);
            builder.Ignore(d => d.PlatbaId).Ignore(d => d.PlatbaPausal).Ignore(d => d.PlatbaTime);
            builder.Ignore(d => d.IsOk);
        }
    }
}