using Diva2.Core.Main.Domains;
using Diva2.Data.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diva2Web.Controllers.Api.V1;

[ApiController]
[AllowAnonymous]
[Route("api/v1/tenants")]
public sealed class TenantsController : ControllerBase
{
    private readonly ITenantCatalog tenantCatalog;

    public TenantsController(ITenantCatalog tenantCatalog)
    {
        this.tenantCatalog = tenantCatalog;
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<TenantPublicInfo>>(StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<TenantPublicInfo>> GetAll()
    {
        return Ok(tenantCatalog.GetAll());
    }

    [HttpGet("{id}")]
    [ProducesResponseType<TenantPublicInfo>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TenantPublicInfo> GetById(string id)
    {
        var tenant = tenantCatalog.GetById(id);
        return tenant == null ? NotFound() : Ok(tenant);
    }
}
