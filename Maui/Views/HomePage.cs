using System.Collections.ObjectModel;
using System.Windows.Input;
using Diva2Maui.Models;
using Diva2Maui.Services;

namespace Diva2Maui.Views;

public partial class HomePage : ContentPage
{
    private static readonly TimeSpan ReminderDelay = TimeSpan.FromMinutes(5);
    private readonly Diva2ApiClient apiClient;
    private readonly Action<int> updateBadge;
    private readonly Dictionary<int, CancellationTokenSource> reminderTimers = [];
    private bool isLoading;
    private string statusText = "";

    public ObservableCollection<NotificationInfo> Notifications { get; } = [];
    public ICommand RefreshCommand { get; }
    public bool IsLoading
    {
        get => isLoading;
        private set { if (isLoading != value) { isLoading = value; OnPropertyChanged(); } }
    }
    public string StatusText
    {
        get => statusText;
        private set { if (statusText != value) { statusText = value; OnPropertyChanged(); } }
    }

    public HomePage(Diva2ApiClient apiClient, string displayName, Action<int> updateBadge)
    {
        InitializeComponent();
        this.apiClient = apiClient;
        this.updateBadge = updateBadge;
        RefreshCommand = new Command(async () => await LoadNotificationsAsync());
        BindingContext = this;
        IdentityHeader.DisplayName = displayName;
        IdentityHeader.TenantName = apiClient.SelectedTenant?.Name ?? string.Empty;
        var tenantDomain = GetTenantDomain(apiClient);
        Title = tenantDomain;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadNotificationsAsync();
    }

    private async Task LoadNotificationsAsync()
    {
        if (IsLoading)
        {
            return;
        }
        IsLoading = true;
        StatusText = Notifications.Count == 0 ? "Načítám…" : "Aktualizuji…";
        try
        {
            var items = await apiClient.GetNotificationsAsync();
            Notifications.Clear();
            foreach (var item in items.OrderByDescending(x => x.CreatedAt)) Notifications.Add(item);
            UpdateBadge();
            await MarkVisibleNotificationsReadAsync();
            ScheduleReminders();
            StatusText = Notifications.Count == 0 ? "" : $"Celkem {Notifications.Count}";
        }
        catch (Exception ex)
        {
            StatusText = $"Notifikace se nepodařilo načíst: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task RefreshNotificationsAsync()
    {
        await LoadNotificationsAsync();
    }

    private async Task MarkVisibleNotificationsReadAsync()
    {
        foreach (var item in Notifications.Where(x => x.ReadAt is null).ToList())
        {
            try
            {
                await apiClient.MarkNotificationReadAsync(item.Id);
                item.ReadAt = DateTimeOffset.Now;
            }
            catch
            {
                // Oznámení zůstane označené jako nepřečtené a další obnovení stav odešle znovu.
            }
        }
    }

    private void ScheduleReminders()
    {
        foreach (var item in Notifications.Where(x => x.IsAwaitingReaction))
        {
            if (reminderTimers.ContainsKey(item.Id))
            {
                continue;
            }
            var cancellation = new CancellationTokenSource();
            reminderTimers[item.Id] = cancellation;
            _ = RemindAfterDelayAsync(item, cancellation.Token);
        }
    }

    private async Task RemindAfterDelayAsync(NotificationInfo item, CancellationToken cancellationToken)
    {
        try
        {
            var elapsed = item.ReadAt is null ? TimeSpan.Zero : DateTimeOffset.Now - item.ReadAt.Value;
            var remaining = ReminderDelay - elapsed;
            if (remaining > TimeSpan.Zero)
            {
                await Task.Delay(remaining, cancellationToken);
            }
            if (cancellationToken.IsCancellationRequested || !item.IsAwaitingReaction)
            {
                return;
            }

            item.ReminderActive = true;
            UpdateBadge();
            await DisplayAlertAsync("Čeká na odpověď", $"{item.Title}\n\nZvolte prosím Mám zájem nebo Nemám zájem.", "Rozumím");
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async void OnAcceptClicked(object? sender, EventArgs e) => await ReactAsync(sender, "Confirmed");
    private async void OnDeclineClicked(object? sender, EventArgs e) => await ReactAsync(sender, "Declined");

    private async void OnDismissClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: NotificationInfo item })
        {
            return;
        }

        await DismissAsync(item);
    }

    private async void OnDismissSwipeInvoked(object? sender, EventArgs e)
    {
        if (sender is not SwipeItem { CommandParameter: NotificationInfo item })
        {
            return;
        }

        if (item.IsAwaitingReaction)
        {
            await DisplayAlertAsync("Nejdříve odpovězte", "Oznámení lze smazat až po odpovědi Mám zájem nebo Nemám zájem.", "OK");
            return;
        }

        await DismissAsync(item);
    }

    private async Task DismissAsync(NotificationInfo item)
    {
        try
        {
            await apiClient.DismissNotificationAsync(item.Id);
            if (reminderTimers.Remove(item.Id, out var timer))
            {
                timer.Cancel();
                timer.Dispose();
            }

            Notifications.Remove(item);
            UpdateBadge();
            StatusText = Notifications.Count == 0 ? "" : $"Celkem {Notifications.Count}";
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Oznámení se nepodařilo skrýt", ex.Message, "OK");
        }
    }

    private async Task ReactAsync(object? sender, string reaction)
    {
        if (sender is not Button { CommandParameter: NotificationInfo item } || item.IsResponding)
        {
            return;
        }

        item.IsResponding = true;
        try
        {
            await apiClient.ReactToNotificationAsync(item.Id, reaction);
            if (reminderTimers.Remove(item.Id, out var timer))
            {
                timer.Cancel();
                timer.Dispose();
            }

            Notifications.Remove(item);
            UpdateBadge();
            StatusText = Notifications.Count == 0 ? "" : $"Celkem {Notifications.Count}";
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Odpověď se neodeslala", ex.Message, "OK");
        }
        finally
        {
            item.IsResponding = false;
        }
    }

    private void UpdateBadge() => updateBadge(Notifications.Count(x => x.IsAwaitingReaction));

    public async Task<int> GetPendingNotificationCountAsync()
    {
        var notifications = await apiClient.GetNotificationsAsync();
        return notifications.Count(x => x.IsAwaitingReaction);
    }

    private static string GetTenantDomain(Diva2ApiClient apiClient)
    {
        var tenant = apiClient.SelectedTenant;
        if (tenant is not null
            && Uri.TryCreate(tenant.BaseUrl, UriKind.Absolute, out var tenantUri)
            && !string.IsNullOrWhiteSpace(tenantUri.Host))
        {
            return tenantUri.Host;
        }

        if (!string.IsNullOrWhiteSpace(tenant?.Id))
        {
            return $"{tenant.Id}.diva2.cz";
        }

        return "diva2.cz";
    }
}
