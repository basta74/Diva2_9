using Diva2.Core.Main.Domains;
using Diva2.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Diva2Web.Infrastructure;

public sealed class FixedDomainService : IDomainService
{
    public FixedDomainService(SubDomain domain) => Domain = domain;
    public SubDomain Domain { get; }
}

public static class TenantDatabaseContextFactory
{
    public static ApplicationDbContext Create(SubDomain tenant)
    {
        var connectionString = $"server=localhost;database={tenant.db};user={tenant.user};password={tenant.pass};";
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;
        return new ApplicationDbContext(options, new FixedDomainService(tenant));
    }

    public static IReadOnlyList<SubDomain> GetUniqueDatabases(IConfiguration configuration)
    {
        var domains = new List<SubDomain>();
        configuration.GetSection("Domains").Bind(domains);
        return domains.Where(x => !string.IsNullOrWhiteSpace(x.db))
            .GroupBy(x => x.db, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.First())
            .ToList();
    }
}
