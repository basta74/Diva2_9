using Diva2.Core.Extensions;
using Diva2.Core.Main;
using Diva2.Core.Main.Trans;
using Diva2.Core.Main.Users;
using Diva2.Core.Main.Zakaznik;
using Diva2Web.Models.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Zakaznici
{
    public class ZakaznikModel
    {

        public ZakaznikModel(User8 u)
        {
            this.Id = u.Id;
            this.Name = $"{u.Prijmeni} {u.Jmeno}";
            this.Jmeno = u.Jmeno;
            this.Prijmeni = u.Prijmeni;
            this.NameClear = StringHelper.RemoveDiacritics(this.Name).ToLowerInvariant();
            this.Email = u.Email;
            this.Nick = u.UserName;
            this.Phone = u.PhoneNumber;
            this.Number = u.InternalNumber;
            this.Street = u.Ulice;
            this.Zip = u.Psc;
            this.Post = u.Posta;
            this.LastLogin = u.LastLogin;
            if (LastLogin.HasValue)
            {
                this.LastLoginDt = DateTimeExtensions.FromUnix(u.LastLogin.Value);
                this.LastLoginStr = LastLoginDt.Value.ToString("dd.MM.yy HH:mm");
            }

            this.GdprBase = u.GdprBase;
            this.GdprDt = u.GdprBaseDT;
            this.GdprNews = u.GdprNews;
            this.GdprNewsDT = u.GdprNewsDT;
            this.Poznamka = u.Poznamka;
            this.Poznamka2 = u.Poznamka2;
            this.Aktivni = u.Platnost;
            this.MayCheaper = u.MayCheaper;

        }

        public int Id { get; set; }

        public string Jmeno { get; set; }

        public string Prijmeni { get; set; }

        public string Name { get; set; }

        [JsonProperty("name_clr")]
        public string NameClear { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Nick { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("count")]
        public int CountLesson { get; set; }

        public string Street { get; set; }
        public string Zip { get; set; }
        public string Post { get; set; }
        public int? LastLogin { get; set; }

        public DateTime? LastLoginDt { get; internal set; }
        public string LastLoginStr { get; internal set; } = "";

        public UserZbytek Zbytek { get; internal set; }

        public IEnumerable<UserTransakce> Transakce { get; set; }

        public IEnumerable<int> TransakceYears { get; set; }

        public bool? GdprBase { get; }
        public DateTime? GdprDt { get; }
        public bool? GdprNews { get; }
        public DateTime? GdprNewsDT { get; }
        public IList<User8GroupUser> Groups { get; internal set; }

        public string Poznamka { get; set; }

        public string Poznamka2 { get; set; }

        public bool Aktivni { get; }
        public bool MayCheaper { get; }

        public int? Kredit { get; set; }

        public int? KreditCas { get; set; }

        public int? KreditCasDen { get; set; }

        public string KreditCasStr { get; set; }

        public string TypKreditu { get; set; }  

        public string Color { get; set; } = "silver";

        public int PocetLekci { get; set; }
    }

    public class ChangePasswordModel : ChangeUserModelBase
    {
        public int KontCislo1 { get; set; }

        public int KontCislo2 { get; set; }

        public string Heslo { get; set; }
    }

    public class AddRemoveGroupModel : ChangeUserModelBase
    {

        public int GroupId { get; set; }

        public bool Value { get; set; }
    }

    public class ChangeUserModelBase
    {
        public int UserId { get; set; }

        public string Temp { get; set; }
    }

    public class AddCredit0Model : ChangeUserModelBase
    {
        public int KontCislo1 { get; set; }

        public int KontCislo2 { get; set; }

        public int Kredit { get; set; }
    }

    public class PrevedKreditModel : ChangeUserModelBase
    {
        public int KontCislo1 { get; set; }

        public int KontCislo2 { get; set; }

        public int NewUserId { get; set; }
    }

    public class ExtendValidityModel
    {
        public int Id { get; set; }

        public int Count { get; set; }
    }
}
