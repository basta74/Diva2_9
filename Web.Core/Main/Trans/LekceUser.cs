using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Diva2.Core.Main.Trans
{

    [DebuggerDisplay("{PobockaId} {LekceId} {UserId} {Poradi} {KontCislo}")]
    public class LekceUser : BaseEntity
    {
        public int LekceId { get; set; }
        public int UserId { get; set; }
        public int PobockaId { get; set; }
        public int PokladnaId { get; set; }
        public int Poradi { get; set; }
        public int KontCislo { get; set; }
        public bool Aktivni { get; set; }
        public bool BylTam { get; set; }
        public bool Nahradnik { get; set; }

        public bool NahradnikJa { get; set; }

        public bool Premiera { get; set; }
        public string ZbytekTyp { get; set; }
        public int Zbytek { get; set; }
        public int ZbyvaDni { get; set; }
        public bool DoMzdy { get; set; }
        public int Unix { get; set; }
        public DateTime Datum { get; set; }
        
        public Lekce Lekce { get; set; }
        public User8 User { get; set; }

    }
}
