// Platform services are not exercised by these HTTP synchronization tests.
public class Color { public static Color FromArgb(string value) => new(); }
public static class Colors { public static Color Transparent => new(); }
public enum FontAttributes { None, Bold }
public class BindableObject { protected void OnPropertyChanged(string? name = null) { } }
public static class FileSystem { public static string CacheDirectory => Path.GetTempPath(); }
public class Preferences
{
    public static Preferences Default { get; } = new();
    public void Set(string key, string value) { }
    public string Get(string key, string fallback) => fallback;
    public void Remove(string key) { }
}
public class SecureStorage
{
    public static SecureStorage Default { get; } = new();
    public Task SetAsync(string key, string value) => Task.CompletedTask;
    public Task<string?> GetAsync(string key) => Task.FromResult<string?>(null);
    public void Remove(string key) { }
}
namespace Diva2Maui.Services
{
    public static class PushNotificationRegistration
    {
        public static Task RegisterAsync(Diva2ApiClient client, CancellationToken token = default) => Task.CompletedTask;
    }
}
