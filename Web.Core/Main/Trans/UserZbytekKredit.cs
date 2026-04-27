using Diva2.Core;
using Diva2.Core.Main.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Trans
{
    public class UserZbytekKredit : BaseEntity
    {

        public int UserId { get; set; }
        public int PokladnaId { get; set; }
        public int Kredit { get; set; }
        public int KreditUnixTime { get; set; }
        public int Rezervace { get; set; }
        public int RezervaceUnixTime { get; set; }

        public bool IsSet { get; set; }

        public int Kr1From { get; set; }
        public int Kr1To { get; set; }
        public int Kr1{ get; set; }

        public int Kr2From { get; set; }
        public int Kr2To { get; set; }
        public int Kr2 { get; set; }
    }
}
