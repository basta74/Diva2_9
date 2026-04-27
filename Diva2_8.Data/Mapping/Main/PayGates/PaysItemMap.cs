using Diva2.Core.Main.PayGates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Data.Mapping.Main.PayGates
{
    public class PaysItemMap : IEntityTypeConfiguration<PaysItem>
    {

        public void Configure(EntityTypeBuilder<PaysItem> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(d => d.Id).HasColumnName("ID");


            builder.ToTable("pays_items");
        }
    }
}
