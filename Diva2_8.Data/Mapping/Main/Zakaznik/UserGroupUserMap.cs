using Diva2.Core.Main.Zakaznik;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Data.Mapping.Main.Zakaznik
{
    class UserGroupUserMap : IEntityTypeConfiguration<User8GroupUser>
    {

        public void Configure(EntityTypeBuilder<User8GroupUser> builder)
        {
            builder.ToTable("users_category_binding");
            builder.HasKey(x => new { x.GroupId, x.UserId });

            builder.Property(d => d.GroupId).HasColumnName("category_id");
            builder.Property(d => d.UserId).HasColumnName("user_id");
            builder.Ignore(d => d.Id);
            /*
            builder.HasOne(ur => ur.Role).WithMany(x => x.Users).HasForeignKey(r => r.RoleId);
            builder.HasOne(ur => ur.User).WithMany(x => x.Roles).HasForeignKey(u => u.UserId);
            /**/
        }
    }
}