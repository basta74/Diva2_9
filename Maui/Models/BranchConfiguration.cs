namespace Diva2Maui.Models;

public sealed class BranchConfiguration
{
    public int BranchId { get; set; }
    public Dictionary<string, string> Settings { get; set; } = [];
    public List<LessonTypeInfo> LessonTypes { get; set; } = [];

    public bool IsEnabled(string key, bool defaultValue = false)
    {
        var value = Settings.FirstOrDefault(item => string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase)).Value;
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value == "1" || bool.TryParse(value, out var enabled) && enabled;
    }

    public int GetInt(string key, int defaultValue = 0)
    {
        var value = Settings.FirstOrDefault(item => string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase)).Value;
        return int.TryParse(value, out var result) ? result : defaultValue;
    }
}

public sealed class LessonTypeInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Abbreviation { get; set; } = "";
    public string Description { get; set; } = "";
}
