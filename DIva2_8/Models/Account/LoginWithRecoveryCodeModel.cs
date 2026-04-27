using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Account
{
    public class LoginWithRecoveryCodeModel
    {
        public string ReturnUrl { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Obnovovací kód")]
        public string RecoveryCode { get; set; }


    }
}
