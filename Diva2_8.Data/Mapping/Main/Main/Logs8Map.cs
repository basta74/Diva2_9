using Diva2.Core.Main.Main;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Data.Mapping.Main.Main
{
    class Logs8Map : IEntityTypeConfiguration<Log8>
    {

        public void Configure(EntityTypeBuilder<Log8> builder)
        {
            builder.HasKey(x => x.Id);
          
            builder.ToTable("logs8");

        }
       
    }
}

