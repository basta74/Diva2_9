using Diva2.Core.Main.Notifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Diva2.Services.Managers.Notifications;

public sealed class NotificationService : INotificationService
{
    private readonly ApplicationDbContext db;

    public NotificationService(ApplicationDbContext db) => this.db = db;

    public async Task<IReadOnlyList<UserNotification>> GetForUserAsync(int userId, CancellationToken cancellationToken) =>
        await db.UserNotifications.AsNoTracking()
            .Where(x => x.UserId == userId)
            .Include(x => x.Lesson)
            .OrderByDescending(x => x.CreatedAt)
            .Take(200)
            .ToListAsync(cancellationToken);

    public async Task<bool> MarkDeliveredAsync(int notificationId, int userId, int? deviceId, CancellationToken cancellationToken)
    {
        var notification = await db.UserNotifications.FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId, cancellationToken);
        if (notification is null) return false;
        var now = DateTime.UtcNow;
        notification.DeliveredAt ??= now;
        if (deviceId.HasValue)
        {
            var delivery = await db.NotificationDeliveries.FirstOrDefaultAsync(x => x.UserNotificationId == notificationId
                && x.UserDeviceId == deviceId && x.UserDevice!.UserId == userId, cancellationToken);
            if (delivery is null) return false;
            delivery.DeliveredAt ??= now;
        }
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<bool> MarkReadAsync(int notificationId, int userId, CancellationToken cancellationToken) =>
        UpdateAsync(notificationId, userId, x =>
        {
            x.DeliveredAt ??= DateTime.UtcNow;
            x.ReadAt ??= DateTime.UtcNow;
        }, cancellationToken);

    public Task<bool> ReactAsync(int notificationId, int userId, NotificationReaction reaction, CancellationToken cancellationToken) =>
        UpdateAsync(notificationId, userId, x =>
        {
            x.DeliveredAt ??= DateTime.UtcNow;
            x.ReadAt ??= DateTime.UtcNow;
            x.Reaction = reaction;
            x.ReactedAt = DateTime.UtcNow;
        }, cancellationToken);

    private async Task<bool> UpdateAsync(int notificationId, int userId, Action<UserNotification> update, CancellationToken cancellationToken)
    {
        var notification = await db.UserNotifications.FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId, cancellationToken);
        if (notification is null) return false;
        update(notification);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<UserDevice> RegisterDeviceAsync(int userId, string pushToken, DevicePlatform platform, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var device = await db.UserDevices.FirstOrDefaultAsync(x => x.PushToken == pushToken, cancellationToken);
        if (device is null)
        {
            device = new UserDevice { UserId = userId, PushToken = pushToken, Platform = platform, RegisteredAt = now };
            db.UserDevices.Add(device);
        }
        device.UserId = userId;
        device.Platform = platform;
        device.LastSeenAt = now;
        device.Active = true;
        await db.UserNotifications.Where(x => x.UserId == userId && x.SentAt == null)
            .ExecuteUpdateAsync(x => x.SetProperty(n => n.Attempts, 0).SetProperty(n => n.Error, (string?)null), cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return device;
    }

    public async Task<bool> DeactivateDeviceAsync(int deviceId, int userId, CancellationToken cancellationToken)
    {
        var device = await db.UserDevices.FirstOrDefaultAsync(x => x.Id == deviceId && x.UserId == userId, cancellationToken);
        if (device is null) return false;
        device.Active = false;
        device.LastSeenAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
