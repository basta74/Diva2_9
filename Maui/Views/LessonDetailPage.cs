using Diva2Maui.Models;
using Diva2Maui.Services;

namespace Diva2Maui.Views;

public partial class LessonDetailPage : ContentPage
{
    private readonly Diva2ApiClient apiClient;
    private readonly LessonInfo lesson;
    private NotificationInfo? pendingNotification;
    private LessonPaymentPreview? paymentPreview;
    private bool loadingPayment;
    private bool reserving;

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
        
        if (lesson.Occupied >= lesson.Capacity)
        {
            btnReservation.Text = "Objednat jako náhradník";
            btnReservation.BackgroundColor = Color.FromArgb("#F28C28");
            btnReservation.TextColor = Colors.White;
        }
        else
        {
            btnReservation.Text = "Objednat";
        }

        btnReservation.IsVisible = string.IsNullOrWhiteSpace(lesson.ReservationStatus)
            && lesson.StartsAt.DateTime > DateTime.Now;

        btnCancelReserv.Text = lesson.CanOfferReservation ? "Nabídnout místo" : "Odhlásit se";
        btnCancelReserv.IsVisible = !string.IsNullOrWhiteSpace(lesson.ReservationStatus)
            && (lesson.CanCancelReservation || lesson.CanOfferReservation);
        ParticipantsButton.IsVisible = lesson.CanShowParticipants;
        btnReservation.IsEnabled = false;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        apiClient.NotificationResolved -= OnNotificationResolved;
        apiClient.NotificationResolved += OnNotificationResolved;
        await LoadPaymentAsync();
        await LoadPendingNotificationAsync();
    }

    private async void OnRefreshPaymentClicked(object? sender, EventArgs e)
    {
        await LoadPaymentAsync();
    }

    private async Task LoadPaymentAsync()
    {
        if (loadingPayment)
        {
            return;
        }
        loadingPayment = true;
        paymentPreview = null;
        btnReservation.IsEnabled = false;
        RefreshPaymentButton.IsEnabled = false;
        try
        {
            paymentPreview = await apiClient.GetLessonPaymentPreviewAsync(lesson.Id);
            CreditBalanceLabel.Text = $"Kredity platné {lesson.StartsAt:dd. MM. yyyy HH:mm}: {paymentPreview.ValidCredits}";
            CreditAvailableLabel.Text = $"Použitelné pro další rezervaci po zohlednění objednávek a limitů: {paymentPreview.AvailableCredits}";
            LessonPriceLabel.Text = $"Cena hodiny: {paymentPreview.LessonCredits} kreditů";
            PaymentTitleLabel.Text = paymentPreview.ReservationCount > 0
                ? "Předpokládaná úhrada dalšího místa" : "Předpokládaná úhrada hodiny";
            PaymentBreakdownLabel.Text = string.Join(Environment.NewLine, paymentPreview.Parts.Select(part =>
                part.Type == "time"
                    ? $"Časové: {part.Credits} kreditů, platnost do {part.ValidTo:dd. MM. yyyy HH:mm}"
                        + (part.ActivatesOnUse ? " (aktivace při čerpání)" : "")
                    : $"Neomezené: {part.Credits} kreditů"));
            if (paymentPreview.LessonCredits == 0)
            {
                PaymentBreakdownLabel.Text = "Hodina je bez odečtu kreditů.";
            }
            PaymentMessageLabel.Text = paymentPreview.Message;
            ReservationLabel.Text = paymentPreview.ReservationCount > 0
                ? $"Vaše rezervace na tuto hodinu: {paymentPreview.ReservationCount}"
                : lesson.ReservationPossibilityText;
            var belowLimit = paymentPreview.MaxReservations <= 0 || paymentPreview.ReservationCount < paymentPreview.MaxReservations;
            btnReservation.IsVisible = belowLimit && lesson.StartsAt > DateTimeOffset.Now;
            btnReservation.IsEnabled = paymentPreview.CanReserve && !reserving;
            btnReservation.Text = paymentPreview.ReservationCount > 0 ? "Objednat další místo"
                : lesson.Occupied >= lesson.Capacity ? "Objednat jako náhradník" : "Objednat";
        }
        catch (Exception ex)
        {
            CreditBalanceLabel.Text = "Kredity se nepodařilo načíst.";
            CreditAvailableLabel.Text = string.Empty;
            LessonPriceLabel.Text = string.Empty;
            PaymentBreakdownLabel.Text = string.Empty;
            PaymentMessageLabel.Text = ex.Message;
        }
        finally
        {
            loadingPayment = false;
            RefreshPaymentButton.IsEnabled = !reserving;
        }
    }

    protected override void OnDisappearing()
    {
        apiClient.NotificationResolved -= OnNotificationResolved;
        base.OnDisappearing();
    }

    private void OnNotificationResolved(int notificationId)
    {
        if (pendingNotification?.Id != notificationId)
        {
            return;
        }

        pendingNotification = null;
        Dispatcher.Dispatch(() => NotificationResponseContainer.IsVisible = false);
    }

    private async Task LoadPendingNotificationAsync()
    {
        try
        {
            pendingNotification = (await apiClient.GetNotificationsAsync())
                .FirstOrDefault(item => item.LessonId == lesson.Id
                    && item.Type.Equals("WaitingListPromoted", StringComparison.OrdinalIgnoreCase)
                    && item.IsAwaitingReaction);
            NotificationResponseContainer.IsVisible = pendingNotification is not null;
        }
        catch
        {
            NotificationResponseContainer.IsVisible = false;
        }
    }

    private async void OnNotificationAcceptClicked(object? sender, EventArgs e)
    {
        await ReactToNotificationAsync("Confirmed");
    }

    private async void OnNotificationDeclineClicked(object? sender, EventArgs e)
    {
        await ReactToNotificationAsync("Declined");
    }

    private async Task ReactToNotificationAsync(string reaction)
    {
        if (pendingNotification is null)
        {
            return;
        }

        NotificationResponseContainer.IsEnabled = false;
        try
        {
            await apiClient.ReactToNotificationAsync(pendingNotification.Id, reaction);
            pendingNotification = null;
            NotificationResponseContainer.IsVisible = false;
            if (Shell.Current is AuthenticatedShell shell)
            {
                await shell.RefreshBadgesAsync();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Odpověď se neodeslala", ex.Message, "OK");
        }
        finally
        {
            NotificationResponseContainer.IsEnabled = true;
        }
    }

    private async void OnReserveClicked(object? sender, EventArgs e)
    {
        if (reserving)
        {
            return;
        }
        reserving = true;
        RefreshPaymentButton.IsEnabled = false;
        try
        {
            btnReservation.IsEnabled = false;
            await LoadPaymentAsync();
            if (paymentPreview is null || !paymentPreview.CanReserve)
            {
                return;
            }
            btnReservation.IsEnabled = false;
            var question = paymentPreview.ReservationCount > 0
                ? "Opravdu chcete objednat další místo na této hodině?"
                : "Opravdu se chcete přihlásit?";
            if (!await DisplayAlertAsync("Přihlášení na hodinu", question + Environment.NewLine + PaymentBreakdownLabel.Text, "Ano", "Ne"))
            {
                btnReservation.IsEnabled = true;
                return;
            }
    
            btnReservation.IsEnabled = false;
            try
            {
                CreateReservationResponse result = await apiClient.CreateReservationAsync(lesson.BranchId, lesson.Id);
                lesson.ReservationStatus = result.ReservationStatus;
                lesson.IsReservedByCurrentUser = true;
                ReservationLabel.Text = result.Message;
                btnReservation.IsVisible = false;
                btnCancelReserv.IsVisible = true;
                await DisplayAlertAsync("Objednávka", result.Message, "OK");
                await NavigateToReservationsAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Objednávka se nezdařila", ex.Message, "OK");
                await LoadPaymentAsync();
            }
        }
        finally
        {
            reserving = false;
            RefreshPaymentButton.IsEnabled = true;
            btnReservation.IsEnabled = paymentPreview?.CanReserve == true;
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

        btnCancelReserv.IsEnabled = false;
        try
        {
            var result = await apiClient.CancelReservationAsync(lesson.BranchId, lesson.Id);
            lesson.ReservationStatus = null;
            lesson.IsReservedByCurrentUser = false;
            ReservationLabel.Text = result.Message;
            btnCancelReserv.IsVisible = false;
            btnReservation.IsVisible = lesson.StartsAt.DateTime > DateTime.Now;
            btnReservation.IsEnabled = true;
            await DisplayAlertAsync("Odhlášení", result.Message, "OK");
            await NavigateToReservationsAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Odhlášení se nezdařilo", ex.Message, "OK");
            btnCancelReserv.IsEnabled = true;
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
