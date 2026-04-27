using Diva2.Core.Main.Lessons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Lekces
{
    public class LekceMustrModel
    {
        public int Id { get; set; }
        public bool Aktivni { get; set; }
        public int PobockaId { get; set; }
        public int Zdroj { get; set; }
        public int Den { get; set; }
        public int Hodina { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Žačátek")]
        [Required]
        public string Cas { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Název")]
        public string Nazev { get; set; }


        [Display(Name = "Počet kreditů")]
        public int Kredit { get; set; }

        public int MinutaKey { get; set; }

        [Display(Name = "Počet minut")]
        public int Minuty { get; set; }

        public int Lektor { get; set; }
        
        [Display(Name = "Lektor")]
        public List<ListItem> Lektori { get; set; } = new List<ListItem>();
        public string LektorString { get; set; }
        public int Lektor2 { get; set; }
        public string Lektor2String { get; set; }

        [Display(Name = "Typ")]
        public List<ListItem> Typy { get; set; } = new List<ListItem>();

        [Display(Name = "Typ")]
        public int Typ { get; set; }
        public string TypString { get; set; }

        [Display(Name = "Počet míst")]
        public int PocetMist { get; set; }

        [Display(Name = "Počet náhradníků")]
        public int PocetNahradniku { get; set; }


        public void CopyFromDb(LekceMustr o)
        {
            Id = o.Id;
            Aktivni = o.Aktivni;
            PobockaId = o.PobockaId;
            Zdroj = o.Zdroj;
            Den = o.Den;
            Hodina = o.Hodina;
            Cas = o.Cas;
            Nazev = o.Nazev;
            Kredit = o.Kredit;
            Minuty = o.Minuty;
            MinutaKey = o.MinutaKey;
            Lektor = o.Lektor;
            Lektor2 = o.Lektor2;
            Typ = o.Typ;
            PocetMist = o.PocetMist;
            PocetNahradniku = o.PocetNahradniku;
        }

        public void CopyToDb(LekceMustr o)
        {
            Id = o.Id;
            o.Aktivni = Aktivni;
            /*
            o.PobockaId = PobockaId;
            o.Zdroj = Zdroj;
            o.Den = Den;
            o.Hodina = Hodina; /**/
            o.Cas = Cas;
            o.Nazev = Nazev;
            o.Kredit = Kredit;
            o.Minuty = Minuty;

            o.Lektor = Lektor;
            o.Lektor2 = Lektor2;
            o.Typ = Typ;
            o.PocetMist = PocetMist;
            o.PocetNahradniku = PocetNahradniku;
        }

    }
}
