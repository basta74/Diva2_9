namespace Diva2Maui.Models;

public sealed class LessonPaymentPreview
{
    public int LessonCredits { get; set; }
    public int ValidCredits { get; set; }
    public int AvailableCredits { get; set; }
    public int MissingCredits { get; set; }
    public int ReservationCount { get; set; }
    public int MaxReservations { get; set; }
    public bool CanReserve { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<CreditPaymentPart> Parts { get; set; } = [];
}

public sealed class CreditPaymentPart
{
    public int SourceId { get; set; }
    public string Type { get; set; } = string.Empty;
    public int Credits { get; set; }
    public DateTimeOffset? ValidTo { get; set; }
    public bool ActivatesOnUse { get; set; }
}
