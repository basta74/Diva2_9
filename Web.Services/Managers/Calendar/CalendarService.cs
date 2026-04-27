using Diva2.Core.Main.Calendar;
using Diva2.Data;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diva2.Services.Managers.Calendar
{
    public class CalendarService : ICalendarService
    {
        private CacheHelper cache;
        private IRepository<CalEvent> rep;
        private IRepository<CalIniMinute> repIniMin;

        public CalendarService(ApplicationDbContext dbContext, IRepository<CalEvent> re, IMemoryCache memoryCache)
        {
            this.cache = new CacheHelper(memoryCache, dbContext.SubDomain);
            this.rep = re;

        }


        #region Events
        public IEnumerable<CalEvent> GetCalEvents(int id, DateTime from, int v = 7)
        {

            return rep.TableUntracked.Where(d => d.Date > from.AddDays(-1) && d.Date < from.AddDays(v + 1) && d.PobockaId == id);

        }

        public void Insert(CalEvent o)
        {
            rep.Insert(o);
        }

        public void Update(CalEvent o)
        {
            rep.Update(o);

        }

        public void Delete(CalEvent o)
        {
            rep.Delete(o);
        }



        #endregion

        
    }
}
