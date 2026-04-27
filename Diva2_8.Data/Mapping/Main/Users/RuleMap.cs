using Diva2.Core.Main.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Data.Mapping.Main.Users
{
    public class RuleMap : IEntityTypeConfiguration<Rule8>
    {
        public void Configure(EntityTypeBuilder<Rule8> builder)
        {
            builder.ToTable("prava");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasColumnName("PRA_ID");
            builder.Property(r => r.Name).HasColumnName("PRA_NAZEV");
            builder.Property(r => r.Description).HasColumnName("PRA_POPIS");
            builder.Property(r => r.PublicAble).HasColumnName("PRA_ABLE");

        }
    }
}
