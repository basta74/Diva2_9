using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Calendar
{
    public class CalIniMinute : BaseEntity
    {
        public int PobockaId { get; set; }

        public int  Minute { get; set; }

        public TimeSpan Time { get; set; }

        public bool Visible { get; set; }
    }
}
