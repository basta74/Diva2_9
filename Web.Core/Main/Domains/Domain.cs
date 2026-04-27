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

    }
}
