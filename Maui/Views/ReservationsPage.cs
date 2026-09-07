using System.Collections.ObjectModel;
using Diva2Maui.Models;
using Diva2Maui.Services;

namespace Diva2Maui.Views;

public partial class ReservationsPage : ContentPage
{
    private readonly Diva2ApiClient apiClient;
    private readonly ObservableCollection<MyReservationInfo> reservations = [];
    private bool isLoading;

    public ReservationsPage(Diva2ApiClient apiClient)
    {
        InitializeComponent();
        IdentityHeader.DisplayName = apiClient.DisplayName;
        IdentityHeader.TenantName = apiClient.SelectedTenant?.Name ?? string.Empty;
        this.apiClient = apiClient;
        ReservationsView.ItemsSource = reservations;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadReservationsAsync();
    }

    private async Task LoadReservationsAsync()
    {
        if (isLoading)
        {
            return;
        }

        isLoading = true;
        try
        {
            var currentReservations = await apiClient.GetMyReservationsAsync();
            reservations.Clear();
            foreach (var reservation in currentReservations)
            {
                reservations.Add(reservation);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Moje objednávky", ex.Message, "OK");
        }
        finally
        {
            isLoading = false;
        }
    }

    private async void OnReservationSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not MyReservationInfo reservation)
        {
            return;
        }

        ReservationsView.SelectedItem = null;
        try
        {
            var currentReservation = (await apiClient.GetMyReservationsAsync())
                .FirstOrDefault(item => item.LessonId == reservation.LessonId);
            if (currentReservation is null)
            {
                reservations.Remove(reservation);
                await DisplayAlertAsync("Objednávka zrušena", "Na této hodině už nejste přihlášeni. Seznam byl aktualizován.", "OK");
                return;
            }

            reservation = currentReservation;
            var lesson = (await apiClient.GetLessonsAsync(reservation.BranchId))
                .FirstOrDefault(item => item.Id == reservation.LessonId);
            if (lesson is null)
            {
                await DisplayAlertAsync("Detail hodiny", "Hodinu se nepodařilo najít v aktuálním rozvrhu.", "OK");
                return;
            }

            var configuration = await apiClient.GetBranchConfigurationAsync(reservation.BranchId);
            var lessonTypes = configuration.LessonTypes.ToDictionary(type => type.Id);
            lesson.BranchId = reservation.BranchId;
            lesson.ReservationStatus = reservation.ReservationStatus;
            lesson.IsReservedByCurrentUser = true;
            lesson.CanCancelReservation = reservation.CanCancel;
            lesson.CanOfferReservation = reservation.CanOffer;
            lesson.ShowCapacity = configuration.IsEnabled("rozvrhUkazPocet");
            lesson.CanShowParticipants = configuration.IsEnabled("lekceUkazZakazniky");
            lesson.ShowLessonType = configuration.IsEnabled("rozvrhUkazTyp") && lessonTypes.ContainsKey(lesson.LessonTypeId);
            lesson.LessonTypeName = lessonTypes.TryGetValue(lesson.LessonTypeId, out var type) ? type.Name : "";
            lesson.AvailabilityText = CreateAvailabilityText(
                lesson,
                configuration.GetInt("lekceUkazLimitOd", 1),
                configuration.IsEnabled("public_rozvrh_ukaz_kapacitu"));
            await Navigation.PushAsync(new LessonDetailPage(apiClient, lesson));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Detail hodiny", ex.Message, "OK");
        }
    }

    private static string CreateAvailabilityText(LessonInfo lesson, int limit, bool showTotalCapacity)
    {
        var free = lesson.Capacity - lesson.Occupied;
        if (free <= 0)
        {
            return "Obsazeno";
        }

        var freeText = limit > 0 && free > limit ? $"{limit} a více" : free.ToString();
        return showTotalCapacity ? $"Volné: {freeText} z {lesson.Capacity}" : $"Volné: {freeText}";
    }
}
