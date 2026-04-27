using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Diva2.Core.Main
{
    public class Pobocka : BaseEntity
    {
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
    }
    public enum PobockaType { classic, calendar }

    public class CalendarSetting
    {

        public int Drahy { get; set; }

        public int Minutes { get; set; }

        public int HourStart { get; set; }

        public int HourEnd { get; set; }

        private IEnumerable<int> _minutes { get; set; }

        public IEnumerable<int> GetMinutes()
        {
            if (_minutes != null) {
                return _minutes;
            }

            int end = HourEnd * 60;
            int m = HourStart * 60;
            List<int> ret = new List<int>();
            do
            {
                ret.Add(m);
                m = m + Minutes;

            } while (m <= end);

            _minutes = ret;
            return _minutes;
        }

    }
}
