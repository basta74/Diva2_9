using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Domains
{
    public class DomainSettings
    {
        public List<SubDomain> Domains { get; set; }
    }

    public class SubDomain
    {
        public string name { get; set; }
        public string db { get; set; }
        public string user { get; set; }
        public string pass { get; set; }

        /// <summary>
        /// Optional public name shown in client applications.
        /// </summary>
        public string publicName { get; set; }

        /// <summary>
        /// Allows hiding a tenant from the public catalog without disabling the web.
        /// </summary>
        public bool? publicEnabled { get; set; }

    }

    public class TenantPublicInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string BaseUrl { get; set; }
    }
}
