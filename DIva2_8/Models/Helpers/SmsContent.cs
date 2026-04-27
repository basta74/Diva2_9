using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Helpers
{
    public class SmsContent
    {
        public int Number { get; set; }

        public Dictionary<int, int> Numbers { get; set; }

        public string Text { get; set; }

        public string Name { get; set; }

        public string Pass { get; set; }

        /// <summary>
        /// pojistovaci kod
        /// </summary>
        public string Kod { get; set; }

        /// <summary>
        /// pojistovaci kod
        /// </summary>
        public string Result { get; set; }

        public double Credit { get; set; }

    }
}
