using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Core.Main.Users;
using Diva2.Core.Main.Trans;

namespace Diva2.Data.Mapping.Main.Users
{
    public class UserMap : IEntityTypeConfiguration<User8>
    {

        public void Configure(EntityTypeBuilder<User8> builder)
        {
            builder.ToTable("users_8");
            builder.HasKey(x => x.Id);
            builder.Property(d => d.Id).HasColumnName("USER_ID");
            builder.Property(d => d.Nazev).HasColumnName("USER_NAZEV");
            builder.Property(d => d.Titul).HasColumnName("USER_TITUL");
            builder.Property(d => d.Jmeno).HasColumnName("USER_JMENO");
            builder.Property(d => d.Prijmeni).HasColumnName("USER_PRIJMENI");
            builder.Property(d => d.UserName).HasColumnName("USER_NICK");
            builder.Property(d => d.PasswordHash).HasColumnName("USER_HESLO");

            builder.Property(d => d.PhoneNumber).HasColumnName("USER_TEL");
            builder.Property(d => d.Email).HasColumnName("USER_EMAIL");
            builder.Property(d => d.Ulice).HasColumnName("USER_ULICE");
            builder.Property(d => d.Psc).HasColumnName("USER_PSC");
            builder.Property(d => d.Posta).HasColumnName("USER_POSTA");

            builder.Property(d => d.Platnost).HasColumnName("USER_PLATNOST");
            builder.Property(d => d.LastLogin).HasColumnName("USER_LAST_LOGIN");
            builder.Property(d => d.Deleted).HasColumnName("USER_DELETED");
            builder.Property(d => d.InternalNumber).HasColumnName("USER_NUMBER");
            builder.Property(d => d.Poznamka).HasColumnName("USER_POPIS");
            builder.Property(d => d.Poznamka2).HasColumnName("Note2");



            builder.Property(d => d.GdprBase).HasColumnName("gdprBase").HasColumnType("bit");
            builder.Property(d => d.GdprBaseDT).HasColumnName("gdprDate");
            builder.Property(d => d.GdprNews).HasColumnName("gdprNews").HasColumnType("bit");
            builder.Property(d => d.GdprNewsDT).HasColumnName("gdprNewsDate");

            builder.HasMany(d => d.Kredity).WithOne().HasForeignKey(k=>k.UserId);
            builder.HasMany(d => d.KredityCas).WithOne().HasForeignKey(k => k.UserId);

            //builder.Property(d => d).HasColumnName("");
            /**/

        }
    }
}
