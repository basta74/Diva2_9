using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Users;
using System;
using System.Collections.Generic;

namespace Diva2.Core.Main.Notifications;

public sealed class UserNotification : BaseEntity
{
    public int UserId { get; set; }
    public int? LessonId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime? ReactedAt { get; set; }
    public NotificationReaction Reaction { get; set; }
    public string? Error { get; set; }
    public int Attempts { get; set; }
    public User8? User { get; set; }
    public Lekce? Lesson { get; set; }
    public ICollection<NotificationDelivery> Deliveries { get; set; } = new List<NotificationDelivery>();
}
