using Diva2.Core.Main.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Data.Mapping.Main.Lessons
{
    public class LekceVideoMap : IEntityTypeConfiguration<LekceVideo>
    {

        public void Configure(EntityTypeBuilder<LekceVideo> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(d => d.Id).HasColumnName("spin_id");

            builder.ToTable("spinhod_video");
        }
    }

    public class LekceTextMap : IEntityTypeConfiguration<LekceText>
    {

        public void Configure(EntityTypeBuilder<LekceText> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(d => d.Id).HasColumnName("spin_id");

            builder.ToTable("spinhod_text");
        }
    }
}