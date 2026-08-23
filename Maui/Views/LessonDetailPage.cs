using Diva2Maui.Models;
using Diva2Maui.Services;

namespace Diva2Maui.Views;

public partial class LessonDetailPage : ContentPage
{
    private readonly Diva2ApiClient apiClient;
    private readonly LessonInfo lesson;

    public LessonDetailPage(Diva2ApiClient apiClient, LessonInfo lesson)
    {
        InitializeComponent();
        IdentityHeader.DisplayName = apiClient.DisplayName;
        IdentityHeader.TenantName = apiClient.SelectedTenant?.Name ?? string.Empty;
        this.apiClient = apiClient;
        this.lesson = lesson;
        DateLabel.Text = lesson.StartsAt.DateTime.ToString(
            "dddd d. M. yyyy", System.Globalization.CultureInfo.GetCultureInfo("cs-CZ"));
        TimeLabel.Text = lesson.TimeText;
        SetValue(LectorContainer, LectorLabel, lesson.Lector);
        SetValue(NameContainer, NameLabel, lesson.Name, lesson.ShowName);
        SetValue(TypeContainer, TypeLabel, lesson.LessonTypeName, lesson.ShowLessonType);
        SetValue(CapacityContainer, CapacityLabel, lesson.AvailabilityText, lesson.ShowCapacity);
        ReservationLabel.Text = lesson.ReservationPossibilityText;
        ReserveButton.IsVisible = string.IsNullOrWhiteSpace(lesson.ReservationStatus)
            && lesson.StartsAt.DateTime > DateTime.Now;
        CancelReservationButton.Text = lesson.CanOfferReservation ? "Nabídnout místo" : "Odhlásit se";
        CancelReservationButton.IsVisible = !string.IsNullOrWhiteSpace(lesson.ReservationStatus)
            && (lesson.CanCancelReservation || lesson.CanOfferReservation);
        ParticipantsButton.IsVisible = lesson.CanShowParticipants;
    }

    private async void OnReserveClicked(object? sender, EventArgs e)
    {
        if (!await DisplayAlertAsync("Přihlášení na hodinu", "Opravdu se chcete přihlásit?", "Ano", "Ne"))
        {
            return;
        }

        ReserveButton.IsEnabled = false;
        try
        {
            var result = await apiClient.CreateReservationAsync(lesson.BranchId, lesson.Id);
            lesson.ReservationStatus = result.ReservationStatus;
            lesson.IsReservedByCurrentUser = true;
            ReservationLabel.Text = result.Message;
            ReserveButton.IsVisible = false;
            CancelReservationButton.IsVisible = true;
            await DisplayAlertAsync("Objednávka", result.Message, "OK");
            await NavigateToReservationsAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Objednávka se nezdařila", ex.Message, "OK");
            ReserveButton.IsEnabled = true;
        }
    }

    private async void OnCancelReservationClicked(object? sender, EventArgs e)
    {
        var title = lesson.CanOfferReservation ? "Nabídnutí místa" : "Odhlášení z hodiny";
        var question = lesson.CanOfferReservation
            ? "Opravdu chcete nabídnout své místo dalším zájemcům?"
            : "Opravdu se chcete odhlásit?";
        if (!await DisplayAlertAsync(title, question, "Ano", "Ne"))
        {
            return;
        }

        CancelReservationButton.IsEnabled = false;
        try
        {
            var result = await apiClient.CancelReservationAsync(lesson.BranchId, lesson.Id);
            lesson.ReservationStatus = null;
            lesson.IsReservedByCurrentUser = false;
            ReservationLabel.Text = result.Message;
            CancelReservationButton.IsVisible = false;
            ReserveButton.IsVisible = lesson.StartsAt.DateTime > DateTime.Now;
            ReserveButton.IsEnabled = true;
            await DisplayAlertAsync("Odhlášení", result.Message, "OK");
            await NavigateToReservationsAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Odhlášení se nezdařilo", ex.Message, "OK");
            CancelReservationButton.IsEnabled = true;
        }
    }

    private async Task NavigateToReservationsAsync()
    {
        if (Navigation.NavigationStack.Count > 1)
        {
            await Navigation.PopToRootAsync(false);
        }

        await Shell.Current.GoToAsync("//reservations");
    }

    private static void SetValue(VisualElement container, Label label, string value, bool visible = true)
    {
        container.IsVisible = visible && !string.IsNullOrWhiteSpace(value);
        label.Text = value;
    }

    private async void OnParticipantsClicked(object? sender, EventArgs e)
    {
        try
        {
            var participants = await apiClient.GetLessonParticipantsAsync(lesson.BranchId, lesson.Id);
            await Navigation.PushAsync(new ParticipantsPage(apiClient, participants));
        }
        catch (Exception ex) { await DisplayAlertAsync("Účastníci", ex.Message, "OK"); }
    }
}
