using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Comunications
{
    public class SmsLog : BaseEntity
    {
        public int PobockaId { get; set; }
        public int LekceId { get; set; }

        public int ZakaznikId { get; set; }

        public int UserId { get; set; }

        public string Cislo { get; set; }

        public string Email { get; set; }

        public string Text { get; set; }

        public int CreateUnix { get; set; }

        public int SendUnix { get; set; }

        public int SendUserId { get; set; }

        public int Stav { get; set; }

        public bool Vyrizeno { get; set; }

        /// <summary>
        /// kod se kterym se vola
        /// </summary>
        public string Kod { get; set; }

        /// <summary>
        /// pripadna chyba
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// odpoved sluzby
        /// </summary>
        public string Result { get; set; }

        public double Credit { get; set; }
    }
}
