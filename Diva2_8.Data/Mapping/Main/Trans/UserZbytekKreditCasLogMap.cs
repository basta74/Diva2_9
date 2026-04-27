using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Core.Main.Trans;

namespace Diva2.Data.Mapping.Main.Trans
{
    class UserZbytekKreditCasLogMap : IEntityTypeConfiguration<UserZbytekKreditCasLog>
    {

        public void Configure(EntityTypeBuilder<UserZbytekKreditCasLog> builder)
        {
            builder.ToTable("spin_user_zbytek_kredit_casovy_log");
            builder.HasKey(x => x.Id);
            builder.Property(d => d.Id).HasColumnName("ID");

        }
    }
}