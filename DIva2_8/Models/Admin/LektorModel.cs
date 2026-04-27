using Diva2.Core.Main.Lektori;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Admin
{
    public class LektorModel
    {
        [HiddenInput]
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} musí být zadán")]
        [Display(Name = "Nick/přezdívka")]
        public string Nick { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Celé jméno")]
        [Required(ErrorMessage = "{0} musí být zadáno")]
        public string Jmeno { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Titul v rámci oblasti")]
        public string Titul { get; set; }
        
        public int Kredity { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "{0} musí být zadán")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Telefon")]
        public string Tel { get; set; }

        [DataType(DataType.MultilineText)]
        public string Popis { get; set; }

        [Display(Name = "Pořadí pro zobrazení")]
        public int Poradi { get; set; }

        public bool Platnost { get; set; }

        /*
         public bool Platnost
         {
             get { return PlatnostInt == 1; }
             set { if (Platnost == true) { PlatnostInt = 1; } else { PlatnostInt = 0; } }
         } /**/

        [Display(Name = "Zobrazit na webu")]
        public bool Viditelnost { get; set; }

        [Display(Name = "Id uživatele v systému")]
        public int UserId { get; set; }

        [Display(Name = "Koeficient pro násobení mzdy")]
        public int Koeficient { get; set; }

        public LektorModel() { 
        
        }

        public LektorModel(Lektor lek)
        {
            CopyFromDb(lek);
        }

        public void CopyFromDb(Lektor le) {
            Id = le.Id;
            Nick = le.Nick;
            Jmeno = le.Jmeno;
            Titul = le.Titul;
            Kredity = le.Kredity;
            Email = le.Email;
            Tel = le.Tel;
            Popis = le.Popis;
            Poradi = le.Poradi;
            Viditelnost = le.Viditelnost;
            Platnost = le.Platnost;
            UserId = le.UserId;
            Koeficient = le.Koeficient;
        }

        public void CopyToDb(Lektor le) {

            le.Nick = Nick;
            le.Jmeno = Jmeno;
            le.Titul = Titul;
            le.Kredity = Kredity;
            le.Email = Email;
            le.Tel = Tel;
            le.Popis = Popis;
            le.Poradi = Poradi;
            le.Viditelnost = Viditelnost;
            le.Platnost = Platnost;
            le.UserId = UserId;
            le.Koeficient = Koeficient;
        }

    }

}
