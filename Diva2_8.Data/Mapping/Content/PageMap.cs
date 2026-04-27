using Diva2.Core.Main.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Data.Mapping.Content
{
    public class PageMap : IEntityTypeConfiguration<Page>
    {

        public void Configure(EntityTypeBuilder<Page> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("pages");
        }
    }
}
