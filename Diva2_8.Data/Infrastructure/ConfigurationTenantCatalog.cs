using Diva2.Core.Main.Domains;
using Microsoft.Extensions.Configuration;

namespace Diva2.Data.Infrastructure;

public sealed class ConfigurationTenantCatalog : ITenantCatalog
{
    private const string ProductionDomain = "diva2.cz";
    private readonly IConfiguration configuration;

    public ConfigurationTenantCatalog(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public IReadOnlyList<TenantPublicInfo> GetAll()
    {
        var domains = new List<SubDomain>();
        configuration.GetSection("Domains").Bind(domains);

        return domains
            .Where(domain => !string.IsNullOrWhiteSpace(domain.name))
            .Where(domain => !string.Equals(domain.name, "localhost", StringComparison.OrdinalIgnoreCase))
            .Where(domain => domain.publicEnabled != false)
            .Select(ToPublicInfo)
            .OrderBy(tenant => tenant.Name)
            .ToList();
    }

    public TenantPublicInfo? GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        return GetAll().FirstOrDefault(tenant =>
            string.Equals(tenant.Id, id.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    private static TenantPublicInfo ToPublicInfo(SubDomain domain)
    {
        var id = domain.name.Trim().ToLowerInvariant();

        return new TenantPublicInfo
        {
            Id = id,
            Name = string.IsNullOrWhiteSpace(domain.publicName) ? id : domain.publicName.Trim(),
            BaseUrl = $"https://{id}.{ProductionDomain}"
        };
    }
}
