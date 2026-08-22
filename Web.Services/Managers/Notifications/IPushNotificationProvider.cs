using Diva2.Core.Main.Notifications;
using System.Threading;
using System.Threading.Tasks;

namespace Diva2.Services.Managers.Notifications;

public sealed record PushNotificationMessage(int DeliveryId, string Token, DevicePlatform Platform, string Title, string Text, int NotificationId);
public sealed record PushNotificationResult(bool Success, string? ProviderMessageId = null, string? Error = null);

public interface IPushNotificationProvider
{
    Task<PushNotificationResult> SendAsync(PushNotificationMessage message, CancellationToken cancellationToken);
}

public sealed class NullPushNotificationProvider : IPushNotificationProvider
{
    public Task<PushNotificationResult> SendAsync(PushNotificationMessage message, CancellationToken cancellationToken) =>
        Task.FromResult(new PushNotificationResult(false, Error: "Push provider is not configured."));
}
