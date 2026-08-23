namespace Diva2Maui.Models;

public sealed class MyReservationInfo
{
    public int LessonId { get; set; }
    public int BranchId { get; set; }
    public string LessonName { get; set; } = "";
    public DateTimeOffset StartsAt { get; set; }
    public string ReservationStatus { get; set; } = "";
    public bool CanCancel { get; set; }
    public bool CanOffer { get; set; }
    public string DateText => StartsAt.DateTime.ToString("dddd d. M.", System.Globalization.CultureInfo.GetCultureInfo("cs-CZ"));
    public string TimeText => StartsAt.DateTime.ToString("HH:mm");
    public string StatusText => ReservationStatus == "waitingList" ? "Náhradník" : "Objednáno";
    public Color CardColor => Color.FromArgb(ReservationStatus == "waitingList" ? "#FFF0CC" : "#DDF5E3");
}

public sealed class LessonParticipantInfo
{
    public string DisplayName { get; set; } = "";
    public bool IsWaitingList { get; set; }
    public bool IsCurrentUser { get; set; }
    public string StatusText => IsWaitingList ? "Náhradník" : "Zákazník";
}

public sealed class CreateReservationResponse
{
    public bool Success { get; set; }
    public string ReservationStatus { get; set; } = "";
    public string Message { get; set; } = "";
}
