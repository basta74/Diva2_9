namespace Diva2Maui.Models;
public sealed class LoginRequest { public string Email { get; set; } = ""; public string Password { get; set; } = ""; }
public sealed class LoginResponse { public string AccessToken { get; set; } = ""; public DateTimeOffset ExpiresAt { get; set; } public int UserId { get; set; } public string DisplayName { get; set; } = ""; }
