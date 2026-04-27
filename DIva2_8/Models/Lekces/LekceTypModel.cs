using Diva2.Core.Main.Lessons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Lekces
{
    public class LekceTypModel
    {
        public LekceTypModel()
        {
        }

        public LekceTypModel(LekceTyp typ)
        {
            CopyFromDb(typ);
        }

        [Microsoft.AspNetCore.Mvc.HiddenInput]
        public int Id { get; set; }

        [Microsoft.AspNetCore.Mvc.HiddenInput]
        public int PobockaId { get; set; }

        [Required(ErrorMessage = "{0} musí být zadán")]
        [DataType(DataType.Text)]
        [Display(Name = "Název")]
        public string Nazev { get; set; }

        [Required(ErrorMessage = "{0} musí být zadán")]
        [Display(Name = "Popis")]
        [DataType(DataType.MultilineText)]
        public string Popis { get; set; }


        public void CopyFromDb(LekceTyp o)
        {
            PobockaId = o.PobockaId;
            Nazev = o.Nazev;
            Popis = o.Popis;
        }
        public void CopyToDb(LekceTyp o)
        {
            o.PobockaId = PobockaId;
            o.Nazev = Nazev;
            o.Popis = Popis;
        }
    }
}
