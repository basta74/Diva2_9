using System.Collections.ObjectModel;
using System.Globalization;

namespace Diva2Maui.Models;
public sealed class LessonInfo
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string Name { get; set; } = "";
    public string Lector { get; set; } = "";
    public DateTimeOffset StartsAt { get; set; }
    public int Capacity { get; set; }
    public int Occupied { get; set; }
    public bool IsReservedByCurrentUser { get; set; }
    public string? ReservationStatus { get; set; }
    public bool CanCancelReservation { get; set; }
    public bool CanOfferReservation { get; set; }
    public int LessonTypeId { get; set; }
    public string LessonTypeName { get; set; } = "";
    public bool ShowLessonType { get; set; }
    public bool ShowCapacity { get; set; } = true;
    public bool CanShowParticipants { get; set; }
    public string AvailabilityText { get; set; } = "";
    public bool ShowReservationStatus => !string.IsNullOrWhiteSpace(ReservationStatus);
    public string ReservationStatusText => ReservationStatus == "waitingList" ? "Náhradník" : "Objednáno";
    public Color CardColor => ReservationStatus switch
    {
        "customer" => Color.FromArgb("#DDF5E3"),
        "waitingList" => Color.FromArgb("#FFF0CC"),
        _ => Colors.Transparent
    };
    public string ReservationPossibilityText => ReservationStatus switch
    {
        "customer" => "Jste přihlášen jako zákazník.",
        "waitingList" => "Jste přihlášen jako náhradník.",
        _ when StartsAt.DateTime <= DateTime.Now => "Na tuto hodinu se již nelze přihlásit.",
        _ => "Na tuto hodinu se můžete přihlásit."
    };
    public bool ShowName => !string.IsNullOrWhiteSpace(Name);
    public string DateText => StartsAt.DateTime.ToString("ddd d. M.");
    public string TimeText => StartsAt.DateTime.ToString("HH:mm");
    public string CapacityText => $"Obsazeno {Occupied} z {Capacity}";
}

public sealed class LessonDayGroup : ObservableCollection<LessonInfo>
{
    public string DateText { get; }

    public LessonDayGroup(DateTime date, IEnumerable<LessonInfo> lessons) : base(lessons)
    {
        DateText = $"{date.ToString("dddd d. M.", CultureInfo.GetCultureInfo("cs-CZ"))} / {Count}";
    }
}
