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
    public int EntryCount { get; set; } = 1;
    public int WaitingEntryCount { get; set; }
    public string EntryCountText => EntryCount == 1 ? "1 vstup" : EntryCount < 5 ? $"{EntryCount} vstupy" : $"{EntryCount} vstupů";

    public static IEnumerable<MyReservationInfo> GroupByLesson(IEnumerable<MyReservationInfo> items)
    {
        return items.GroupBy(item => (item.BranchId, item.LessonId)).Select(group =>
        {
            var first = group.First();
            var count = group.Sum(item => item.EntryCount);
            var waiting = group.Sum(item => item.ReservationStatus == "waitingList" ? item.EntryCount : item.WaitingEntryCount);
            return new MyReservationInfo
            {
                LessonId = first.LessonId,
                BranchId = first.BranchId,
                LessonName = first.LessonName,
                StartsAt = first.StartsAt,
                EntryCount = count,
                WaitingEntryCount = waiting,
                ReservationStatus = waiting == count ? "waitingList" : "customer",
                CanCancel = group.Any(item => item.CanCancel),
                CanOffer = group.Any(item => item.CanOffer)
            };
        });
    }
    public string DateText => StartsAt.DateTime.ToString("dddd d. M.", System.Globalization.CultureInfo.GetCultureInfo("cs-CZ"));
    public string TimeText => StartsAt.DateTime.ToString("HH:mm");
    public string StatusText => ReservationStatus == "waitingList" ? "Náhradník"
        : WaitingEntryCount > 0 ? $"Objednáno · náhradníci: {WaitingEntryCount}" : "Objednáno";
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
