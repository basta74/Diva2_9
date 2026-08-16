using System;

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
}
