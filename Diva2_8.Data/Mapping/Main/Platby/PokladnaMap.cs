using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Core.Main.Platby;

namespace Diva2.Data.Mapping.Main.Platby
{
    class PokladnaMap : IEntityTypeConfiguration<Pokladna>
    {

        public void Configure(EntityTypeBuilder<Pokladna> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(d => d.Id).HasColumnName("ID");
            builder.Property(d => d.Nazev).HasColumnName("nazev");
            builder.ToTable("spin_pokladna");
        }
    }
}

