using Diva2.Core.Main.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;




namespace Diva2.Data.Infrastructure
{
    public class DomainService : IDomainService
    {
        public static IList<SubDomain> domains;
        public SubDomain Domain { get; private set; }

        public DomainService(IHttpContextAccessor accessor, IConfiguration configuration)
        {
            if (domains == null)
            {
                LoadDomains(configuration);
            }

            var host = accessor.HttpContext?.Request?.Host.Host;

            Domain = FindDomain(host) ?? throw new Exception("Domain not found");
        }

        private void LoadDomains(IConfiguration configuration)
        {
            domains = new List<SubDomain>();

            configuration.GetSection("Domains").Bind(domains);
        }

        private SubDomain FindDomain(string host)
        {
            var domain = domains.FirstOrDefault(t => t.name.ToLower() == host.ToLower());

            if (domain == null)
            {
                int index = host.IndexOf('.');
                if (index > 0)
                {
                    return FindDomain(host.Substring(0, index));
                }
            }

            return domain;
        }
    }
}
