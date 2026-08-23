using System.Collections.ObjectModel;
using Diva2Maui.Models;
using Diva2Maui.Services;

namespace Diva2Maui.Views;

public partial class LessonsPage : ContentPage
{
    private readonly Diva2ApiClient apiClient;
    private readonly BranchInfo branch;
    private readonly ObservableCollection<LessonInfo> lessons = [];

    public LessonsPage(Diva2ApiClient apiClient, BranchInfo branch)
    {
        InitializeComponent();
        IdentityHeader.DisplayName = apiClient.DisplayName;
        IdentityHeader.TenantName = apiClient.SelectedTenant?.Name ?? string.Empty;
        this.apiClient = apiClient;
        this.branch = branch;
        BranchNameLabel.Text = branch.Name;
        LessonsView.ItemsSource = lessons;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (lessons.Count > 0) return;
        try
        {
            foreach (var lesson in await apiClient.GetLessonsAsync(branch.Id))
            {
                lessons.Add(lesson);
            }
            EmptyText.Text = lessons.Count == 0 ? "Nejsou vypsané žádné budoucí lekce." : "";
        }
        catch (Exception ex)
        {
            EmptyText.Text = "Lekce se nepodařilo načíst.";
            await DisplayAlertAsync("Chyba připojení", ex.Message, "OK");
        }
    }
}
