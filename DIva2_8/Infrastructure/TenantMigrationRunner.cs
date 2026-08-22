using Microsoft.EntityFrameworkCore;

namespace Diva2Web.Infrastructure;

public static class TenantMigrationRunner
{
    public static async Task<int> RunAsync(IConfiguration configuration, ILogger logger, CancellationToken cancellationToken = default)
    {
        var succeeded = new List<string>();
        var failed = new List<(string Database, string Error)>();
        foreach (var tenant in TenantDatabaseContextFactory.GetUniqueDatabases(configuration))
        {
            try
            {
                await using var db = TenantDatabaseContextFactory.Create(tenant);
                await db.Database.MigrateAsync(cancellationToken);
                succeeded.Add(tenant.db);
                logger.LogInformation("Migrated tenant database {Database} ({Tenant}).", tenant.db, tenant.name);
            }
            catch (Exception ex)
            {
                failed.Add((tenant.db, ex.Message));
                logger.LogError(ex, "Migration failed for tenant database {Database} ({Tenant}); continuing.", tenant.db, tenant.name);
            }
        }

        Console.WriteLine($"Successful databases ({succeeded.Count}): {string.Join(", ", succeeded)}");
        Console.WriteLine($"Failed databases ({failed.Count}): {string.Join(", ", failed.Select(x => $"{x.Database}: {x.Error}"))}");
        return failed.Count == 0 ? 0 : 1;
    }
}
