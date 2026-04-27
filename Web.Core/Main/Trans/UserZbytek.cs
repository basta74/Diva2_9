using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Trans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diva2.Core.Main.Trans
{
    public class UserZbytek
    {
        public Dictionary<int, int> Kredity { get; set; }
        public IList<UserZbytekKreditCas> KredityCas { get; set; }

        public int KredityItem(int pokladnaId)
        {
            Kredity.TryGetValue(pokladnaId, out int val);
            return val;
        }

        public UserZbytekKreditCas KredityTimeItem(int pokladnaId, DateTime dt)
        {
            UserZbytekKreditCas uzkc = null;

            if (KredityCas != null)
            {
                var unix = ((DateTimeOffset)dt).ToUnixTimeSeconds();

                var krc = KredityCas.Where(d => d.PokladnaId == pokladnaId && d.PlatnostDoUnix > unix && d.PlatnostOdUnix < unix);
                if (krc.Count() == 0)
                {

                }
                else if (krc.Count() == 1)
                {
                    uzkc = krc.FirstOrDefault();
                    if (uzkc.Aktivni)
                    {
                        var zb = uzkc.PlatnostDoUnix - unix;
                        var d = (int)(zb / (60 * 60 * 24));
                        uzkc.ZbyvaDni = d.ToString();
                    }
                    else
                    {
                        uzkc.ZbyvaDni = " - ";
                    }
                }
                // vice
                else
                {
                    uzkc = new UserZbytekKreditCas();
                    uzkc.Kredit = krc.Sum(d => d.Kredit);
                    uzkc.ZbyvaDni = " + ";
                }
            }

            return uzkc;
        }

        public void SetZbytekToActualLekceUser(LekceUser lu, Lekce le)
        {
            var dt = le.Datum;
            dt = dt.Add(le.Cas);

            var unix = ((DateTimeOffset)dt).ToUnixTimeSeconds();

            lu.Zbytek = KredityItem(lu.PokladnaId);
            lu.ZbytekTyp = "k";


            if (KredityCas != null)
            {       

                var krcs = KredityCas.Where(d => d.PokladnaId == lu.PokladnaId && d.PlatnostDoUnix > unix && d.PlatnostOdUnix < unix);
                if (krcs.Count() == 0)
                {

                }
                else if (krcs.Count() == 1)
                {
                    var krc1 = krcs.FirstOrDefault();
                    lu.Zbytek = krc1.Kredit;
                    if (krc1.PlatnostUnixBreak > 0)
                    {
                        lu.ZbytekTyp = "kb";
                    }
                    else { 
                        lu.ZbytekTyp = "kc";
                    }

                    var zb = krc1.PlatnostDoUnix - unix;
                    var d = (int)(zb / (60 * 60 * 24));
                    lu.ZbyvaDni = d;
                }
                // vice
                else
                {

                    lu.Zbytek = krcs.Sum(d => d.Kredit);
                    lu.ZbyvaDni = 888;
                    lu.ZbytekTyp = "kc";
                }
            }

        }
        public string GetForString(int pokId, DateTime dt)
        {
            long unix = ((DateTimeOffset)dt).ToUnixTimeSeconds();
            var ret = GetForString(pokId, unix);
            return ret;
        }
        public string GetForString(int pokId, long unix = 0)
        {

            string ret = "";

            if (unix == 0)
            {
                unix = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
            }

            Kredity.TryGetValue(pokId, out int val);

            ret = val.ToString();


            if (KredityCas != null)
            {

                var krc = KredityCas.Where(d => d.PokladnaId == pokId && unix < d.PlatnostDoUnix && d.PlatnostOdUnix > unix);
                if (krc.Count() == 0)
                {

                }
                else if (krc.Count() == 1)
                {
                    var krc1 = krc.FirstOrDefault();

                    var zb = krc1.PlatnostDoUnix - unix;
                    var d = zb / (60 * 60 * 24);

                    ret = $" {krc1.Kredit}/ {d}";
                }
                // vice
                else 
                {

                    var zb = krc.Sum(d => d.Kredit);
                    ret = $" {zb}/ +";
                }
            }

            return ret;
        }
    }
}
