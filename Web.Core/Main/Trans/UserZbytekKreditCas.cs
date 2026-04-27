using Diva2.Core;
using Diva2.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Diva2.Core.Main.Trans
{
    [DebuggerDisplay("{Kredit} l-{PocetLidi} m-{PocetMesicu} {PlatnostOd}-{PlatnostDo} {Platny}")]
    public class UserZbytekKreditCas : BaseEntity
    {
        public int PlatbaId { get; set; }
        public int UserId { get; set; }
        public int PokladnaId { get; set; }
        public int Kredit { get; set; }
        public int PocetLidi { get; set; }
        public bool Aktivni { get; set; }
        public int PocetMesicu { get; set; }

        public int PlatnostOdUnix { get; set; }
        
        public int PlatnostDoUnix { get; set; }

        public int? PlatnostUnixBreak { get; set; }

        public int KreditUnixTime { get; set; }

        public DateTime PlatnostOd { get { return DateTimeExtensions.FromUnix(PlatnostOdUnix); } }
        public DateTime PlatnostDo { get { return DateTimeExtensions.FromUnix(PlatnostDoUnix); } }
        
        // stare prosle nebudou platne
        public bool Platny { get; set; }

        public bool Prodlouzeno { get; set; }

        public string ZbyvaDni { get; set; }
        
    }
}
