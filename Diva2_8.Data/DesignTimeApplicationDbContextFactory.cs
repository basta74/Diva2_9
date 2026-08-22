using Diva2.Core.Main.Domains;
using Diva2.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Diva2.Data;

public sealed class DesignTimeApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        const string connectionString = "server=localhost;database=diva2_design;user=design;password=design;";
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)))
            .Options;
        return new ApplicationDbContext(options, new DesignTimeDomainService());
    }

    private sealed class DesignTimeDomainService : IDomainService
    {
        public SubDomain Domain { get; } = new() { name = "design", db = "diva2_design", user = "design", pass = "design" };
    }
}
