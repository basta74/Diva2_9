using Diva2.Data.Infrastructure;
using Diva2Web.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diva2Web.Controllers.Api.V1;

[ApiController]
[Authorize(AuthenticationSchemes = ApiTokenAuthenticationHandler.AuthenticationScheme)]
[Route("api/v1/changes")]
public sealed class ChangesController(IDomainService domainService, IConfiguration configuration) : ControllerBase
{
    [HttpGet("all")]
    [AllowAnonymous]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public IActionResult GetAll()
    {
        var changes = configuration.GetSection("Domains").GetChildren()
            .Select(domain => domain["name"])
            .Where(name => !string.IsNullOrWhiteSpace(name)
                && !string.Equals(name, "localhost", StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(name => new { timestamp = DomainChangeStamp.Get(name!), subdomain = name })
            .OrderByDescending(change => change.timestamp, StringComparer.Ordinal)
            .ThenBy(change => change.subdomain, StringComparer.OrdinalIgnoreCase)
            .ToList();
        return Ok(changes);
    }

    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public IActionResult Get()
    {
        var domain = domainService.Domain?.name;
        if (string.IsNullOrWhiteSpace(domain))
        {
            return NotFound();
        }
        return Ok(new { timestamp = DomainChangeStamp.Get(domain) });
    }
}
