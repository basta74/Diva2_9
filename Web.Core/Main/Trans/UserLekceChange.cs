using Diva2.Core;
using Diva2.Core.Main.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Trans
{
    /// <summary>
    /// prihlaseni a odhlaseni z lekce
    /// </summary>
    public class UserLekceChange : BaseEntity
    {
        public int ProvedlId { get; set; }

        public int LekceId { get; set; }

        public int UserId { get; set; }

        public User8 User { get; set; }

        public string UserName
        {
            get
            {

                return (User != null) ? $"{User.Prijmeni} {User.Jmeno}" : "";
            }

        }


        public string Status { get; set; }

        public DateTime Ts { get; set; }

        public string TsCz { get { return Ts.ToString("dd.MM.yyyy HH:mm"); } }


    }

    public class UserLekceLogOut : BaseEntity
    {

        public UserLekceLogOut()
        {

        }
        public UserLekceLogOut(UserLekceChange ch)
        {
            ProvedlId = ch.ProvedlId;
            LekceId = ch.LekceId;
            UserId = ch.UserId;
            Ts = ch.Ts;

        }

        public int ProvedlId { get; set; }

        public int LekceId { get; set; }

        public int UserId { get; set; }

        public int Poradi { get; set; }

        public int PocetMist { get; set; }

        public int PocetZakazniku { get; set; }

        public bool ExistujeNahradnik { get; set; }
        public bool JePlatny { get; set; }
        public int SmsUserId { get; set; }
        public bool SmsActive { get; set; }
        public int? SmsStatus { get; set; }
        public DateTime Ts { get; set; }

        public string TsCz { get { return Ts.ToString("dd.MM.yyyy HH:mm"); } }
    }

    public class UserLekceLogIn : BaseEntity
    {
        public UserLekceLogIn()
        {

        }

        public UserLekceLogIn(LekceUser lu)
        {
            LekceId = lu.LekceId;
            UserId = lu.UserId;
            Poradi = lu.Poradi;
        }

        public int ProvedlId { get; set; }

        public int LekceId { get; set; }

        public int UserId { get; set; }

        public int Poradi { get; set; }

        public int PocetMist { get; set; }

        public int PocetZakazniku { get; set; }

        /// <summary>
        /// PobInt("maxKreditDoMinusu") PobInt("maxKreditDoMinusu1")
        /// </summary>
        public int KreditInit { get; set; }

        public int KreditLekce { get; set; }
        

        public int ObjednanychLekci { get; set; }

        public int ObjednanychVLekci { get; set; }

        public int ObjednaneKredity { get; set; }

        public int ZbytekKredit { get; set; }

        public int ZbytekKreditCas { get; set; }

        public DateTime Ts { get; set; }

        public string TsCz { get { return Ts.ToString("dd.MM.yyyy HH:mm"); } }

        public bool Pridat { get; set; }
        public bool FromAdministrace { get; set; }
        public int MaxPerUser { get; set; }
        public int MaxPerUserTime { get; set; }
    }
}
