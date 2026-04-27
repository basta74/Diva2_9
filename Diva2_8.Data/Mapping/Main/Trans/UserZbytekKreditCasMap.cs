using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Core.Main.Trans;

namespace Diva2.Data.Mapping.Main.Trans
{
    class UserZbytekKreditCasMap : IEntityTypeConfiguration<UserZbytekKreditCas>
    {

        public void Configure(EntityTypeBuilder<UserZbytekKreditCas> builder)
        {
            builder.ToTable("spin_user_zbytek_kredit_casovy");
            builder.HasKey(x => x.Id);

            builder.Property(d => d.Id).HasColumnName("ID");

            builder.Property(d => d.PlatbaId).HasColumnName("platbaID");
            builder.Property(d => d.UserId).HasColumnName("userID");
            builder.Property(d => d.PokladnaId).HasColumnName("pokladnaID");
            builder.Property(d => d.Kredit).HasColumnName("kredit");
            builder.Property(d => d.PocetLidi).HasColumnName("pocetLidi");
            builder.Property(d => d.Aktivni).HasColumnName("aktivni");
            builder.Property(d => d.PocetMesicu).HasColumnName("pocetMesicu");
            builder.Property(d => d.PlatnostOdUnix).HasColumnName("platnostOdUnix");
            builder.Property(d => d.PlatnostDoUnix).HasColumnName("platnostDoUnix");
            builder.Property(d => d.PlatnostUnixBreak).HasColumnName("platnostUnixBreak");
            builder.Property(d => d.KreditUnixTime).HasColumnName("kreditUnixTime");

            builder.Ignore(d => d.PlatnostDo).Ignore(d => d.PlatnostOd);
            builder.Ignore(d => d.ZbyvaDni);

        }
    }
}