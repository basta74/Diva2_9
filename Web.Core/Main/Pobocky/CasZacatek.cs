using Diva2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Diva2.Core.Main.Pobocky
{
    [DebuggerDisplay("{PobockaId} {Poradi} {Value} {Minuta}")]
    public class CasZacatek : BaseEntity
    {
        public int PobockaId { get; set; }

        public int Poradi  { get; set; }

        public string Value { get; set; }

        public int Minuta { get; set; }


    }
}
