using Diva2Maui.Services;
using Diva2Maui.Views;

namespace Diva2Maui;

public sealed class AuthenticatedShell : Shell
{
    public AuthenticatedShell(Diva2ApiClient apiClient, string displayName)
    {
        Title = apiClient.SelectedTenant?.Name ?? "Diva2";
        FlyoutBehavior = FlyoutBehavior.Disabled;

        var tabs = new TabBar();
        tabs.Items.Add(CreateTab("Hlavní", "home", "nav_home.svg", new HomePage(apiClient, displayName)));
        tabs.Items.Add(CreateTab("Lekce", "branches", "nav_list.svg", new BranchesPage(apiClient, displayName)));
        tabs.Items.Add(CreateTab("Objednávky", "reservations", "nav_orders.svg", new ReservationsPage(apiClient)));
        tabs.Items.Add(CreateTab("Moje", "my", "nav_my.svg", new MyPage(apiClient, displayName)));
        Items.Add(tabs);
    }

    private static ShellContent CreateTab(string title, string route, string icon, ContentPage page) => new()
    {
        Title = title,
        Route = route,
        Icon = icon,
        Content = page
    };
}
