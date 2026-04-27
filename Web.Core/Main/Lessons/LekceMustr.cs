using Diva2.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Lessons
{
    public class LekceMustr : BaseEntity
    {
        public bool Aktivni { get; set; }
        public int PobockaId { get; set; }
        public int Zdroj { get; set; }
        public int Den { get; set; }
        public int Hodina { get; set; }
        public string Cas { get; set; }

        public int MinutaKey { get; set; }
        public string Nazev { get; set; }
        public int Kredit { get; set; }
        public int Minuty { get; set; }
        public int Lektor { get; set; }
        public int Lektor2 { get; set; }
        public int Typ { get; set; }
        public int PocetMist { get; set; }
        public int PocetNahradniku { get; set; }


        public void CopyFromSablona(LekceMustrTyp le)
        {

            Nazev = le.Nazev;
            Kredit = le.Kredit;
            Minuty = le.Minuty;
            Lektor = le.Lektor;
            Lektor2 = le.Lektor2;
            Typ = le.Typ;
            PocetMist = le.PocetMist;
            PocetNahradniku = le.PocetNahradniku;
            Aktivni = le.Aktivni;
        }

    }

    public class LekceMustrTyp : BaseEntity
    {

        public string NazevTyp { get; set; }
        public int PobockaId { get; set; }
        public bool Aktivni { get; set; }
        public string Nazev { get; set; }
        public int Kredit { get; set; }
        public int Minuty { get; set; }
        public int Lektor { get; set; }
        public int Lektor2 { get; set; }
        public int Typ { get; set; }
        public int PocetMist { get; set; }
        public int PocetNahradniku { get; set; }
       
    }
}
