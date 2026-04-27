using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Platby
{
    public class PlatbaBaseModel
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "{0} musí být zadán")]
        [DataType(DataType.Text)]
        [Display(Name = "Název")]
        public string Popis { get; set; }


        [Display(Name = "Částka")]
        public int Castka { get; set; }

        [Range(1, 9999)]
        [Display(Name = "Pořadí")]
        public int Poradi { get; set; }

        [Display(Name = "Použít v systému")]
        public bool Platnost { get; set; }

        [Display(Name = "Viditelné na webu")]
        public bool Visible { get; set; }

        [Display(Name = "Je slevněná (zobrazí se ve slevách)")]
        public bool Slevnena { get; set; }

    }
}
