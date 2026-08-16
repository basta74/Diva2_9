namespace Diva2.Data.Infrastructure;

/// <summary>
/// Provides the database connection string for the current application scope.
/// </summary>
public interface IConnectionStringProvider
{
    string GetConnectionString();
}
