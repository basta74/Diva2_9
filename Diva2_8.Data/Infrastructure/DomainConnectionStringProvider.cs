namespace Diva2.Data.Infrastructure;

/// <summary>
/// Builds a connection string from the domain selected for the current request.
/// </summary>
public sealed class DomainConnectionStringProvider : IConnectionStringProvider
{
    private readonly IDomainService domainService;

    public DomainConnectionStringProvider(IDomainService domainService)
    {
        this.domainService = domainService;
    }

    public string GetConnectionString()
    {
        var domain = domainService.Domain
            ?? throw new InvalidOperationException("Pro aktuální host není nakonfigurovaná doména, nelze sestavit connection string.");

        return $"server=localhost;database={domain.db};user={domain.user};password={domain.pass};";
    }
}
