using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Diva2.Core.Extensions;
using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Trans;
using Diva2.Core.Main.Users;
using Diva2.Core.Model.Json;
using Diva2Web.Models.Responses;
using Microsoft.AspNetCore.Authentication;

namespace Diva2Web.Models.Users
{
    public class UserModel
    {
        public UserModel()
        {

        }
        public UserModel(User8 user)
        {
            CopyFromDb(user);
        }

        public int? Id { get; set; }

        public string Nick { get; set; }

        public string Heslo { get; set; }

        public bool MayCheaper { get; set; }

        [Required]
        [Display(Name = "Jméno")]
        public string Jmeno { get; set; }

        [Required]
        [Display(Name = "Příjmení")]
        public string Prijmeni { get; set; }

        public string CeleJmeno { get; set; }

        [Required]
        [EmailAddress]

        public string Email { get; set; }


        public string Telefon { get; set; }

        public string Ulice { get; set; }

        [Display(Name = "Psč")]
        public string Psc { get; set; }

        [Display(Name = "Pošta")]
        public string Posta { get; set; }

        public DateTime? LastLogin { get; set; }

        public List<string> Roles { get; set; } = new List<string>();

        public List<string> Rules { get; set; } = new List<string>();

        public IList<LekceUser> _objednaneLekce { get; set; }
        public IList<LekceUser> ObjednaneLekce
        {
            get { return _objednaneLekce; }
            set
            {
                _objednaneLekce = value;

                foreach (var lu in _objednaneLekce)
                {
                    ObjednaneKredity += lu.Lekce.Kredit;
                }


            }
        }

        public IList<UserTransakce> HistorieLekci { get; set; }

        public IEnumerable<int> HistorieLekciYears { get; set; }
        /// <summary>
        /// Celkovy pocet objednavek
        /// </summary>
        public int PocetObjednavek { get; set; }

        public int ObjednaneKredity { get; private set; } = 0;
        public UserZbytek Zbytek { get; internal set; }

        public bool HasRule(string key)
        {
            return Rules.Contains(key);
        }

        public void CopyToDb(User8 u)
        {

            u.Email = Email;

            u.Jmeno = Jmeno;
            u.Prijmeni = Prijmeni;
            u.PhoneNumber = Telefon;
            u.Ulice = Ulice;
            u.Psc = Psc;
            u.Posta = Posta;

            u.GdprBase = GdprBase;
            u.GdprBaseDT = GdprBaseDT;
            u.GdprNews = GdprNews;
            u.GdprNewsDT = GdprNewsDT;
        }

        public void CopyFromDb(User8 u)
        {

            Id = u.Id;
            Nick = u.UserName;
            Email = u.Email;
            CeleJmeno = $"{u.Prijmeni} {u.Jmeno}";
            Jmeno = u.Jmeno;
            Prijmeni = u.Prijmeni;
            Telefon = u.PhoneNumber;
            if (u.LastLogin.HasValue)
            {
                LastLogin = DateTimeExtensions.FromUnix(u.LastLogin.Value);
            }
            Ulice = u.Ulice;
            Psc = u.Psc;
            Posta = u.Posta;

            GdprBase = u.GdprBase;
            GdprBaseDT = u.GdprBaseDT;
            GdprNews = u.GdprNews;
            GdprNewsDT = u.GdprNewsDT;

            MayCheaper = u.MayCheaper;
        }

        public bool? GdprBase { get; set; }

        public DateTime? GdprBaseDT { get; set; }

        public bool? GdprNews { get; set; }

        public DateTime? GdprNewsDT { get; set; }

    }

    public class UserModelLogin : JsonStatus
    {

        [Required]
        [Display(Name = "Zadejte email")]
        [EmailAddress]
        public string Nick { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Heslo { get; set; }

        public string? ReturnUrl { get;  set; }
        
        public List<AuthenticationScheme>? Providers { get;  set; }
    }

    public class UserModelRegister
    {
        [Required]
        [Display(Name = "Jméno")]
        public string Jmeno { get; set; }

        [Required]
        [Display(Name = "Příjmení")]
        public string Prijmeni { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Heslo {0} musí být  min {2} a max {1} znaky.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string NewPass { get; set; }


        [Required]
        [StringLength(100, ErrorMessage = "Heslo {0} musí být  min {2} a max {1} znaky.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Potvrzení hesla")]
        [Compare("NewPass", ErrorMessage = "Hesla se neshoduji")]
        public string NewPass2 { get; set; }


        [Display(Name = "Telefon")]
        [Phone]
        public string Telefon { get; set; }

        [Display(Name = "Ulice / Obec")]
        public string Ulice { get; set; }

        [Display(Name = "Město / Pošta")]
        public string Posta { get; set; }
    }

    public class UserModelPassword
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Původní heslo")]
        public string OldPass { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Heslo {0} musí být  min {2} a max {1} znaky.", MinimumLength = 6)]
        [Display(Name = "Nové Heslo")]
        public string NewPass { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Heslo {0} musí být  min {2} a max {1} znaky.", MinimumLength = 6)]
        [Display(Name = "Potvzení hesla")]
        [Compare("NewPass", ErrorMessage ="Hesla se neshoduji")]
        public string NewPass2 { get; set; }
    }

    public class UserModelUpdateForgotPassword
    {
        public int Id { get; set; }

        public JsonStatus JsonStatus { get; set; }


        [Required]
        [StringLength(100, ErrorMessage = "Heslo {0} musí být  min {2} a max {1} znaky.", MinimumLength = 6)]
        [Display(Name = "Nové Heslo")]
        public string NewPass { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Heslo {0} musí být  min {2} a max {1} znaky.", MinimumLength = 6)]
        [Display(Name = "Potvzení hesla")]
        public string NewPass2 { get; set; }
    }

    public class ChangePasswordModel 
    {
        public JsonStatus JsonStatus { get; set; }

        public int UserId { get; set; }

        public int KontCislo1 { get; set; }

        public int KontCislo2 { get; set; }

        public string Heslo { get; set; }
    }

    public class UserResetPasswordModel
    {
        public string Token { get; set; }

        public string Email { get; set; }

        public JsonStatus JsonStatus { get; set; }


        [Required]
        [StringLength(100, ErrorMessage = "Heslo {0} musí být  min {2} a max {1} znaky.", MinimumLength = 6)]
        [Display(Name = "Nové Heslo")]
        public string NewPass { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Heslo {0} musí být  min {2} a max {1} znaky.", MinimumLength = 6)]
        [Display(Name = "Potvzení hesla")]
        public string NewPass2 { get; set; }
    }

    public class UserConfirmEmailModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public JsonStatus JsonStatus { get; set; }


    }

}

