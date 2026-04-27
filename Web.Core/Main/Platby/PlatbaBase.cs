using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Platby
{
    public class PlatbaBase : BaseEntity
    {
        /// <summary>
        /// 1,2 kr 3,4 krc
        /// </summary>
        public int Kategorie { get; set; }
        public int Castka { get; set; }
        public string Popis { get; set; }

        public int Poradi { get; set; }

        public bool Platnost { get; set; }
        public bool Visible { get; set; }
    }

    public class PlatbaCas : PlatbaBase
    {
        public string Verze { get; set; }
        public int AutorId { get; set; }
        public int Unix { get; set; }

    }

    public class PlatbaKredit : PlatbaBase
    {
        public int PokladnaId { get; set; }
        public int Kredity { get; set; }
        public int AutorID { get; set; }
        public int Unix { get; set; }

    }

    public class PlatbaKreditCas : PlatbaBase
    {
        public int PokladnaId { get; set; }
        public int Kredity { get; set; }

        public int PocetLidi { get; set; }
        public int PocetMesicu { get; set; }

        public int AutorID { get; set; }
        public int Ts { get; set; }


    }

    public class PlatbaCasPobocka : BaseEntity
    {
        public int PlatbaId { get; set; }
        public int PobockaId { get; set; }
    }

}
