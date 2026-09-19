using Diva2Maui.Services;
using Diva2Maui.Views;

namespace Diva2Maui;

public sealed class AuthenticatedShell : Shell
{
    private readonly ShellContent homeTab;
    private readonly ShellContent reservationsTab;
    private int pendingNotificationCount;
    private int reservationCount;
    private readonly Diva2ApiClient apiClient;
    private CancellationTokenSource? synchronization;

    public AuthenticatedShell(Diva2ApiClient apiClient, string displayName)
    {
        this.apiClient = apiClient;
        Title = apiClient.SelectedTenant?.Name ?? "Diva2";
        FlyoutBehavior = FlyoutBehavior.Disabled;

        var tabs = new TabBar();
        homeTab = CreateTab("Hlavní", "home", "nav_home.svg", new HomePage(apiClient, displayName, SetNotificationBadge));
        tabs.Items.Add(homeTab);
        tabs.Items.Add(CreateTab("Lekce", "branches", "nav_list.svg", new BranchesPage(apiClient, displayName)));
        reservationsTab = CreateTab("Objednávky", "reservations", "nav_orders.svg", new ReservationsPage(apiClient, SetReservationBadge));
        tabs.Items.Add(reservationsTab);
        tabs.Items.Add(CreateTab("Moje", "my", "nav_my.svg", new MyPage(apiClient, displayName)));
        Items.Add(tabs);
        Loaded += async (_, _) =>
        {
            StartSynchronization();
            ApplyBadges();
#if ANDROID
            if (Platforms.Android.AndroidPushNotificationService.ConsumeNotificationsToOpen())
            {
                await OpenNotificationsAsync();
                return;
            }
#endif
            await RefreshBadgesAsync();
        };
        Unloaded += (_, _) => StopSynchronization();
    }

    public void StartSynchronization()
    {
        if (synchronization is not null)
        {
            return;
        }
        synchronization = new CancellationTokenSource();
        _ = SynchronizeAsync(synchronization.Token);
    }

    public void StopSynchronization()
    {
        synchronization?.Cancel();
        synchronization?.Dispose();
        synchronization = null;
    }

    private async Task SynchronizeAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        try
        {
            do
            {
                try
                {
                    if (await apiClient.SynchronizeDomainAsync(cancellationToken))
                    {
                        SetReservationBadge(apiClient.Data.Reservations.Count);
                        switch (CurrentPage)
                        {
                            case BranchesPage branches:
                                await branches.RefreshSynchronizedDataAsync();
                                break;
                            case ReservationsPage reservations:
                                reservations.RefreshSynchronizedData();
                                break;
                            case MyPage my:
                                my.RefreshSynchronizedData();
                                break;
                            case ParticipantsPage participants:
                                await participants.RefreshSynchronizedDataAsync();
                                break;
                            case LessonDetailPage detail:
                                await detail.RefreshSynchronizedDataAsync();
                                break;
                        }
                        await ((HomePage)homeTab.Content).RefreshNotificationsAsync();
                    }
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Domain synchronization failed: {ex.Message}");
                }
            }
            while (await timer.WaitForNextTickAsync(cancellationToken));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
    }

    private void SetNotificationBadge(int count)
    {
        pendingNotificationCount = Math.Max(0, count);
        Dispatcher.Dispatch(ApplyBadges);
    }

    private void SetReservationBadge(int count)
    {
        reservationCount = Math.Max(0, count);
        Dispatcher.Dispatch(ApplyBadges);
    }

    public async Task RefreshBadgesAsync()
    {
        try
        {
            var notifications = await ((HomePage)homeTab.Content).GetPendingNotificationCountAsync();
            var reservations = await ((ReservationsPage)reservationsTab.Content).GetReservationCountAsync();
            SetNotificationBadge(notifications);
            SetReservationBadge(reservations);
        }
        catch
        {
            ApplyBadges();
        }
    }

    public async Task OpenNotificationsAsync()
    {
#if ANDROID
        Platforms.Android.AndroidPushNotificationService.ConsumeNotificationsToOpen();
#endif
        await GoToAsync("//home");
        await ((HomePage)homeTab.Content).RefreshNotificationsAsync();
    }

    private void ApplyBadges()
    {
#if ANDROID
        var root = Handler?.PlatformView as Android.Views.View;
        var navigation = FindBottomNavigation(root);
        if (navigation is not null)
        {
            ApplyAndroidBadge(navigation, 0, pendingNotificationCount);
            ApplyAndroidBadge(navigation, 2, reservationCount);
            homeTab.Title = "Hlavní";
            reservationsTab.Title = "Objednávky";
            return;
        }
#endif
        homeTab.Title = pendingNotificationCount > 0 ? $"Hlavní ({pendingNotificationCount})" : "Hlavní";
        reservationsTab.Title = reservationCount > 0 ? $"Objednávky ({reservationCount})" : "Objednávky";
    }

#if ANDROID
    private static void ApplyAndroidBadge(Google.Android.Material.BottomNavigation.BottomNavigationView navigation, int itemIndex, int count)
    {
        var item = navigation.Menu?.GetItem(itemIndex);
        if (item is null)
        {
            return;
        }

        if (count > 0)
        {
            var badge = navigation.GetOrCreateBadge(item.ItemId);
            badge.Number = count;
            badge.SetVisible(true);
        }
        else
        {
            navigation.RemoveBadge(item.ItemId);
        }
    }

    private static Google.Android.Material.BottomNavigation.BottomNavigationView? FindBottomNavigation(Android.Views.View? view)
    {
        if (view is Google.Android.Material.BottomNavigation.BottomNavigationView navigation)
        {
            return navigation;
        }

        if (view is not Android.Views.ViewGroup group)
        {
            return null;
        }

        for (var index = 0; index < group.ChildCount; index++)
        {
            var found = FindBottomNavigation(group.GetChildAt(index));
            if (found is not null)
            {
                return found;
            }
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
