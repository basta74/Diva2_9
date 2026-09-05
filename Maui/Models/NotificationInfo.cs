using System.Globalization;
using System.Text.Json.Serialization;

namespace Diva2Maui.Models;

public sealed class NotificationInfo
{
    public int Id { get; set; }
    public int? LessonId { get; set; }
    public string Type { get; set; } = "";
    public string Title { get; set; } = "";
    public string Text { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ReadAt { get; set; }
    public NotificationLessonInfo? Lesson { get; set; }

    public bool IsUnread => ReadAt is null;
    public Color CardColor => Color.FromArgb(IsUnread ? "#EAF3FF" : "#FFFFFF");
    public FontAttributes TitleFontAttributes => IsUnread ? FontAttributes.Bold : FontAttributes.None;
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
