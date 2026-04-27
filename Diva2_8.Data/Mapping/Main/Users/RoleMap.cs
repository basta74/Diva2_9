using Diva2.Core.Main.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Data.Mapping.Main.Users
{
    public class RoleMap : IEntityTypeConfiguration<Role8>
    {
        public void Configure(EntityTypeBuilder<Role8> builder)
        {
            builder.ToTable("role");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasColumnName("ROLE_ID");
            builder.Property(r => r.Name).HasColumnName("ROLE_NAZEV");
            builder.Property(r => r.Description).HasColumnName("ROLE_POPIS");
            builder.Property(r => r.PublicAble).HasColumnName("ROLE_ABLE");

        }
    }
}
