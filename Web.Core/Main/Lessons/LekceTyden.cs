using Diva2.Core.Main;
using Diva2.Core.Main.Lessons;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Lessons
{
    public class Rozvrhy
    {

        public List<int> PouzitePoradi { get; set; } = new List<int>();



        public Dictionary<int, LekceTyden> Tydny = new Dictionary<int, LekceTyden>();

        public HashSet<int> ActLectors { get; set; } = new HashSet<int>();
    }

    public class LekceTyden
    {
        public Dictionary<int, LekceDen> Dny = new Dictionary<int, LekceDen>();
    }

    public class LekceDen
    {
        public Dictionary<int, Lekce> Lekces = new Dictionary<int, Lekce>();
    }


    public class Rozvrhy2
    {

        public List<int> PouzitePoradi { get; set; } = new List<int>();

        public Dictionary<int, LekceTyden2> Tydny = new Dictionary<int, LekceTyden2>();

        public HashSet<int> ActLectors { get; set; } = new HashSet<int>();
    }

    public class LekceTyden2
    {
        public DateTime Monday { get; set; }
        public Dictionary<int, DateTime> PouziteDny { get; set; } = new Dictionary<int, DateTime>();

        public Dictionary<int, LekceVose> Hodiny = new Dictionary<int, LekceVose>();
    }

    public class LekceVose
    {
        public Dictionary<int, Lekce> Dny = new Dictionary<int, Lekce>();
    }


    public class Rozvrhy3
    {
        public Dictionary<DateTime, LekceDen3> Dny = new Dictionary<DateTime, LekceDen3>();
    }

    public class LekceDen3
    {
        public Dictionary<int, Dictionary<int, Lekce>> LekcesDraha = new Dictionary<int, Dictionary<int, Lekce>>();
    }
}
