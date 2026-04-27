using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage ="{0} musí být zadáno")]
        [Display(Name ="Jméno")]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0} musí být zadáno")]
        [Display(Name = "Heslo")]
        public string Password { get; set; }
        [Display(Name = "pamatovat si")]
        public bool RememberMe { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }
    }
}
