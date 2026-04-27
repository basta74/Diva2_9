using Diva2.Core.Main.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Data.Mapping.Main.Users
{
    class UserRoleMap : IEntityTypeConfiguration<UserRoles8>
    {

        public void Configure(EntityTypeBuilder<UserRoles8> builder)
        {
            builder.ToTable("role_for_user");
            builder.HasKey(x => new { x.RoleId, x.UserId });

            builder.Property(d => d.RoleId).HasColumnName("RU_ROLE_ID");
            builder.Property(d => d.UserId).HasColumnName("RU_USER_ID");
            //builder.Ignore(d => d.Id);
            /*
            builder.HasOne(ur => ur.Role).WithMany(x => x.Users).HasForeignKey(r => r.RoleId);
            builder.HasOne(ur => ur.User).WithMany(x => x.Roles).HasForeignKey(u => u.UserId);
            /**/
        }
    }
}