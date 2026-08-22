using Diva2.Core.Main.Notifications;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Diva2.Services.Managers.Notifications;

public interface INotificationService
{
    Task<IReadOnlyList<UserNotification>> GetForUserAsync(int userId, CancellationToken cancellationToken);
    Task<bool> MarkDeliveredAsync(int notificationId, int userId, int? deviceId, CancellationToken cancellationToken);
    Task<bool> MarkReadAsync(int notificationId, int userId, CancellationToken cancellationToken);
    Task<bool> ReactAsync(int notificationId, int userId, NotificationReaction reaction, CancellationToken cancellationToken);
    Task<UserDevice> RegisterDeviceAsync(int userId, string pushToken, DevicePlatform platform, CancellationToken cancellationToken);
    Task<bool> DeactivateDeviceAsync(int deviceId, int userId, CancellationToken cancellationToken);
}
