using Diva2.Core.Main.Users;
using System;
using System.Collections.Generic;

namespace Diva2.Core.Main.Notifications;

public sealed class UserDevice : BaseEntity
{
    public int UserId { get; set; }
    public string PushToken { get; set; } = string.Empty;
    public DevicePlatform Platform { get; set; }
    public DateTime RegisteredAt { get; set; }
    public DateTime LastSeenAt { get; set; }
    public bool Active { get; set; }
    public User8? User { get; set; }
    public ICollection<NotificationDelivery> Deliveries { get; set; } = new List<NotificationDelivery>();
}
