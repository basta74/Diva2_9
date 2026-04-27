
using Diva2.Core.Main;
using Diva2.Core.Main.Lektori;
using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Main;
using Diva2.Core.Main.Platby;
using Diva2.Core.Main.Pobocky;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core
{

    public class Company : MainIni
    {
        public Company(MainIni ini)
        {

            this.MasterUrl = ini.MasterUrl;
            this.MasterUrlText = ini.MasterUrlText;
            this.MasterJmenoMajitele = ini.MasterJmenoMajitele;
            this.MasterName = ini.MasterName;
            this.MasterStreet = ini.MasterStreet;
            this.MasterZip = ini.MasterZip;
            this.MasterPost = ini.MasterPost;
            this.Email = ini.Email;
            this.ICO = ini.ICO;
            this.DIC = ini.DIC;
            this.RegisterEmail = ini.RegisterEmail;


            this.SmsActive = ini.SmsActive;
            this.SmsName = ini.SmsName;
            this.SmsPass = ini.SmsPass;

            this.TextNaPrijmovyDoklad = ini.TextNaPrijmovyDoklad;
            this.ShowRestPlacesOnBoard = ini.ShowRestPlacesOnBoard;

        }

        public List<Branch> Branches { get; set; } = new List<Branch>();

        public string Tenant { get; set; }

        public IList<Lektor> Lectors { get; set; } = new List<Lektor>();

        public IList<LekceTyp> LessonTypes { get; set; } = new List<LekceTyp>();

    }

    public class Branch
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public int Kredity { get; set; }

        public int Minuty { get; set; }

        public bool Visible { get; set; }

        public int PocetMist { get; set; }

        public int PokladnaId { get; set; }

        /// <summary>
        /// 1- s mustrem nahore hodiny, 2-s mustrem nahore datum, 3-plovoucí
        /// </summary>
        public int Typ { get; set; }

        public int Order { get; set; }

        public string Color1 { get; set; } = "#AF6486";

        public string Color2 { get; set; } = "#E469A1";

        public string MotivHlavni { get; set; } = "#8B1D1D";

        public string MotivKontra { get; set; } = "#ED3908";

        public bool PobockaType { get; set; }

        public bool PodbarvovatObsazene { get; set; }

        public string CalendarSetting { get; set; }

        public CalendarSetting CalendarSettingObj { get; set; } = new CalendarSetting() { Drahy = 1, Minutes = 15, HourStart = 6, HourEnd = 22 };
        public List<BranchIni> Inis { get; set; }
        public IList<PlatbaKredit> PaymentsCredit { get; set; }
        public IList<PlatbaKreditCas> PaymentsTime { get; set; }

        public Branch(Pobocka pob)
        {
            this.Id = pob.Id;

            this.Name = pob.Name;

            this.Description = pob.Description;

            this.Kredity = pob.Kredity;

            this.Minuty = pob.Minuty;

            this.Visible = pob.Visible;

            this.PocetMist = pob.PocetMist;

            this.PokladnaId = pob.PokladnaId;

            /// <summary>
            /// 1- s mustrem nahore hodiny, 2-s mustrem nahore datum, 3-plovoucí
            /// </summary>
            this.Typ = pob.Typ;

            this.Order = pob.Order;

            this.Color1 = pob.Color1;

            this.Color2 = pob.Color2;

            this.MotivHlavni = pob.MotivHlavni;

            this.MotivKontra = pob.MotivKontra;

            this.PobockaType = pob.PobockaType;

            this.PodbarvovatObsazene = pob.PodbarvovatObsazene;

            this.CalendarSetting = pob.CalendarSetting;
        }

      

        

        public class BranchIni {

            public string Name { get; set; }
            public string Value { get; set; }

        }



    }
}
