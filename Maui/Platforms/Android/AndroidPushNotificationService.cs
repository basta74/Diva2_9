using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Diva2Maui.Services;
using Firebase.Messaging;

namespace Diva2Maui.Platforms.Android;

public static class AndroidPushNotificationService
{
    private const string TokenPreferenceKey = "firebase_push_token";
    private const string OpenNotificationsPreferenceKey = "open_notifications_after_push";
    private static Diva2ApiClient? apiClient;

    public static async Task RegisterAsync(Diva2ApiClient client, CancellationToken cancellationToken = default)
    {
        apiClient = client;
        var token = Preferences.Default.Get(TokenPreferenceKey, string.Empty);
        if (string.IsNullOrWhiteSpace(token))
        {
            token = await GetFirebaseTokenAsync(cancellationToken);
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        Preferences.Default.Set(TokenPreferenceKey, token);
        await client.RegisterDeviceAsync(token, cancellationToken);
    }

    public static async Task TokenChangedAsync(string token)
    {
        Preferences.Default.Set(TokenPreferenceKey, token);
        if (apiClient is null)
        {
            return;
        }

        try
        {
            await apiClient.RegisterDeviceAsync(token);
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine($"Registrace FCM tokenu se nezdařila: {exception.Message}");
        }
    }

    public static void ShowNotification(RemoteMessage message)
    {
        const string channelId = "diva2_notifications";
        var context = global::Android.App.Application.Context;
        var manager = (NotificationManager?)context.GetSystemService(Context.NotificationService);
        if (manager is null)
        {
            return;
        }

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            manager.CreateNotificationChannel(new NotificationChannel(
                channelId,
                "Oznámení Diva2",
                NotificationImportance.Default));
        }

        var intent = context.PackageManager?.GetLaunchIntentForPackage(context.PackageName!);
        PendingIntent? pendingIntent = null;
        if (intent is not null)
        {
            intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            intent.PutExtra("notificationId", GetData(message, "notificationId") ?? string.Empty);
            pendingIntent = PendingIntent.GetActivity(
                context,
                0,
                intent,
                PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
        }

        var title = message.GetNotification()?.Title ?? GetData(message, "title") ?? "Diva2";
        var body = message.GetNotification()?.Body ?? GetData(message, "body") ?? string.Empty;
        var notification = new NotificationCompat.Builder(context, channelId)
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetContentTitle(title)
            .SetContentText(body)
            .SetStyle(new NotificationCompat.BigTextStyle().BigText(body))
            .SetAutoCancel(true)
            .SetContentIntent(pendingIntent)
            .Build();

        manager.Notify(message.MessageId?.GetHashCode() ?? System.Environment.TickCount, notification);
    }

    public static void MarkNotificationsToOpen()
    {
        Preferences.Default.Set(OpenNotificationsPreferenceKey, true);
    }

    public static bool ConsumeNotificationsToOpen()
    {
        var openNotifications = Preferences.Default.Get(OpenNotificationsPreferenceKey, false);
        if (openNotifications)
        {
            Preferences.Default.Remove(OpenNotificationsPreferenceKey);
        }

        return openNotifications;
    }

    private static async Task<string> GetFirebaseTokenAsync(CancellationToken cancellationToken)
    {
        var completion = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var registration = cancellationToken.Register(() => completion.TrySetCanceled(cancellationToken));
        FirebaseMessaging.Instance.GetToken().AddOnCompleteListener(new FirebaseTokenListener(completion));
        return await completion.Task;
    }

    private static string? GetData(RemoteMessage message, string key)
    {
        return message.Data.TryGetValue(key, out var value) ? value : null;
    }

    private sealed class FirebaseTokenListener(TaskCompletionSource<string> completion) : Java.Lang.Object, global::Android.Gms.Tasks.IOnCompleteListener
    {
        public void OnComplete(global::Android.Gms.Tasks.Task task)
        {
            if (task.IsSuccessful && task.Result is not null)
            {
                completion.TrySetResult(task.Result.ToString() ?? string.Empty);
                return;
            }

            completion.TrySetException(new InvalidOperationException(
                task.Exception?.Message ?? "Firebase nevrátil registrační token."));
        }
    }
}

[Service(Exported = false)]
[IntentFilter(["com.google.firebase.MESSAGING_EVENT"])]
public sealed class Diva2FirebaseMessagingService : FirebaseMessagingService
{
    public override void OnNewToken(string token)
    {
        base.OnNewToken(token);
        _ = AndroidPushNotificationService.TokenChangedAsync(token);
    }

    public override void OnMessageReceived(RemoteMessage message)
    {
        base.OnMessageReceived(message);
        AndroidPushNotificationService.ShowNotification(message);
    }
}
