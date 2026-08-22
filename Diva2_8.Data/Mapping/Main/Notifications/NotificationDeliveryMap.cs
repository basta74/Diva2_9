using Diva2.Core.Main.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diva2.Data.Mapping.Main.Notifications;

internal sealed class NotificationDeliveryMap : IEntityTypeConfiguration<NotificationDelivery>
{
    public void Configure(EntityTypeBuilder<NotificationDelivery> builder)
    {
        builder.ToTable("spin_user_notification_delivery");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProviderMessageId).HasMaxLength(256);
        builder.Property(x => x.Error).HasMaxLength(2000);
        builder.HasIndex(x => new { x.UserNotificationId, x.UserDeviceId }).IsUnique();
        builder.HasIndex(x => new { x.SentAt, x.Attempts });
        builder.HasOne(x => x.UserNotification).WithMany(x => x.Deliveries).HasForeignKey(x => x.UserNotificationId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.UserDevice).WithMany(x => x.Deliveries).HasForeignKey(x => x.UserDeviceId).OnDelete(DeleteBehavior.Cascade);
    }
}
