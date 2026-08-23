using System.Globalization;

namespace Diva2Maui.Models;

public sealed class BranchCreditsInfo
{
    public int BranchId { get; set; }
    public string BranchName { get; set; } = "";
    public bool ShowUnlimitedCredits { get; set; }
    public int UnlimitedCredits { get; set; }
    public bool ShowTimeCredits { get; set; }
    public List<TimeCreditInfo> TimeCredits { get; set; } = [];
}

public sealed class TimeCreditInfo
{
    public int Credits { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset? ValidFrom { get; set; }
    public DateTimeOffset? ValidTo { get; set; }
    public string ValidityText => IsActive && ValidFrom.HasValue && ValidTo.HasValue
        ? $"{ValidFrom.Value.DateTime.ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("cs-CZ"))} – {ValidTo.Value.DateTime.ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("cs-CZ"))}"
        : "Neaktivováno";
}
