using Diva2.Core.Main.Main;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Models
{
    public class MenuItem
    {
        public string Area { get; set; }

        public string Url { get; set; }

        public string Controller { get; set; } = "Home";

        public string Method { get; set; }

        public string Text { get; set; }

        /// <summary>
        /// Pravo ktere je nutne pro zobrazeni
        /// </summary>
        public string Pravo { get; set; }

        /// <summary>
        /// pob Ini ktere je nutne pro zobrazeni
        /// </summary>
        public string SettingPobIni { get; set; }

        /// <summary>
        /// pob Ini ktere je nutne pro zobrazeni
        /// </summary>
        public MainIniRuleItem SettingMainIni { get; set; }

        public bool Visible { get; set; } = false;

        public int Order { get; set; } = 10;

        public List<MenuItem> Items { get; set; }

    }
}
