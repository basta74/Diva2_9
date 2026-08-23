using Diva2Maui.Services;

namespace Diva2Maui.Views;

public partial class HomePage : ContentPage
{
    public HomePage(Diva2ApiClient apiClient, string displayName)
    {
        InitializeComponent();
        IdentityHeader.DisplayName = displayName;
        IdentityHeader.TenantName = apiClient.SelectedTenant?.Name ?? string.Empty;
        var tenantDomain = GetTenantDomain(apiClient);
        Title = tenantDomain;
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
