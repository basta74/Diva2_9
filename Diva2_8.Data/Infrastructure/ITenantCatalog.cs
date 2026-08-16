using Diva2.Core.Main.Domains;

namespace Diva2.Data.Infrastructure;

public interface ITenantCatalog
{
    IReadOnlyList<TenantPublicInfo> GetAll();
    TenantPublicInfo? GetById(string id);
}
