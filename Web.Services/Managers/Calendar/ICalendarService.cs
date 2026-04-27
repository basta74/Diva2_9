using Diva2.Core.Main.Calendar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Services.Managers.Calendar
{
    public interface ICalendarService
    {
        IEnumerable<CalEvent> GetCalEvents(int id, DateTime currentDate, int v);

        void Insert(CalEvent o);

        void Delete(CalEvent o);

        void Update(CalEvent o);


       
    }
}
