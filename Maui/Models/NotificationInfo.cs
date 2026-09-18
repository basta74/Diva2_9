using System.Globalization;
using System.Text.Json.Serialization;

namespace Diva2Maui.Models;

public sealed class NotificationInfo : BindableObject
{
    private DateTimeOffset? readAt;
    private DateTimeOffset? reactedAt;
    private string reaction = "None";
    private bool reminderActive;
    private bool isResponding;

    public int Id { get; set; }
    public int? LessonId { get; set; }
    public string Type { get; set; } = "";
    public string Title { get; set; } = "";
    public string Text { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ReadAt
    {
        get => readAt;
        set
        {
            if (readAt == value)
            {
                return;
            }
            readAt = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsUnread));
            OnPropertyChanged(nameof(CardColor));
            OnPropertyChanged(nameof(TitleFontAttributes));
        }
    }
    public DateTimeOffset? ReactedAt
    {
        get => reactedAt;
        set { if (reactedAt != value) { reactedAt = value; OnPropertyChanged(); } }
    }
    public string Reaction
    {
        get => reaction;
        set
        {
            var normalized = string.IsNullOrWhiteSpace(value) ? "None" : value;
            if (reaction == normalized)
            {
                return;
            }
            reaction = normalized;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsAwaitingReaction));
            OnPropertyChanged(nameof(HasReaction));
            OnPropertyChanged(nameof(ReactionText));
            OnPropertyChanged(nameof(CardColor));
        }
    }
    public NotificationLessonInfo? Lesson { get; set; }

    public bool IsUnread => ReadAt is null;
    public bool IsAwaitingReaction => Reaction.Equals("None", StringComparison.OrdinalIgnoreCase);
    public bool HasReaction => !IsAwaitingReaction;
    public bool ReminderActive
    {
        get => reminderActive;
        set { if (reminderActive != value) { reminderActive = value; OnPropertyChanged(); OnPropertyChanged(nameof(CardColor)); } }
    }
    public bool IsResponding
    {
        get => isResponding;
        set { if (isResponding != value) { isResponding = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanRespond)); } }
    }
    public bool CanRespond => !IsResponding;
    public Color CardColor => Color.FromArgb(ReminderActive ? "#FFF0E0" : IsUnread ? "#EAF3FF" : "#FFFFFF");
    public FontAttributes TitleFontAttributes => IsUnread ? FontAttributes.Bold : FontAttributes.None;
    public string ReactionText => Reaction.Equals("Confirmed", StringComparison.OrdinalIgnoreCase)
        ? "Odpověď: Mám zájem"
        : Reaction.Equals("Declined", StringComparison.OrdinalIgnoreCase) ? "Odpověď: Nemám zájem" : "";
    public string CreatedAtText => CreatedAt.LocalDateTime.ToString("d. M. yyyy HH:mm", CultureInfo.GetCultureInfo("cs-CZ"));
    public bool HasLesson => Lesson is not null;
    public string LessonText => Lesson is null
        ? ""
        : $"{Lesson.Name} · {Lesson.StartsAt.LocalDateTime.ToString("d. M. yyyy HH:mm", CultureInfo.GetCultureInfo("cs-CZ"))}";
}

public sealed class NotificationLessonInfo
{
    public int Id { get; set; }
    [JsonPropertyName("nazev")]
    public string Name { get; set; } = "";
    public DateTimeOffset StartsAt { get; set; }
}
