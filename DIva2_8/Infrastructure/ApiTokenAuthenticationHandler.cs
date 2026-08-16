using System.Security.Claims;
using System.Text.Encodings.Web;
using Diva2.Data.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Diva2Web.Infrastructure;

public sealed class ApiTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthenticationScheme = "Diva2Api";
    private readonly IApiTokenService tokenService;
    private readonly IDomainService domainService;

    public ApiTokenAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IApiTokenService tokenService,
        IDomainService domainService) : base(options, logger, encoder)
    {
        this.tokenService = tokenService;
        this.domainService = domainService;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorization = Request.Headers.Authorization.ToString();
        if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var payload = tokenService.Validate(authorization["Bearer ".Length..].Trim());
        if (payload == null || !string.Equals(payload.TenantId, domainService.Domain.name, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.Fail("Neplatný nebo expirovaný token."));
        }

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, payload.UserId.ToString()),
            new Claim("tenant", payload.TenantId)
        }, AuthenticationScheme);

        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), AuthenticationScheme);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
