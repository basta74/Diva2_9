using System.Collections.ObjectModel;
using System.Windows.Input;
using Diva2Maui.Models;
using Diva2Maui.Services;

namespace Diva2Maui.Views;

public partial class HomePage : ContentPage
{
    private readonly Diva2ApiClient apiClient;
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

    public HomePage(Diva2ApiClient apiClient, string displayName)
    {
        InitializeComponent();
        this.apiClient = apiClient;
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
        if (IsLoading) return;
        IsLoading = true;
        StatusText = Notifications.Count == 0 ? "Načítám…" : "Aktualizuji…";
        try
        {
            var items = await apiClient.GetNotificationsAsync();
            Notifications.Clear();
            foreach (var item in items.OrderByDescending(x => x.CreatedAt)) Notifications.Add(item);
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
