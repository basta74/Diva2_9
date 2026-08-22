using System;

namespace Diva2.Core.Main.Notifications;

public sealed class NotificationDelivery : BaseEntity
{
    public int UserNotificationId { get; set; }
    public int UserDeviceId { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public int Attempts { get; set; }
    public string? ProviderMessageId { get; set; }
    public string? Error { get; set; }
    public UserNotification? UserNotification { get; set; }
    public UserDevice? UserDevice { get; set; }
}
