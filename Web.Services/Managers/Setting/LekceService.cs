using Diva2.Core.Extensions;
using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Main;
using Diva2.Core.Model.Json.Response;
using Diva2.Data;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Diva2.Services.Managers.Setting
{

    public class LekceService : ILekceService
    {
        private readonly CacheHelper cache;
        private readonly IRepository<Lekce> repository;
        private readonly IRepository<LekceTyp> repTyp;
        private readonly IRepository<LekceMustr> repMustr;
        private readonly IRepository<Log8> repLog;

        public LekceService(ApplicationDbContext dbContext, IMemoryCache memoryCache,
                            IRepository<Lekce> repository, IRepository<LekceTyp> repTyp, IRepository<LekceMustr> repMustr, IRepository<Log8> repLo)
        {

            this.cache = new CacheHelper(memoryCache, dbContext.SubDomain);

            this.repository = repository;
            this.repTyp = repTyp;
            this.repMustr = repMustr;
            this.repLog = repLo;
        }

        #region Lekce
        public void ClearWeaksFromMonday(int pobId)
        {
            string cacheItemKey = $"GetLekceFromMonday>{pobId}";
            cache.ClearDataSub(cacheItemKey);
        }

        public Rozvrhy GetWeaksFromMonday(int pobId, DateTime day)
        {
            DateTime monday = DateTimeExtensions.StartOfWeek(day, DayOfWeek.Monday);

            string cacheItemKey = $"GetLekceFromMonday>{pobId}";

            var dtn = DateTime.Now;

            IEnumerable<Lekce> result = null;

            if (monday > dtn)
            {
                result = repository.Table.Where(d => d.PobockaId == pobId && d.Datum >= monday).OrderBy(d => d.Datum);
            }
            else
            {
                var days = (dtn - monday).TotalDays;

                if (days > 28)
                {
                    DateTime dt28 = monday.AddDays(28 + 1);
                    result = repository.Table.Where(d => d.PobockaId == pobId && d.Datum >= monday && d.Datum < dt28).OrderBy(d => d.Datum);
                }
                else
                {
                    result = repository.Table.Where(d => d.PobockaId == pobId && d.Datum >= monday).OrderBy(d => d.Datum);
                }
            }
            Rozvrhy ro = FillData(result);
            return ro;
        }

        public void ClearWeaksApi(int pobId)
        {
            string cacheItemKey = $"GetLekceWeakApi>{pobId}";
            cache.ClearDataSub(cacheItemKey);
        }

        public BranchData GetWeaksApi(int pobId) {

            string cacheItemKey = $"GetLekceWeakApi>{pobId}";

            DateTime dt = DateTime.Now;

            BranchData br = new BranchData();
            br.Lessons = repository.Table.Where(d => d.PobockaId == pobId && d.Datum > dt.AddDays(-1)).OrderBy(d => d.Datum).ToList();



            return br;
        }

        public void ClearWeaksFuture(int pobId)
        {
            string cacheItemKey = $"GetLekceWeakFuture>{pobId}";
            cache.ClearDataSub(cacheItemKey);
        }

        public Rozvrhy GetWeaksFuture(int pobId, DateTime? dtIn = null)
        {
            DateTime dt = dtIn ?? DateTime.Now;

            string cacheItemKey = $"GetLekceWeakFuture>{pobId}";


            var result = repository.Table.Where(d => d.PobockaId == pobId && d.Datum > dt.AddDays(-1)).OrderBy(d => d.Datum);

            Rozvrhy ro = FillData(result);

            return ro;

        }

        public Rozvrhy2 GetWeaksFuture2(int pobId, DateTime? dtIn = null)
        {
            DateTime dt = dtIn ?? DateTime.Now;

            string cacheItemKey = $"GetLekceWeakFuture>{pobId}";


            var result = repository.Table.Where(d => d.PobockaId == pobId && d.Datum > dt.AddDays(-1)).OrderBy(d => d.Datum);

            Rozvrhy2 ro = FillData2(result);

            return ro;

        }

        private Rozvrhy FillData(IEnumerable<Lekce> result)
        {

            Rozvrhy ro = new Rozvrhy();
            Dictionary<int, LekceTyden> list = null; // cache.GetDataSub<Dictionary<int, LekceTyden>>(cacheItemKey);
            if (true)
            {
                ro.Tydny = new Dictionary<int, LekceTyden>();
                foreach (Lekce lek in result)
                {
                    if (lek.Lektor1 > 0)
                    {
                        ro.ActLectors.Add(lek.Lektor1);
                    }

                    if (!ro.PouzitePoradi.Contains(lek.MinutaKey))
                    {
                        ro.PouzitePoradi.Add(lek.MinutaKey);
                    }


                    var lekTy = new LekceTyden();
                    if (!ro.Tydny.TryGetValue(lek.Rok + lek.Tyden, out lekTy))
                    {
                        lekTy = new LekceTyden();
                        ro.Tydny.Add(lek.Rok + lek.Tyden, lekTy);
                    }


                    var lekDe = new LekceDen();
                    if (!lekTy.Dny.TryGetValue(lek.Den, out lekDe))
                    {
                        lekDe = new LekceDen();
                        lekTy.Dny.Add(lek.Den, lekDe);
                    }

                    try
                    {
                        lekDe.Lekces.Add(lek.MinutaKey, lek);

                    }
                    catch (Exception ex)
                    {
                        //  repLog.Insert(new Log8() { Category = LogCategory.rozvrh_generate, Text = ex.Message.ToString(), Created = DateTime.Now });
                    }

                }
            }

            return ro;

        }

        private Rozvrhy2 FillData2(IEnumerable<Lekce> result)
        {

            Rozvrhy2 ro = new Rozvrhy2();

            if (true)
            {
                ro.Tydny = new Dictionary<int, LekceTyden2>();
                foreach (var lek in result)
                {
                    ro.ActLectors.Add(lek.Lektor1);

                    if (!ro.PouzitePoradi.Contains(lek.MinutaKey))
                    {
                        ro.PouzitePoradi.Add(lek.MinutaKey);
                    }

                    var lekTy = new LekceTyden2();
                    if (!ro.Tydny.TryGetValue(lek.Rok + lek.Tyden, out lekTy))
                    {
                        lekTy = new LekceTyden2();

                        lekTy.Monday = DateTimeExtensions.StartOfWeek(lek.Datum, DayOfWeek.Monday);

                        ro.Tydny.Add(lek.Rok + lek.Tyden, lekTy);
                    }


                    var lekDlePoradi = new LekceVose();
                    if (!lekTy.Hodiny.TryGetValue(lek.MinutaKey, out lekDlePoradi))
                    {
                        lekDlePoradi = new LekceVose();
                        lekTy.Hodiny.Add(lek.MinutaKey, lekDlePoradi);
                    }

                    if (!lekTy.PouziteDny.ContainsKey(lek.Den))
                    {
                        lekTy.PouziteDny.Add(lek.Den, lek.Datum);
                    }

                    try
                    {
                        lekDlePoradi.Dny.Add(lek.Den, lek);
                    }
                    catch (Exception ex)
                    {
                        //  repLog.Insert(new Log8() { Category = LogCategory.rozvrh_generate, Text = ex.Message.ToString(), Created = DateTime.Now });
                    }

                }
            }

            return ro;

        }

        public Rozvrhy3 GetWeaksFuture3(int pobId)
        {
            DateTime dt = DateTime.Now;

            string cacheItemKey = $"GetLekceWeakFuture>{pobId}";


            var result = repository.Table.Where(d => d.PobockaId == pobId && d.Datum > dt.AddDays(-1)).OrderBy(d => d.Datum);

            Rozvrhy3 ro = FillData3(result);

            return ro;

        }

        private Rozvrhy3 FillData3(IOrderedQueryable<Lekce> result)
        {
            Rozvrhy3 ro = new Rozvrhy3();

            if (true)
            {

                var dny = result.ToLookup(d => d.Datum.Date);
                foreach (var den in dny)
                {
                    LekceDen3 d3 = new LekceDen3();
                    ro.Dny.Add(den.Key, d3);

                    var lekceDne = dny[den.Key];

                    var drahy = lekceDne.ToLookup(d => d.Zdroj);

                    foreach (var dr in drahy)
                    {
                        Dictionary<int, Lekce> dict = drahy[dr.Key].ToDictionary(d => d.MinutaKey, dr => dr);
                        d3.LekcesDraha.Add(dr.Key, dict);
                    }
                }
            }

            return ro;
        }

        public IEnumerable<Lekce> GetWeakByDay(int pobId, DateTime dt)
        {
            

            int w = DateTimeExtensions.GetWeekNumber(dt);
            DateTime monday = DateTimeExtensions.StartOfWeek(dt, DayOfWeek.Monday);

            var result = new List<Lekce>();
            DateTime sunday = monday.AddDays(7);
            result = repository.Table.Where(d => d.PobockaId == pobId && d.Tyden == w && d.Rok == sunday.Year).ToList();


            return result;
        }

        public Lekce GetById(int id)
        {
            return repository.Table.Where(d => d.Id == id).FirstOrDefault();
        }

        public void Insert(Lekce obj)
        {
            repository.Insert(obj);
            ClearWeaksFromMonday(obj.PobockaId);
            ClearWeaksFuture(obj.PobockaId);
        }

        public void Update(Lekce obj)
        {
            repository.Update(obj);
            ClearWeaksFromMonday(obj.PobockaId);
            ClearWeaksFuture(obj.PobockaId);
        }

        public void Delete(Lekce obj)
        {
            repository.Delete(obj);
            ClearWeaksFromMonday(obj.PobockaId);
            ClearWeaksFuture(obj.PobockaId);
        }
        #endregion

        #region LekceMustr

        public LekceMustr GetMustrById(int id)
        {
            return repMustr.Table.Where(d => d.Id == id).FirstOrDefault();
        }

        #endregion


        public List<int> GetLektoryMesic(int rok, int mes)
        {

            List<int> li = (from m in repMustr.TableUntracked
                            join l in repository.TableUntracked
                            on m.Id equals l.MustrId
                            where l.Datum.Year == rok && l.Datum.Month == mes
                            select l.Lektor1).Distinct().ToList();
            return li;

        }

        public List<Lekce> GetMzdyMesic(int rok, int mes)
        {
            var le = repository.TableUntracked.Where(d => d.Datum.Year == rok && d.Datum.Month == mes).OrderBy(d => d.Datum).ToList();
            return le;

        }

        public List<Lekce> GetMzdyMesic(int lekId, int rok, int mes)
        {
            List<int> li = (from m in repMustr.TableUntracked
                            join l in repository.TableUntracked
                            on m.Id equals l.MustrId
                            where l.Datum.Year == rok && l.Datum.Month == mes
                            select l.Lektor1).Distinct().ToList();


            var le = repository.TableUntracked.Where(d => d.Datum.Year == rok && d.Datum.Month == mes && d.Lektor1 == lekId).OrderBy(d => d.Datum).ToList();
            return le;

        }

        public List<Lekce> GetByDay(int pobId, DateTime currentDate)
        {
            var le = repository.TableUntracked.Where(d => d.PobockaId == pobId && d.Datum == currentDate).OrderBy(d => d.HodinaPoradi).ToList();
            return le;
        }

        public Lekce GetBy(int rok, int tyden, int den, int hodinaPoradi)
        {
            var le = repository.TableUntracked.Where(d => d.Rok == rok && d.Tyden == tyden && d.Den == den && d.MinutaKey == hodinaPoradi).FirstOrDefault();
            return le;
        }


        public IList<Lekce> GetNotClossed(int id)
        {
            var le = repository.TableUntracked.Where(d => d.Zauctovano == false && d.PobockaId == id && d.PocetZakazniku > 0 && d.Datum < DateTime.Now).OrderBy(d => d.Datum).ToList();
            return le;
        }
    }

}