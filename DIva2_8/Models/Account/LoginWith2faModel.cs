using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Account
{
    public class LoginWith2faModel
    {
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
        [Required]
        [StringLength(7, ErrorMessage = "{0} musí být minimálně {2} a maximálně {1} znaků dlouhý.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Ověřovací kód")]
        public string TwoFactorCode { get; set; }

        [Display(Name = "Pamatovat si na tomto zařízení")]
        public bool RememberMachine { get; set; }

    }
}
