using System.Collections.ObjectModel;
using Diva2Maui.Models;
using Diva2Maui.Services;

namespace Diva2Maui.Views;

public partial class BranchesPage : ContentPage
{
    private readonly Diva2ApiClient apiClient;
    private readonly ObservableCollection<LessonDayGroup> days = [];
    private readonly List<Button> tabButtons = [];
    private bool loaded;
    private int loadedReservationVersion = -1;
    private BranchInfo? selectedBranch;
    private Button? selectedButton;

    public BranchesPage(Diva2ApiClient apiClient, string displayName)
    {
        InitializeComponent();
        IdentityHeader.DisplayName = displayName;
        IdentityHeader.TenantName = apiClient.SelectedTenant?.Name ?? string.Empty;
        Shell.SetNavBarIsVisible(this, false);
        this.apiClient = apiClient;
        LessonsView.ItemsSource = days;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (loaded)
        {
            if (loadedReservationVersion != apiClient.ReservationVersion
                && selectedBranch is not null
                && selectedButton is not null)
            {
                await SelectBranchAsync(selectedBranch, selectedButton);
            }

            return;
        }
        loaded = true;
        try
        {
            var branches = (await apiClient.GetBranchesAsync())
                .Where(branch => !branch.Name.Contains("video", StringComparison.OrdinalIgnoreCase))
                .Take(3).ToList();
            CreateTabs(branches);
            if (branches.Count > 0) await SelectBranchAsync(branches[0], tabButtons[0]);
            else EmptyText.Text = "Nejsou dostupné žádné pobočky.";
        }
        catch (Exception ex)
        {
            EmptyText.Text = "Pobočky se nepodařilo načíst.";
            await DisplayAlertAsync("Chyba připojení", ex.Message, "OK");
        }
    }

    private void CreateTabs(IReadOnlyList<BranchInfo> branches)
    {
        BranchTabs.Clear();
        tabButtons.Clear();
        for (var index = 0; index < branches.Count; index++)
        {
            BranchTabs.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            var branch = branches[index];
            var button = new Button
            {
                Text = branch.Name,
                Padding = new Thickness(4, 2),
                Margin = 0,
                HeightRequest = 40,
                CornerRadius = 8,
                FontSize = 14,
                LineBreakMode = LineBreakMode.TailTruncation
            };
            button.Clicked += async (_, _) => await SelectBranchAsync(branch, button);
            Grid.SetColumn(button, index);
            BranchTabs.Add(button);
            tabButtons.Add(button);
        }
    }

    private async Task SelectBranchAsync(BranchInfo branch, Button selectedButton)
    {
        selectedBranch = branch;
        this.selectedButton = selectedButton;
        foreach (var button in tabButtons) button.Opacity = ReferenceEquals(button, selectedButton) ? 1 : 0.55;
        days.Clear();
        EmptyText.Text = "Načítám…";
        LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = true;
        try
        {
            var configuration = await apiClient.GetBranchConfigurationAsync(branch.Id);
            var lessonTypes = configuration.LessonTypes.ToDictionary(type => type.Id);
            var showCapacity = configuration.IsEnabled("rozvrhUkazPocet");
            var showLessonType = configuration.IsEnabled("rozvrhUkazTyp");
            var showParticipants = configuration.IsEnabled("lekceUkazZakazniky");
            var availabilityLimit = configuration.GetInt("lekceUkazLimitOd", 1);
            var showTotalCapacity = configuration.IsEnabled("public_rozvrh_ukaz_kapacitu");
            var lessons = await apiClient.GetLessonsAsync(branch.Id);
            var reservationStatuses = (await apiClient.GetMyReservationsAsync())
                .Where(item => item.BranchId == branch.Id)
                .GroupBy(item => item.LessonId)
                .ToDictionary(group => group.Key, group => group.First());
            foreach (var lesson in lessons)
            {
                lesson.BranchId = branch.Id;
                var reservation = reservationStatuses.GetValueOrDefault(lesson.Id);
                lesson.ReservationStatus = reservation?.ReservationStatus;
                lesson.IsReservedByCurrentUser = reservation is not null;
                lesson.CanCancelReservation = reservation?.CanCancel == true;
                lesson.CanOfferReservation = reservation?.CanOffer == true;
                lesson.ShowCapacity = showCapacity;
                lesson.CanShowParticipants = showParticipants;
                lesson.AvailabilityText = CreateAvailabilityText(lesson, availabilityLimit, showTotalCapacity);
                lesson.ShowLessonType = showLessonType && lessonTypes.ContainsKey(lesson.LessonTypeId);
                lesson.LessonTypeName = lessonTypes.TryGetValue(lesson.LessonTypeId, out var type) ? type.Name : "";
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"DIVA2 MAUI pobočka {branch.Id}: API objednávky {reservationStatuses.Count}, podbarvené lekce {lessons.Count(item => item.ReservationStatus is not null)}");
            var debugReservations = (await apiClient.GetMyReservationsAsync()).Where(item => item.BranchId == branch.Id).ToList();
            foreach (var reservation in debugReservations)
            {
                var matchedLesson = lessons.FirstOrDefault(item => item.Id == reservation.LessonId);
                System.Diagnostics.Debug.WriteLine(
                    $"DIVA2 MATCH branch={reservation.BranchId} lesson={reservation.LessonId} " +
                    $"objednávka='{reservation.LessonName}' {reservation.StartsAt:O} -> " +
                    (matchedLesson is null
                        ? "LEKCE NENALEZENA"
                        : $"lekce='{matchedLesson.Name}' {matchedLesson.StartsAt:O}"));
            }
#endif
            foreach (var group in lessons.OrderBy(lesson => lesson.StartsAt).GroupBy(lesson => lesson.StartsAt.DateTime.Date))
                days.Add(new LessonDayGroup(group.Key, group));
            EmptyText.Text = days.Count == 0 ? "Nejsou vypsané žádné budoucí hodiny." : "";
            loadedReservationVersion = apiClient.ReservationVersion;
        }
        catch (Exception ex)
        {
            EmptyText.Text = "Rozvrh se nepodařilo načíst.";
            await DisplayAlertAsync("Chyba připojení", ex.Message, "OK");
        }
        finally { LoadingIndicator.IsRunning = LoadingIndicator.IsVisible = false; }
    }

    private static string CreateAvailabilityText(LessonInfo lesson, int limit, bool showTotalCapacity)
    {
        var free = lesson.Capacity - lesson.Occupied;
        if (free <= 0) return "Obsazeno";

        var freeText = limit > 0 && free > limit ? $"{limit} a více" : free.ToString();
        return showTotalCapacity ? $"Volné: {freeText} z {lesson.Capacity}" : $"Volné: {freeText}";
    }

    private async void OnLessonSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not LessonInfo lesson) return;
        LessonsView.SelectedItem = null;
        await Navigation.PushAsync(new LessonDetailPage(apiClient, lesson));
    }
}
