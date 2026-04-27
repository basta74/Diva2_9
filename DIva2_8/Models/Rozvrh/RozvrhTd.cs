using Diva2.Core.Main.Calendar;
using Diva2.Core.Main.Lektori;
using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Trans;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Rozvrh
{
    public class RozvrhTd
    {
        public Lekce Lekce { get; set; }

        public IList<Lektor> Lektori { get; set; }

        public IList<LekceTyp> TypyLekci { get; set; }

        public IList<LekceUser> ObjednaneLekce { get; set; }

        public int UkazOd { get; set; }

        public int UkazKapacitu { get; set; }

        public int UkazLektora { get; set; }

        public int Lektor2 { get; set; }

        public int UkazTyp { get; set; }

        public CalIniMinute Cas { get; set; }

        public int ColSpan { get; set; }
    }
}
