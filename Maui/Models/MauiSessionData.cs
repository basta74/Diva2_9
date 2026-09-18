namespace Diva2Maui.Models;

/// <summary>Downloaded data for the current tenant and signed-in customer.</summary>
public sealed class MauiSessionData
{
    public IReadOnlyDictionary<int, BranchConfiguration> BranchConfigurations { get; internal set; }
        = new Dictionary<int, BranchConfiguration>();

    public IReadOnlyList<BranchCreditsInfo> Credits { get; internal set; } = [];

    // Credits remain grouped by the cash desk grouping returned by the API.
    public IEnumerable<TimeCreditInfo> TimeCredits => Credits.SelectMany(credits => credits.TimeCredits);

    public bool IsLoaded { get; internal set; }
}
