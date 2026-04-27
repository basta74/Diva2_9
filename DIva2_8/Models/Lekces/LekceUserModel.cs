using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Trans;
using Diva2Web.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Lekces
{
    public class LekceUserModel
    {

        public LekceUserModel(LekceUser z)
        {

            CopyFromDb(z);
        }

        public int Id { get; set; }
        public int SpinId { get; set; }
        public int UserId { get; set; }
        public int PobockaId { get; set; }
        public int PokladnaId { get; set; }
        public int Poradi { get; set; }
        public int KontCislo { get; set; }
        public bool Aktivni { get; set; }
        public bool BylTam { get; set; }
        public bool Nahradnik { get; set; }
        public bool NahradnikJa { get; set; }
        public bool Premiera { get; set; }
        public string ZbytekTyp { get; set; }
        public int Zbytek { get; set; }
        public int ZbyvaDni { get; set; }
        public bool DoMzdy { get; set; }
        public int Unix { get; set; }
        public string Message { get; set; } = "";

        public UserModel User { get; set; }

        public string UserName { get { return User != null ? User.CeleJmeno : ""; } }

        public void CopyFromDb(LekceUser o)
        {
            Id = o.Id;
            UserId = o.UserId;
            SpinId = o.LekceId;
            UserId = o.UserId;
            PobockaId = o.PobockaId;
            PokladnaId = o.PobockaId;
            Poradi = o.Poradi;
            KontCislo = o.KontCislo;
            Aktivni = o.Aktivni;
            BylTam = o.BylTam;
            Nahradnik = o.Nahradnik;
            NahradnikJa = o.NahradnikJa;
            Premiera = o.Premiera;
            ZbytekTyp = o.ZbytekTyp;
            Zbytek = o.Zbytek;
            ZbyvaDni = o.ZbyvaDni;
            DoMzdy = o.DoMzdy;
            Unix = o.Unix;
            if (o.User != null)
            {
                User = new UserModel(o.User);
            }



        }
    }
}
