using Diva2Maui.Models;
using Diva2Maui.Services;

namespace Diva2Maui.Views;

public partial class MyPage : ContentPage
{
    private readonly Diva2ApiClient apiClient;

    public MyPage(Diva2ApiClient apiClient, string displayName)
    {
        InitializeComponent();
        this.apiClient = apiClient;
        IdentityHeader.DisplayName = displayName;
        IdentityHeader.TenantName = apiClient.SelectedTenant?.Name ?? string.Empty;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        CreditsContainer.Clear();
        try
        {
            foreach (var credits in await apiClient.GetMyCreditsAsync())
            {
                AddCredits(credits);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Moje kredity", ex.Message, "OK");
        }
    }

    private void AddCredits(BranchCreditsInfo credits)
    {
        var values = new Grid
        {
            ColumnSpacing = 12,
            RowSpacing = 5,
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star)
            }
        };
        var row = 0;
        if (credits.ShowUnlimitedCredits)
        {
            values.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            values.Add(new Label { Text = "Neomezené:", FontSize = 16 }, 0, row);
            values.Add(new Label { Text = credits.UnlimitedCredits.ToString(), FontSize = 16 }, 1, row++);
        }

        if (credits.ShowTimeCredits && credits.TimeCredits.Count > 0)
        {
            var timeValues = new VerticalStackLayout { Spacing = 3 };
            foreach (var timeCredit in credits.TimeCredits)
            {
                timeValues.Add(new Label
                {
                    Text = $"{timeCredit.Credits}   {timeCredit.ValidityText}",
                    FontSize = 14,
                    TextColor = Colors.Gray
                });
            }

            values.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            values.Add(new Label { Text = "Časové:", FontSize = 16 }, 0, row);
            values.Add(timeValues, 1, row++);
        }

        if (row == 0)
        {
            return;
        }

        var creditContent = new VerticalStackLayout { Spacing = 4 };
        if (!string.IsNullOrWhiteSpace(credits.BranchName))
        {
            creditContent.Add(new Label
            {
                Text = credits.BranchName,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#512BD4")
            });
        }

        creditContent.Add(values);
        CreditsContainer.Add(new Border
        {
            Padding = 10,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
            Content = creditContent
        });
    }

    private void OnLogoutClicked(object? sender, EventArgs e)
    {
        apiClient.Logout();
        if (Window is not null)
        {
            Window.Page = new AppShell();
        }
    }

    private void OnChangeTenantClicked(object? sender, EventArgs e)
    {
        apiClient.Logout();
        Preferences.Default.Remove("selected_tenant");
        if (Window is not null)
        {
            Window.Page = new AppShell();
        }
    }
}
