using Diva2.Core.Main.Comunications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Data.Mapping.Main.Comunications
{
    class SmsLogMap : IEntityTypeConfiguration<SmsLog>
    {

        public void Configure(EntityTypeBuilder<SmsLog> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(d => d.Id).HasColumnName("ID");
            builder.Property(d => d.PobockaId).HasColumnName("pobockaID");
            builder.Property(d => d.LekceId).HasColumnName("spinID");
            builder.Property(d => d.ZakaznikId).HasColumnName("zakaznikID");
            builder.Property(d => d.UserId).HasColumnName("userID");
            builder.Property(d => d.Cislo).HasColumnName("cislo");
            builder.Property(d => d.Email).HasColumnName("email");
            builder.Property(d => d.Text).HasColumnName("text");
            builder.Property(d => d.CreateUnix).HasColumnName("createUnix");
            builder.Property(d => d.SendUnix).HasColumnName("sendUnix");
            builder.Property(d => d.SendUserId).HasColumnName("sendUserID");
            builder.Property(d => d.Stav).HasColumnName("sendStav");
            builder.Property(d => d.Vyrizeno).HasColumnName("vyrizeno");

            /*
            builder.Ignore(d => d.Platnost);
            builder.Ignore(d => d.Viditelnost);
            /**/
            builder.ToTable("sms");
        }

    }
}
