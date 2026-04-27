using Diva2.Core;
using Diva2.Core.Main.Trans;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Users
{
    public class User8 : IdentityUser<int>
    {
        public string Nazev { get; set; }

        public string Titul { get; set; }

        public string Jmeno { get; set; }

        public string Prijmeni { get; set; }

        public string Ulice { get; set; }

        public string Psc { get; set; }

        public string Posta { get; set; }

        public bool Platnost { get; set; }

        /// <summary>
        /// muze mit levnejsi
        /// </summary>
        public bool MayCheaper { get; set; }

        public int? LastLogin { get; set; }

        public bool Deleted { get; set; }

        public DateTime? DeletedDt { get; set; }

        public int? DeletedByUserId { get; set; }

        public int InternalNumber { get; set; }

        public string Poznamka { get; set; }

        public string Poznamka2 { get; set; }

        // public virtual IList<UserRoles8> Roles { get; set; } = new List<UserRoles8>();

        //public  IList<LekceUser> Lekce { get; set; } = new List<LekceUser>();

        /// <summary>
        /// Nick
        /// </summary>
       /* public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }*/



        public bool? GdprBase { get; set; }

        public DateTime? GdprBaseDT { get; set; }

        public bool? GdprNews { get; set; }

        public DateTime? GdprNewsDT { get; set; }

        public virtual ICollection<UserZbytekKredit> Kredity { get; set; }

        public virtual ICollection<UserZbytekKreditCas> KredityCas { get; set; }
        /**/

    }
}
