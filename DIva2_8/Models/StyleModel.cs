using Diva2.Core.Main;
using Diva2Web.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models
{
    public class StyleModel
    {
        private Pobocka pobocka;

        public string Color1 { get; set; } = "#AF6486";

        public string Color2 { get; set; } = "#E469A1";

        public string MotivHlavni { get; set; } = "#8B1D1D";

        public string MotivKontra { get; set; } = "#ED3908";

        public StyleModel()
        {

        }

        public StyleModel(Pobocka pobocka)
        {
            if (pobocka.Color1 != "") {
                Color1 = pobocka.Color1;
            }
            if (pobocka.Color2 != "")
            {
                Color2 = pobocka.Color2;
            }
            if (pobocka.MotivHlavni != "")
            {
                MotivHlavni = pobocka.MotivHlavni;
            }
            if (pobocka.MotivHlavni != "")
            {
                MotivKontra = pobocka.MotivKontra;
            }
        }

    }
}
