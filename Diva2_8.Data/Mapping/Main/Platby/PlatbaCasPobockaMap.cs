using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Core.Main.Platby;

namespace Diva2.Data.Mapping.Main.Platby
{
    class PlatbaCasPobockaMap : IEntityTypeConfiguration<PlatbaCasPobocka>
    {

        public void Configure(EntityTypeBuilder<PlatbaCasPobocka> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(d => d.Id).HasColumnName("ID");


            builder.Property(d => d.PlatbaId).HasColumnName("platbaID");
            builder.Property(d => d.PobockaId).HasColumnName("pokladnaID");

            builder.ToTable("spin_platba_cas_pob");
        }
    }
}
