namespace Diva2Maui.Models;

/// <summary>Downloaded data for the current tenant and signed-in customer.</summary>
public sealed class MauiSessionData
{
    public string? Timestamp { get; internal set; }
    public IReadOnlyList<BranchInfo> Branches { get; internal set; } = [];
    public IReadOnlyDictionary<int, IReadOnlyList<LessonInfo>> Lessons { get; internal set; }
        = new Dictionary<int, IReadOnlyList<LessonInfo>>();
    public IReadOnlyList<MyReservationInfo> Reservations { get; internal set; } = [];
    public IReadOnlyDictionary<(int BranchId, int LessonId), IReadOnlyList<LessonParticipantInfo>> Participants { get; internal set; }
        = new Dictionary<(int, int), IReadOnlyList<LessonParticipantInfo>>();
    public IReadOnlyDictionary<int, BranchConfiguration> BranchConfigurations { get; internal set; }
        = new Dictionary<int, BranchConfiguration>();

    public IReadOnlyList<BranchCreditsInfo> Credits { get; internal set; } = [];

    // Credits remain grouped by the cash desk grouping returned by the API.
    public IEnumerable<TimeCreditInfo> TimeCredits => Credits.SelectMany(credits => credits.TimeCredits);

    public bool IsLoaded { get; internal set; }
}
