namespace Diva2Maui.Services;

public static class PushNotificationRegistration
{
    public static async Task RegisterAsync(Diva2ApiClient apiClient, CancellationToken cancellationToken = default)
    {
#if ANDROID
        try
        {
            await Platforms.Android.AndroidPushNotificationService.RegisterAsync(apiClient, cancellationToken);
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine($"Registrace push notifikací se nezdařila: {exception.Message}");
        }
#else
        await Task.CompletedTask;
#endif
    }
}
