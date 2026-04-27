using Diva2.Core.Main.Trans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Data.Mapping.Main.Trans
{
    class UserLekceChangeMap : IEntityTypeConfiguration<UserLekceChange>
    {

        public void Configure(EntityTypeBuilder<UserLekceChange> builder)
        {
            builder.ToTable("spinuserchange");
            builder.HasKey(x => x.Id);

            builder.Property(d => d.LekceId).HasColumnName("spinID");
            builder.Property(d => d.UserId).HasColumnName("userID");
            builder.Property(d => d.ProvedlId).HasColumnName("provedlID");
            builder.Property(d => d.Status).HasColumnName("status");
            builder.Property(d => d.Ts).HasColumnName("timestamp");

            builder.Ignore(d => d.UserName);
        }
    }

    class UserLekceLogOutMap : IEntityTypeConfiguration<UserLekceLogOut>
    {

        public void Configure(EntityTypeBuilder<UserLekceLogOut> builder)
        {
            builder.ToTable("spinuserchange_logout");
            builder.HasKey(x => x.Id);

            builder.Property(d => d.LekceId).HasColumnName("spinID");
            builder.Property(d => d.UserId).HasColumnName("userID");
            builder.Property(d => d.ProvedlId).HasColumnName("provedlID");

            builder.Property(d => d.Ts).HasColumnName("timestamp");


        }
    }

    class UserLekceLogInMap : IEntityTypeConfiguration<UserLekceLogIn>
    {

        public void Configure(EntityTypeBuilder<UserLekceLogIn> builder)
        {
            builder.ToTable("spinuserchange_login");
            builder.HasKey(x => x.Id);

            builder.Property(d => d.LekceId).HasColumnName("spinID");
            builder.Property(d => d.UserId).HasColumnName("userID");
            builder.Property(d => d.ProvedlId).HasColumnName("provedlID");


            builder.Property(d => d.FromAdministrace).HasColumnName("fromAdmin");
            builder.Property(d => d.Ts).HasColumnName("timestamp");


        }
    }
}