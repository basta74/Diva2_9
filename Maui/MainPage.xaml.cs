using System.Collections.ObjectModel;
using Diva2Maui.Models;
using Diva2Maui.Services;
using Diva2Maui.Views;

namespace Diva2Maui;

public partial class MainPage : ContentPage
{
    private readonly Diva2ApiClient apiClient = new();
    private readonly List<TenantInfo> allTenants = [];
    private readonly ObservableCollection<TenantInfo> visibleTenants = [];
    private bool loaded;

    public MainPage()
    {
        InitializeComponent();
        TenantsView.ItemsSource = visibleTenants;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!loaded) { loaded = true; await LoadTenantsAsync(); }
    }

    private async Task LoadTenantsAsync()
    {
        LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = true;
        EmptyText.Text = "Načítám provozovatele…";
        try
        {
            allTenants.Clear();
            allTenants.AddRange(await apiClient.GetTenantsAsync());

            var savedTenantId = Preferences.Default.Get("selected_tenant", "");
            var savedTenant = allTenants.FirstOrDefault(tenant =>
                string.Equals(tenant.Id, savedTenantId, StringComparison.OrdinalIgnoreCase));
            if (savedTenant is not null)
            {
                apiClient.SelectTenant(savedTenant);
                var displayName = await apiClient.TryRestoreSessionAsync();
                if (displayName is not null && Window is not null)
                    Window.Page = new AuthenticatedShell(apiClient, displayName);
                else
                    await Navigation.PushAsync(new LoginPage(apiClient, savedTenant));
                return;
            }

            ApplyFilter(TenantSearch.Text);
            EmptyText.Text = allTenants.Count == 0 ? "Nebyli nalezeni žádní provozovatelé." : "";
        }
        catch (Exception ex)
        {
            visibleTenants.Clear();
            EmptyText.Text = "Katalog se nepodařilo načíst.";
            var details = ex.InnerException is null ? ex.Message : $"{ex.Message}\n{ex.InnerException.Message}";
            await DisplayAlertAsync(
                "Chyba připojení",
                $"{details}\n\nAdresa: {Diva2ApiClient.DefaultCatalogUrl}api/v1/tenants",
                "OK");
        }
        finally { LoadingIndicator.IsRunning = LoadingIndicator.IsVisible = false; }
    }

    private void ApplyFilter(string? query)
    {
        var text = query?.Trim() ?? "";
        var items = allTenants.Where(x => text.Length == 0 || x.Name.Contains(text, StringComparison.CurrentCultureIgnoreCase) || x.Id.Contains(text, StringComparison.OrdinalIgnoreCase));
        visibleTenants.Clear();
        foreach (var item in items) visibleTenants.Add(item);
    }

    private void OnSearchChanged(object? sender, TextChangedEventArgs e) => ApplyFilter(e.NewTextValue);
    private async void OnRefreshClicked(object? sender, EventArgs e) => await LoadTenantsAsync();
    private async void OnTenantSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not TenantInfo tenant) return;
        TenantsView.SelectedItem = null;
        apiClient.SelectTenant(tenant);
        Preferences.Default.Set("selected_tenant", tenant.Id);
        await Navigation.PushAsync(new LoginPage(apiClient, tenant));
    }
}
