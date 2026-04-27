using Diva2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Diva2.Core.Main.Pobocky
{
    [DebuggerDisplay("{PobockaId} {Name}-{Value}")]
    public class PobockaIni : BaseEntity
    {
        public int PobockaId { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string TypeString { get; set; }

        public PobockaIniType Type { get; set; }

        public string Default { get; set; }

        public string Desc { get; set; }
    }

    public enum PobockaIniType { String, Int, Bolean,
        Time
    }
}
