using Diva2.Core.Main.Platby;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Platby
{
    public class PlatbaKreditCasCiselnikModel : PlatbaBaseModel
    {

        [Microsoft.AspNetCore.Mvc.HiddenInput]
        public int PokladnaId { get; set; }

        [Microsoft.AspNetCore.Mvc.HiddenInput]
        public int Kategorie { get; set; }
        
        [Range(10, Int32.MaxValue)]
        [Display(Name = "Kredity min. 10")]
        public int Kredity { get; set; }


        [Range(1, 12)]
        [Display(Name = "Počet měsíců")]
        public int PocetMesicu { get; set; }

        [Range(1, Int32.MaxValue)]
        [Display(Name = "Počet lidí")]
        public int PocetLidi { get; set; }
                       
        public int AutorID { get; set; }

        public void CopyFromDb(PlatbaKreditCas o) {
            Id = o.Id;
            PokladnaId = o.PokladnaId;
            Kategorie = o.Kategorie;
            Castka = o.Castka;
            Kredity = o.Kredity;
            PocetMesicu = o.PocetMesicu;
            PocetLidi = o.PocetLidi;
            Popis = o.Popis;
            Poradi = o.Poradi;
            Platnost = o.Platnost;
            Visible = o.Visible;
            AutorID = o.AutorID;
            Slevnena = (o.Kategorie == 2);
        }

        public void CopyToDb(PlatbaKreditCas o)
        {
            o.Id = Id;
            o.PokladnaId = PokladnaId;
            o.Castka = Castka;
            o.Kredity = Kredity;
            o.PocetMesicu = PocetMesicu;
            o.PocetLidi = PocetLidi;
            o.Popis = Popis;
            o.Poradi = Poradi;
            o.Platnost = Platnost;
            o.Visible = Visible;
            o.AutorID = AutorID;
            o.Kategorie = Slevnena ? 2 : 1;
        }
    }
}
