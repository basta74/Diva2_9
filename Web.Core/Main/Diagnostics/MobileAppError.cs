using System;

namespace Diva2.Core.Main.Diagnostics;

public sealed class MobileAppError : BaseEntity
{
    public int UserId { get; set; }
    public int? LessonId { get; set; }
    public DateTime OccurredAt { get; set; }
    public DateTime ReceivedAt { get; set; }
    public string Platform { get; set; } = string.Empty;
    public string OsVersion { get; set; } = string.Empty;
    public string DeviceManufacturer { get; set; } = string.Empty;
    public string DeviceModel { get; set; } = string.Empty;
    public string AppVersion { get; set; } = string.Empty;
    public string AppBuild { get; set; } = string.Empty;
    public string Page { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string ExceptionType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public string InnerException { get; set; } = string.Empty;
}
