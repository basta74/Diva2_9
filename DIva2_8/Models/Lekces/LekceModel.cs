using Diva2.Core.Main.Lessons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Lekces
{
    public class LekceModel 
    {
        public int Id { get; set; }

        public int Verze { get; set; }

        public int MustrId { get; set; }

        public int PobockaId { get; set; }

        public int Zdroj { get; set; }

        public int Rok { get; set; }

        public int Tyden { get; set; }

        public int Den { get; set; }

        public DateTime Datum { get; set; }

        public int? HodinaPoradi { get; set; }

        public int MinutaKey { get; set; }

        [Display(Name = "Lekce na celý den")]
        public bool CelyDen { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Žačátek")]
        [Required]
        public TimeSpan Cas { get; set; }

        public string Nazev { get; set; }
        
        [Range(0, Int32.MaxValue)]
        [Display(Name = "Počet kreditů")]
        public int Kredit { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "Počet minut")]
        public int Minuty { get; set; }

        public int PocetJednotek { get; set; }

        public int Lektor1 { get; set; }
        
        public List<ListItem> Lektori { get; set; } = new List<ListItem>();

        public int Lektor2 { get; set; }

        public int TypHodiny { get; set; }
        public List<ListItem> Typy { get; set; } = new List<ListItem>();

        [Range(0, Int32.MaxValue)]
        [Display(Name = "Počet míst")]
        public int PocetMist { get; set; }

        public int PocetZakazniku { get; set; }
        
        [Range(0, Int32.MaxValue)]
        [Display(Name = "Počet náhradníků")]
        public int PocetNahradniku { get; set; }

        public bool Zauctovano { get; set; }

        public DateTime? ZauctovanoDate { get; set; }

        public int? ZauctovanoUserId { get; set; }

        public string? ZauctovanoUserName { get; set; }

        public int PocetPrihlasenych { get; set; }

        internal void CopyFromDb(Lekce o)
        {

            Id = o.Id;
            Verze = o.Verze;
            MustrId = o.MustrId;
            PobockaId = o.PobockaId;
            Zdroj = o.Zdroj;
            Rok = o.Rok;
            Tyden = o.Tyden;
            Den = o.Den;
            Datum = o.Datum;
            HodinaPoradi = o.HodinaPoradi;
            MinutaKey = o.MinutaKey;
            CelyDen = o.CelyDen;
            Cas = o.Cas;
            Nazev = o.Nazev;
            Kredit = o.Kredit;
            Minuty = o.Minuty;
            PocetJednotek = o.PocetJednotek;
            Lektor1 = o.Lektor1;
            Lektor2 = o.Lektor2;
            TypHodiny = o.TypHodiny;
            PocetMist = o.PocetMist;
            PocetZakazniku = o.PocetZakazniku;
            PocetNahradniku = o.PocetNahradniku;
            Zauctovano = o.Zauctovano;
            ZauctovanoDate = o.ZauctovanoDate;
            ZauctovanoUserId = o.ZauctovalUserId;
        }

        internal void CopyToDb(Lekce o)
        {

            o.Id = Id;
            o.Verze = Verze;
            o.MustrId = MustrId;
            o.PobockaId = PobockaId;
            o.Zdroj = Zdroj;
            o.Rok = Rok;
            o.Tyden = Tyden;
            o.Den = Den;
            o.Datum = Datum;
            o.HodinaPoradi = HodinaPoradi;
            o.Cas = Cas;
            o.Nazev = Nazev;
            o.Kredit = Kredit;
            o.Minuty = Minuty;
            o.MinutaKey = MinutaKey;
            o.CelyDen = CelyDen;
            o.PocetJednotek = PocetJednotek;
            o.Lektor1 = Lektor1;
            o.Lektor2 = Lektor2;
            o.TypHodiny = TypHodiny;
            o.PocetMist = PocetMist;
            o.PocetZakazniku = PocetZakazniku;
            o.PocetNahradniku = PocetNahradniku;
            o.Zauctovano = Zauctovano;
            o.ZauctovanoDate = ZauctovanoDate;
            o.ZauctovalUserId = ZauctovanoUserId;
        }

        internal void CopyToDbForUpdate(Lekce o)
        {

            o.Id = Id;
            /*
            o.Verze = Verze;
            o.MustrId = MustrId;
            o.PobockaId = PobockaId;
            o.Zdroj = Zdroj;
            o.Rok = Rok;
            o.Tyden = Tyden;
            o.Den = Den;
            o.Datum = Datum;
            o.HodinaPoradi = HodinaPoradi;
            /**/
            o.Cas = Cas;
            o.Nazev = Nazev;
            o.Kredit = Kredit;
            o.Minuty = Minuty;
            o.PocetJednotek = PocetJednotek;
            o.Lektor1 = Lektor1;
            o.Lektor2 = Lektor2;
            o.TypHodiny = TypHodiny;
            o.CelyDen = CelyDen;
            o.PocetMist = PocetMist;
            //o.PocetZakazniku = PocetZakazniku;
            o.PocetNahradniku = PocetNahradniku;
            /*
            o.Zauctovano = Zauctovano;
            o.ZauctovanoDate = ZauctovanoDate;
            o.ZauctovanoUserId = ZauctovanoUserId; /**/
        }

        public DateTime GetZacatek()
        {

            var dt = Datum;
            var cas = dt.Add(Cas);

            return cas;
        }
    }
}