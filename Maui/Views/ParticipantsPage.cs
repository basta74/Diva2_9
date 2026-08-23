using Diva2Maui.Models;
using Diva2Maui.Services;

namespace Diva2Maui.Views;

public partial class ParticipantsPage : ContentPage
{
    public ParticipantsPage(Diva2ApiClient apiClient, IReadOnlyList<LessonParticipantInfo> participants)
    {
        InitializeComponent();
        IdentityHeader.DisplayName = apiClient.DisplayName;
        IdentityHeader.TenantName = apiClient.SelectedTenant?.Name ?? string.Empty;
        ParticipantsView.ItemsSource = participants;
    }
}
