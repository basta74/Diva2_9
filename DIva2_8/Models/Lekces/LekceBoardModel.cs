using Diva2.Core.Main;
using Diva2.Core.Main.Lessons;
using Diva2Web.Models.Admin;
using Diva2Web.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Lekces
{
    public class LekceBoardModel
    {
        private Lekce lekce;

        public LekceBoardModel(Lekce lekce)
        {
            CopyFromDb(lekce);
            Zakaznici = new List<LekceUserModel>();
        }

        public LekceBoardModel()
        {
        }

        public UserModel User { get; set; }

        public Pobocka Pobocka { get; set; }

        public int Id { get; set; }

        public int PobockaId { get; set; }


        public DateTime Datum { get; set; }

        public int HodinaPoradi { get; set; }

        public TimeSpan HodinaZacatek { get; set; }

        /// <summary>
        /// Zacatek hodiny datum + cas
        /// </summary>
        public DateTime DatumHodina
        {
            get
            {
                DateTime dt = this.Datum;
                dt = dt.Add(this.HodinaZacatek);
                return dt;
            }
        }


        public string Nazev { get; set; }

        public int Kredit { get; set; }

        public int Minuty { get; set; }

        public int Lektor1 { get; set; }

        public int Lektor2 { get; set; }

        public LektorModel Lektor1_O { get; internal set; }
        public LektorModel Lektor2_O { get; internal set; }


        public int TypHodiny { get; set; }

        public LekceTypModel TypHodiny_O { get; set; }

        public int PocetMist { get; set; }

        public int PocetZakazniku { get; set; }

        public int PocetNahradniku { get; set; }

        public bool Obsazeno()
        {

            return (PocetNahradniku + PocetMist) < PocetZakazniku;

        }

        public bool Zauctovano { get; set; }

        public int? ZauctovalUserId { get; set; }

        public string ZauctovalUserName { get; set; }

        public DateTime? ZauctovanoDate { get; set; }

        public IList<LekceUserModel> Zakaznici { get; set; }

        public LekceVideo Video { get; set; }

        public string Text { get; set; }

        internal void CopyFromDb(Lekce o)
        {
            Id = o.Id;
            Nazev = o.Nazev;
            Kredit = o.Kredit;
            Minuty = o.Minuty;
            Lektor1 = o.Lektor1;
            Lektor2 = o.Lektor2;
            TypHodiny = o.TypHodiny;
            PocetMist = o.PocetMist;
            PocetZakazniku = o.PocetZakazniku;
            PocetNahradniku = o.PocetNahradniku;
            Datum = o.Datum;
            HodinaZacatek = o.Cas;
            Zauctovano = o.Zauctovano;
            ZauctovalUserId = o.ZauctovalUserId;
            ZauctovalUserName = o.ZauctovalUserName;
            ZauctovanoDate = o.ZauctovanoDate;
        }


        public DateTime DatumProOdhlaseni(AdminPageModel model)
        {

            DateTime zacatekBase = this.DatumHodina;
            //pevne
            if (model.GetIniPobBool("lekceOdhlasPevne"))
            {
                DateTime zacatek = new DateTime(zacatekBase.Year, zacatekBase.Month, zacatekBase.Day);

                // zada pocet dni kdy se da odhlasit 0 ten den 1 a vic kolik dni pred hodinou
                int minusD = model.GetIniPobInt("lekceOdhlasPevneDenPred");

                string minusHs = model.GetIniPob("lekceOdhlasPevneHodDne");
                TimeSpan ts = TimeSpan.Parse(minusHs);
                int minusH = ts.Hours;

                zacatek = zacatek.AddDays(-minusD);
                zacatek = zacatek.AddHours(minusH);

                // zacatek 9:00 a dle nastaveni 12:00 tak snizim puvodni zacatek o hodinu
                if (zacatek > zacatekBase) {
                    return zacatekBase.AddHours(-1);
                }
                else
                {
                    return zacatek;
                }
            }
            //plov
            else if (model.GetIniPobBool("lekceOdhlasPlov"))
            {

                int minusH = model.GetIniPobInt("lekceOdhlasPlovHod");
                zacatekBase = zacatekBase.AddHours(-minusH);
                return zacatekBase;
            }
            else
            {
                return zacatekBase.AddHours(-1);
            }

            
        }

        public DateTime DatumProPrihlaseni(AdminPageModel model)
        {
            DateTime zacatek = this.DatumHodina;


            int hodPred = model.GetIniPobInt("lekce_plovhod_prihlas");

            if (hodPred > 0)
            {
                zacatek = zacatek.AddHours(-hodPred);
            }
            else
            {
                /*
                int dniPred = model.GetIniPobInt("lekce_pevhod_prihlas_den");
                string casPred = model.GetIniPob("lekce_pevhod_prihlas_cas");

                if (dniPred > 0)
                {
                    zacatek = zacatek.AddDays(-dniPred);
                }
                /**/
            }

            return zacatek;
        }
    }
}
