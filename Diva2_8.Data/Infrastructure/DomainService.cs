using Diva2.Core.Main.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            // Get host from request or header if empty
            var host = accessor.HttpContext.Request.Host.Host ?? accessor.HttpContext.Request.Headers[HeaderNames.Host];
            Domain = FindDomain(host.ToString());
        }

        /// <summary>
        /// Load doamins from config
        /// </summary>
        /// <param name="configuration"></param>
        private void LoadDomains(IConfiguration configuration)
        {
            domains = new List<SubDomain>();
            
            /*
            domains = (IList<SubDomain>)configuration
                .GetSection("DomainSettings:Domains")
                .GetChildren()
                .Select(x => x.Value)
                .ToList(); */
        }

        /// <summary>
        /// Find domain by name
        /// </summary>
        /// <param name="host">Host name</param>
        /// <returns>Subdomain configuration</returns>
        private SubDomain FindDomain(string host)
        {
            // check full domain name eg. yoga.diva2.cz, its prefered
            var domain = domains.FirstOrDefault(t => t.name.ToLower() == host.ToLower());
            if (domain == null)
            {
                // trim domain on subdomain
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
