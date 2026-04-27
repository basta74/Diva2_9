using Diva2.Core.Main.Platby;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Platby
{
    public class PlatbaKreditCiselnikModel : PlatbaBaseModel
    {

        [Microsoft.AspNetCore.Mvc.HiddenInput]
        public int PokladnaId { get; set; }

        [Microsoft.AspNetCore.Mvc.HiddenInput]
        public int Kategorie { get; set; }

        [Range(1, 9999)]
        [Display(Name = "Počet kreditů")]
        public int Kredity { get; set; }

        public int AutorID { get; set; }

        public void CopyFromDb(PlatbaKredit o)
        {

            Id = o.Id;
            PokladnaId = o.PokladnaId;
            Kategorie = o.Kategorie;
            Castka = o.Castka;
            Kredity = o.Kredity;
            Popis = o.Popis;
            Poradi = o.Poradi;
            Platnost = o.Platnost;
            Visible = o.Visible;
            AutorID = o.AutorID;
            Slevnena = o.Kategorie == 2;
        }

        public void CopyToDb(PlatbaKredit o)
        {
            o.Id = Id;
            o.PokladnaId = PokladnaId;
            o.Kategorie = Slevnena ? 2 : 1;
            o.Castka = Castka;
            o.Kredity = Kredity;
            o.Popis = Popis;
            o.Poradi = Poradi;
            o.Platnost = Platnost;
            o.Visible = Visible;
            o.AutorID = AutorID;
        }
    }
}
