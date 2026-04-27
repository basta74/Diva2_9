using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Diva2.Core.Main.Lessons
{
    [DebuggerDisplay("{PobockaId} {Rok}{Tyden} d:{Den} h:{HodinaPoradi} {Id}")]
    public class Lekce : BaseEntity
    {

        public int Verze { get; set; }

        public int MustrId { get; set; }

        public int PobockaId { get; set; }

        public int Zdroj { get; set; }

        public int Rok { get; set; }

        public int Tyden { get; set; }

        public int Den { get; set; }

        public DateTime Datum { get; set; }


        private DateTime _datumCas;

        public DateTime DatumCas
        {
            get
            {
                _datumCas = Datum;
                _datumCas.Add(Cas);
                return _datumCas;
            }
        }

        public int? HodinaPoradi { get; set; }

        public int MinutaKey { get; set; }

        public TimeSpan Cas { get; set; }


        public string Nazev { get; set; }

        public int Kredit { get; set; }

        public int Minuty { get; set; }

        public int PocetJednotek { get; set; }

        public int Lektor1 { get; set; }

        public int Lektor2 { get; set; }

        public int TypHodiny { get; set; }

        public int PocetMist { get; set; }

        public int PocetZakazniku { get; set; }

        public int PocetNahradniku { get; set; }

        public bool Zauctovano { get; set; }

        public int? ZauctovalUserId { get; set; }

        public string ZauctovalUserName { get; set; }

        public DateTime? ZauctovanoDate { get; set; }

        public LekceRozvrhTable ForTable { get; set; }
        public bool CelyDen { get; set; }

        public void CopyFromMustr(LekceMustr le)
        {
            //Verze = le.ve
            MustrId = le.Id;
            PobockaId = le.PobockaId;
            Zdroj = le.Zdroj;
            //Rok = le
            //Tyden = le
            //Datum = le.Hodina;
            Den = le.Den;
            HodinaPoradi = le.Hodina;
            MinutaKey = le.MinutaKey;
            Cas = TimeSpan.Parse(le.Cas.ToString());
            Nazev = le.Nazev;
            Kredit = le.Kredit;
            Minuty = le.Minuty;
            PocetJednotek = 1;
            Lektor1 = le.Lektor;
            Lektor2 = le.Lektor2;
            TypHodiny = le.Typ;
            PocetMist = le.PocetMist;
            //PocetZakazniku = le.po
            PocetNahradniku = le.PocetNahradniku;
        }

        public void CopyFromSablona(LekceMustrTyp le)
        {

            PobockaId = le.PobockaId;

            Nazev = le.Nazev;
            Kredit = le.Kredit;
            Minuty = le.Minuty;
            PocetJednotek = 1;
            Lektor1 = le.Lektor;
            Lektor2 = le.Lektor2;
            TypHodiny = le.Typ;
            PocetMist = le.PocetMist;
            //PocetZakazniku = le.po
            PocetNahradniku = le.PocetNahradniku;
        }

        public DateTime DatumHodina
        {
            get
            {
                DateTime dt = this.Datum;
                dt = dt.Add(this.Cas);
                return dt;
            }
        }
    }

    public class LekceRozvrhTable
    {

        public string Nazev { get; set; }
        public string Typ { get; set; }
        public string Lektor1 { get; set; }
        public string Lektor2 { get; set; }
        public string Zbyva { get; set; }
        public string Cas { get; set; }

    }

    public class LekceTyp : BaseEntity
    {
        public int PobockaId { get; set; }

        public string Nazev { get; set; }

        public string NazevAdmin { get; set; }

        public string Zkratka { get; set; }

        public int JednotkaPocet { get; set; }

        public int JednotkaPocetDo { get; set; }

        public int JednotkaMinut { get; set; }

        public int Kredit { get; set; }

        public int Mista { get; set; }

        public string Popis { get; set; }


    }
}
