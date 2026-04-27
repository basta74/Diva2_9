using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Calendar
{
    public class CalEvent : BaseEntity
    {
        public int PobockaId { get; set; }

        public int DrahaId { get; set; }

        public int Day { get; set; }

        public DateTime Date { get; set; }

        public int From { get; set; }

        public int To { get; set; }

        public int CreatedUserId { get; set; }

        public DateTime CreatedDt { get; set; }

        public DateTime Ts { get; set; }
    }
}
