using Diva2Maui.Models;
using Diva2Maui.Services;

namespace Diva2Maui.Views;

public partial class LoginPage : ContentPage
{
    private readonly Diva2ApiClient apiClient;

    public LoginPage(Diva2ApiClient apiClient, TenantInfo tenant)
    {
        InitializeComponent();
        this.apiClient = apiClient;
        TenantNameLabel.Text = tenant.Name;
    }

    private async Task LoginAsync()
    {
        ErrorLabel.IsVisible = false;
        if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            ErrorLabel.Text = "Zadejte e-mail a heslo.";
            ErrorLabel.IsVisible = true;
            return;
        }

        LoginButton.IsEnabled = false;
        LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = true;
        try
        {
            var login = await apiClient.LoginAsync(EmailEntry.Text.Trim(), PasswordEntry.Text);
            if (Window is not null)
                Window.Page = new AuthenticatedShell(apiClient, login.DisplayName);
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
        finally
        {
            PasswordEntry.Text = "";
            LoginButton.IsEnabled = true;
            LoadingIndicator.IsRunning = LoadingIndicator.IsVisible = false;
        }
    }

    private async void OnLoginClicked(object? sender, EventArgs e) => await LoginAsync();
    private async void OnPasswordCompleted(object? sender, EventArgs e) => await LoginAsync();
    private async void OnBackClicked(object? sender, EventArgs e) => await Navigation.PopAsync();
}
