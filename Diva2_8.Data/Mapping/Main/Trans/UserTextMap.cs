using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Core.Main.Trans;

namespace Diva2.Data.Mapping.Main.Trans
{
    class UserTextMap : IEntityTypeConfiguration<UserText>
    {

        public void Configure(EntityTypeBuilder<UserText> builder)
        {
            builder.ToTable("spinuser_text");
            builder.HasKey(x => x.Id);

            builder.Property(d => d.LekceId).HasColumnName("spinID");
            builder.Property(d => d.UserId).HasColumnName("userID");
            builder.Property(d => d.KontrolniCislo).HasColumnName("kontrolniCislo");
            builder.Property(d => d.Text).HasColumnName("text");

        }
    }
}