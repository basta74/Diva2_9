using Diva2.Core.Main.Domains;

namespace Diva2.Data.Infrastructure
{
    public interface IDomainService
    {
        SubDomain Domain { get; }
    }
}