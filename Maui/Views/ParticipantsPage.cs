using Diva2Maui.Models;
using Diva2Maui.Services;

namespace Diva2Maui.Views;

public partial class ParticipantsPage : ContentPage
{
    private readonly Diva2ApiClient apiClient;
    private readonly int branchId;
    private readonly int lessonId;

    public ParticipantsPage(Diva2ApiClient apiClient, IReadOnlyList<LessonParticipantInfo> participants, int branchId, int lessonId)
    {
        InitializeComponent();
        this.apiClient = apiClient;
        this.branchId = branchId;
        this.lessonId = lessonId;
        IdentityHeader.DisplayName = apiClient.DisplayName;
        IdentityHeader.TenantName = apiClient.SelectedTenant?.Name ?? string.Empty;
        ParticipantsView.ItemsSource = participants;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (apiClient.Data.Timestamp is not null)
        {
            try
            {
                await RefreshSynchronizedDataAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Participant refresh failed: {ex.Message}");
            }
        }
    }

    public async Task RefreshSynchronizedDataAsync()
    {
        if (!apiClient.Data.BranchConfigurations.TryGetValue(branchId, out var configuration)
            || !configuration.IsEnabled("lekceUkazZakazniky")
            || !apiClient.Data.Lessons.TryGetValue(branchId, out var lessons)
            || !lessons.Any(item => item.Id == lessonId))
        {
            ParticipantsView.ItemsSource = Array.Empty<LessonParticipantInfo>();
            return;
        }
        ParticipantsView.ItemsSource = apiClient.Data.Participants.TryGetValue((branchId, lessonId), out var participants)
            ? participants : await apiClient.GetLessonParticipantsAsync(branchId, lessonId);
    }
}
