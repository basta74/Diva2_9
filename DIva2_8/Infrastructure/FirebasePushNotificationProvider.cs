using Diva2.Services.Managers.Notifications;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace Diva2Web.Infrastructure;

public sealed class FirebasePushNotificationProvider : IPushNotificationProvider, IDisposable
{
    private readonly FirebaseApp? firebaseApp;
    private readonly string? configurationError;

    public FirebasePushNotificationProvider(IConfiguration configuration, ILogger<FirebasePushNotificationProvider> logger)
    {
        var credentialsPath = configuration["Firebase:CredentialsPath"];
        if (string.IsNullOrWhiteSpace(credentialsPath))
        {
            credentialsPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
        }

        if (string.IsNullOrWhiteSpace(credentialsPath) || !File.Exists(credentialsPath))
        {
            configurationError = "Firebase service account file is not configured or does not exist.";
            logger.LogError("{Error} Configured path: {Path}", configurationError, credentialsPath);
            return;
        }

        firebaseApp = FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile(credentialsPath)
        }, $"diva2-{Guid.NewGuid():N}");
    }

    public async Task<PushNotificationResult> SendAsync(
        PushNotificationMessage message,
        CancellationToken cancellationToken)
    {
        if (firebaseApp is null)
        {
            return new PushNotificationResult(false, Error: configurationError);
        }

        if (message.Platform != Diva2.Core.Main.Notifications.DevicePlatform.Android)
        {
            return new PushNotificationResult(false, Error: $"Push platform {message.Platform} is not configured.");
        }

        try
        {
            var firebaseMessage = new Message
            {
                Token = message.Token,
                Notification = new Notification
                {
                    Title = message.Title,
                    Body = message.Text
                },
                Data = new Dictionary<string, string>
                {
                    ["notificationId"] = message.NotificationId.ToString(),
                    ["deliveryId"] = message.DeliveryId.ToString(),
                    ["title"] = message.Title,
                    ["body"] = message.Text
                },
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        ChannelId = "diva2_notifications"
                    }
                }
            };

            var providerMessageId = await FirebaseMessaging.GetMessaging(firebaseApp)
                .SendAsync(firebaseMessage, false, cancellationToken);
            return new PushNotificationResult(true, providerMessageId);
        }
        catch (Exception exception)
        {
            return new PushNotificationResult(false, Error: exception.Message);
        }
    }

    public void Dispose()
    {
        firebaseApp?.Delete();
    }
}
