using Diva2.Core.Main.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diva2.Data.Mapping.Main.Notifications;

internal sealed class UserDeviceMap : IEntityTypeConfiguration<UserDevice>
{
    public void Configure(EntityTypeBuilder<UserDevice> builder)
    {
        builder.ToTable("spin_user_device");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PushToken).HasMaxLength(512).IsRequired();
        builder.Property(x => x.Platform).HasConversion<string>().HasMaxLength(16);
        builder.HasIndex(x => x.PushToken).IsUnique();
        builder.HasIndex(x => new { x.UserId, x.Active });
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
