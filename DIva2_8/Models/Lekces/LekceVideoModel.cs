using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Lekces
{
    public class LekceVideoModel
    {
        public int Id { get; set; }

        [Display(Name = "Url  - pokud url smažete, smaže se celý záznam")]
        [StringLength(150, ErrorMessage = "Maximální délka je 150 znaků")]
        public string Url { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Maximální délka je 150 znaků")]
        public string TextUrl { get; set; }

        [StringLength(255, ErrorMessage = "Maximální délka je 255 znaků")]
        public string Text { get; set; }

        public DateTime Dt { get; set; }
    }

    public class LekceTextModel
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime Dt { get; set; }
    }
}
