using Diva2Maui.Services;
using Diva2Maui.Views;

namespace Diva2Maui;

public sealed class AuthenticatedShell : Shell
{
    private readonly ShellContent homeTab;
    private int pendingNotificationCount;

    public AuthenticatedShell(Diva2ApiClient apiClient, string displayName)
    {
        Title = apiClient.SelectedTenant?.Name ?? "Diva2";
        FlyoutBehavior = FlyoutBehavior.Disabled;

        var tabs = new TabBar();
        homeTab = CreateTab("Hlavní", "home", "nav_home.svg", new HomePage(apiClient, displayName, SetNotificationBadge));
        tabs.Items.Add(homeTab);
        tabs.Items.Add(CreateTab("Lekce", "branches", "nav_list.svg", new BranchesPage(apiClient, displayName)));
        tabs.Items.Add(CreateTab("Objednávky", "reservations", "nav_orders.svg", new ReservationsPage(apiClient)));
        tabs.Items.Add(CreateTab("Moje", "my", "nav_my.svg", new MyPage(apiClient, displayName)));
        Items.Add(tabs);
        Loaded += (_, _) => ApplyNotificationBadge();
    }

    private void SetNotificationBadge(int count)
    {
        pendingNotificationCount = Math.Max(0, count);
        Dispatcher.Dispatch(ApplyNotificationBadge);
    }

    private void ApplyNotificationBadge()
    {
#if ANDROID
        var root = Handler?.PlatformView as Android.Views.View;
        var navigation = FindBottomNavigation(root);
        var item = navigation?.Menu?.GetItem(0);
        if (navigation is not null && item is not null)
        {
            if (pendingNotificationCount > 0)
            {
                var badge = navigation.GetOrCreateBadge(item.ItemId);
                badge.Number = pendingNotificationCount;
                badge.SetVisible(true);
            }
            else
            {
                navigation.RemoveBadge(item.ItemId);
            }
            homeTab.Title = "Hlavní";
            return;
        }
#endif
        homeTab.Title = pendingNotificationCount > 0 ? $"Hlavní ({pendingNotificationCount})" : "Hlavní";
    }

#if ANDROID
    private static Google.Android.Material.BottomNavigation.BottomNavigationView? FindBottomNavigation(Android.Views.View? view)
    {
        if (view is Google.Android.Material.BottomNavigation.BottomNavigationView navigation) return navigation;
        if (view is not Android.Views.ViewGroup group) return null;
        for (var index = 0; index < group.ChildCount; index++)
        {
            var found = FindBottomNavigation(group.GetChildAt(index));
            if (found is not null) return found;
        }
        return null;
    }
#endif

    private static ShellContent CreateTab(string title, string route, string icon, ContentPage page) => new()
    {
        Title = title,
        Route = route,
        Icon = icon,
        Content = page
    };
}
