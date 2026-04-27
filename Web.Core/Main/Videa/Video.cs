using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Videa
{
    public class Video : BaseEntity
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string Image { get; set; }

        public string Desc { get; set; }

        public bool Visible { get; set; }

        public int Kredity { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }
}
