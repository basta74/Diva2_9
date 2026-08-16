using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;

namespace Diva2Web.Infrastructure;

public interface IApiTokenService
{
    string Create(int userId, string tenantId, DateTimeOffset expiresAt);
    ApiTokenPayload? Validate(string token);
}

public sealed class ApiTokenPayload
{
    public int UserId { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
}

public sealed class ApiTokenService : IApiTokenService
{
    private readonly IDataProtector protector;

    public ApiTokenService(IDataProtectionProvider provider)
    {
        protector = provider.CreateProtector("Diva2.Api.V1.AccessToken");
    }

    public string Create(int userId, string tenantId, DateTimeOffset expiresAt)
    {
        return protector.Protect(JsonSerializer.Serialize(new ApiTokenPayload
        {
            UserId = userId,
            TenantId = tenantId,
            ExpiresAt = expiresAt
        }));
    }

    public ApiTokenPayload? Validate(string token)
    {
        try
        {
            var payload = JsonSerializer.Deserialize<ApiTokenPayload>(protector.Unprotect(token));
            return payload?.ExpiresAt > DateTimeOffset.UtcNow ? payload : null;
        }
        catch
        {
            return null;
        }
    }
}
