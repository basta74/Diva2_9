using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace Diva2Maui;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private const int NotificationPermissionRequestCode = 1001;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu
            && CheckSelfPermission(Android.Manifest.Permission.PostNotifications) != Permission.Granted)
        {
            RequestPermissions([Android.Manifest.Permission.PostNotifications], NotificationPermissionRequestCode);
        }

        HandleNotificationIntent(Intent);
    }

    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);
        Intent = intent;
        HandleNotificationIntent(intent);
    }

    private static void HandleNotificationIntent(Intent? intent)
    {
        if (intent?.HasExtra("notificationId") != true)
        {
            return;
        }

        Platforms.Android.AndroidPushNotificationService.MarkNotificationsToOpen();
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (Shell.Current is AuthenticatedShell shell)
            {
                await shell.OpenNotificationsAsync();
            }
        });
    }
}
