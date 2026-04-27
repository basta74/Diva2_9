using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Account
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Uživatelké jméno")]
        public string UserName { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Heslo {0} musí být  min {2} a max {1} znaky.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Potvrdit heslo")]
        [Compare("Password", ErrorMessage = "Hesla musí být shodná")]
        public string ConfirmPassword { get; set; }

    }
}
