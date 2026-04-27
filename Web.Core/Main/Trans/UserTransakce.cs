using Diva2.Core;
using Diva2.Core.Main.Platby;
using Diva2.Core.Main.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Trans
{
    public class UserTransakce : BaseEntity, ICloneable
    {

        public User8 User { get; set; }
        public int UserId { get; set; }
        public int PokladnaId { get; set; }
        public int ProvedlId { get; set; }
        /// <summary>
        /// +,-,>
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// k,kc,c
        /// </summary>
        public string Typ { get; set; }
        public int Kredit { get; set; }
        public int? Castka { get; set; }
        public int DoPokladny { get; set; }
        public int LekceId { get; set; }

        public int? VideoId { get; set; }
        public int Doklad { get; set; }
        public int Zbytek { get; set; }
        public int ZbyvaDni { get; set; }

        public int Increment { get; set; }
        public DateTime Timestamp { get; set; }

        public DateTime Datum { get; set; }
        public int UnixTime { get; set; }

        public DateTime? PlatnostOd { get; set; }
        public DateTime? PlatnostDo { get; set; }
        public int? PlatnostOdUnix { get; set; }
        public int? PlatnostDoUnix { get; set; }
        public int PlatbaId { get; set; }
        public PlatbaKreditCas PlatbaTime { get; set; }
        public PlatbaCas PlatbaPausal { get; set; }
        public bool IsOk { get; set; }
        public bool ZBanky { get; set; } = false;

        public object Clone()
        {
            UserTransakce t = new UserTransakce();

            t.UserId = this.UserId;
            t.PokladnaId = this.PokladnaId;
            t.ProvedlId = this.ProvedlId;
            t.Status = this.Status;
            t.Typ = this.Typ;
            t.Kredit = this.Kredit;
            t.Castka = this.Castka;
            t.DoPokladny = this.DoPokladny;
            t.ZBanky = this.ZBanky;
            t.LekceId = this.LekceId;
            t.Doklad = this.Doklad;
            t.Zbytek = this.Zbytek;
            t.ZbyvaDni = this.ZbyvaDni;

            t.Increment = this.Increment;
            t.Timestamp = this.Timestamp;

            t.Datum = this.Datum;
            t.UnixTime = this.UnixTime;

            t.PlatnostOd = this.PlatnostOd;
            t.PlatnostDo = this.PlatnostDo;
            t.PlatnostOdUnix = this.PlatnostOdUnix;
            t.PlatnostDoUnix = this.PlatnostDoUnix;

            return t;
        }
        public enum TypTransakceKreditu { k, kc, p };
    }
}
