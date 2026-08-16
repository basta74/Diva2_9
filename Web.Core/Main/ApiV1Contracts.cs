using System;
using System.Collections.Generic;

namespace Diva2.Core.Api.V1;

public sealed class ApiLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class ApiLoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public int UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}

public sealed class ApiBranchInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public sealed class ApiLessonInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Lector { get; set; } = string.Empty;
    public DateTimeOffset StartsAt { get; set; }
    public int Capacity { get; set; }
    public int Occupied { get; set; }
    public bool IsReservedByCurrentUser { get; set; }
    public string ReservationStatus { get; set; }
    public int LessonTypeId { get; set; }
}

public sealed class ApiLessonParticipant
{
    public string DisplayName { get; set; } = string.Empty;
    public bool IsWaitingList { get; set; }
    public bool IsCurrentUser { get; set; }
}

public sealed class ApiMyReservation
{
    public int LessonId { get; set; }
    public int BranchId { get; set; }
    public string LessonName { get; set; } = string.Empty;
    public DateTimeOffset StartsAt { get; set; }
    public string ReservationStatus { get; set; } = string.Empty;
}

public sealed class ApiCreateReservationResponse
{
    public bool Success { get; set; }
    public string ReservationStatus { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public sealed class ApiBranchCredits
{
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public bool ShowUnlimitedCredits { get; set; }
    public int UnlimitedCredits { get; set; }
    public bool ShowTimeCredits { get; set; }
    public IReadOnlyList<ApiTimeCredit> TimeCredits { get; set; } = Array.Empty<ApiTimeCredit>();
}

public sealed class ApiTimeCredit
{
    public int Credits { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset? ValidFrom { get; set; }
    public DateTimeOffset? ValidTo { get; set; }
}

public sealed class ApiBranchConfiguration
{
    public int BranchId { get; set; }
    public IReadOnlyDictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
    public IReadOnlyList<ApiLessonTypeInfo> LessonTypes { get; set; } = Array.Empty<ApiLessonTypeInfo>();
}

public sealed class ApiLessonTypeInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
