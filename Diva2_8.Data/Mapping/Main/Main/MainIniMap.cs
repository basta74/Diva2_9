using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Core.Main.Platby;
using Diva2.Core.Main.Main;

namespace Diva2.Data.Mapping.Main
{
    class MainIniCoverMap : IEntityTypeConfiguration<MainIniCover>
    {

        public void Configure(EntityTypeBuilder<MainIniCover> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(d => d.Id).HasColumnName("Id");
            builder.Property(d => d.Data).HasColumnName("Values");
            builder.Property(d => d.Styly).HasColumnName("Styles");
            builder.Property(d => d.GatePays).HasColumnName("GatePays");
            builder.Property(d => d.BankAccount).HasColumnName("BankAccount");


            builder.ToTable("spin_ini_main");

            builder.Ignore(d => d.MainIniObj);
            builder.Ignore(d => d.MainStyleObj);
            builder.Ignore(d => d.MainGatePaysObj);
            builder.Ignore(d => d.BankAccountObj);

        }
    }
}

