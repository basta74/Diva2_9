using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Admin
{
    public class SkupinaZakaznikaModel
    {
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


        public void CopyFromDb(User8Group o)
        {
            PobockaId = o.PobockaId;
            Nazev = o.Nazev;
            Popis = o.Popis;
        }
        public void CopyToDb(User8Group o)
        {
            o.PobockaId = PobockaId;
            o.Nazev = Nazev;
            o.Popis = Popis;
        }
    }
}
