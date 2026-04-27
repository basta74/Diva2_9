using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Core.Main.Trans;

namespace Diva2.Data.Mapping.Main.Trans
{
    class UserZbytekKreditMap : IEntityTypeConfiguration<UserZbytekKredit>
    {

        public void Configure(EntityTypeBuilder<UserZbytekKredit> builder)
        {
            builder.ToTable("spin_user_zbytek_kredit");
            builder.HasKey(x => new { x.UserId, x.PokladnaId });

            builder.Property(d => d.UserId).HasColumnName("userID");
            builder.Property(d => d.PokladnaId).HasColumnName("pokladnaID");
            builder.Property(d => d.Kredit).HasColumnName("kredit");
            builder.Property(d => d.KreditUnixTime).HasColumnName("kreditUnixTime");
            builder.Property(d => d.Rezervace).HasColumnName("rezervace");
            builder.Property(d => d.RezervaceUnixTime).HasColumnName("rezervaceUnixTime");
            builder.Property(d => d.IsSet).HasColumnName("krIs");
            builder.Ignore(d => d.Id);
        }
    }
}