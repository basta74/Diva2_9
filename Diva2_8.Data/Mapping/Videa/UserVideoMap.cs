using Diva2.Core.Main.Videa;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Data.Mapping.Videa
{
    public class UserVideoMap : IEntityTypeConfiguration<UserVideo>
    {
        public void Configure(EntityTypeBuilder<UserVideo> builder)
        {
            builder.ToTable("users_videa");
            builder.HasKey(r => r.Id);
        }
    }
}
