using Diva2.Core.Main.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Data.Mapping.Main.Zakaznik
{
    public class SkupinaZakaznikaMap : IEntityTypeConfiguration<User8Group>
    {

        public void Configure(EntityTypeBuilder<User8Group> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(d => d.Id).HasColumnName("id");
            builder.Property(d => d.PobockaId).HasColumnName("pobocka_id");
            builder.Property(d => d.Nazev).HasColumnName("nazev");
            builder.Property(d => d.Popis).HasColumnName("popis");
            

            builder.ToTable("users_category");
        }
    }
}