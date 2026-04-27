using Diva2.Core.Main.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Data.Mapping.Main.Users
{
    public class RoleRuleMap : IEntityTypeConfiguration<RoleRule8>
    {
        public void Configure(EntityTypeBuilder<RoleRule8> builder)
        {
            builder.ToTable("prava_for_role");
            builder.HasKey(r => new { r.RoleId, r.PravoId });
            builder.Property(r => r.RoleId).HasColumnName("ROLE_ID");
            builder.Property(r => r.PravoId).HasColumnName("PRAVA_ID");
            builder.Ignore(d => d.Id);

        }
    }
}
