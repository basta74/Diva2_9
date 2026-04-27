using Diva2.Core;
using Diva2.Core.Extensions;
using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Platby;
using Diva2.Core.Main.Trans;
using Diva2.Core.Main.Users;
using Diva2.Core.Main.Zakaznik;
using Diva2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Diva2.Core.Main.Trans.UserTransakce;

namespace Diva2.Services.Managers.Platby
{
    public class ObjednavkyService : IObjednavkyService
    {

        private readonly IRepository<LekceUser> repLekceUser;
        private readonly ApplicationDbContext dbContext;
        private readonly IRepository<Lekce> repLekce;
        private readonly IRepository<User8> repUser;
        private readonly IRepository<UserTransakce> repTransUser;
        private readonly IRepository<UserText> repUserText;
        private readonly IRepository<UserZbytekKreditCas> repUserCreditsTime;
        private readonly IRepository<UserZbytekKredit> repUserCredits;
        private readonly IRepository<UserLekceChange> repUserChange;
        private readonly IRepository<UserLekceLogOut> repUserChangeLogOut;
        private readonly IRepository<UserLekceLogIn> repUserChangeLogIn;
        private readonly IRepository<PrijmoveDoklady> repPrijmoveDoklady;
        private readonly IRepository<UserZbytekKreditCasLog> repUserCreditsTimeLog;
        protected static CacheHelper cache;


        public ObjednavkyService(ApplicationDbContext dbContext, IMemoryCache memoryCache,
            IRepository<LekceUser> reposLekceUser, IRepository<UserTransakce> reposTransUser, IRepository<Lekce> reposLekce,
            IRepository<User8> userRep, IRepository<UserZbytekKreditCas> repUserCreditsTime, IRepository<UserZbytekKredit> repUserCredits,
            IRepository<UserLekceChange> repUserChange, IRepository<UserText> repUserText, IRepository<PrijmoveDoklady> repPrijmoveDoklady,
            IRepository<UserZbytekKreditCasLog> repUserCreditsTimeLog, IRepository<UserLekceLogOut> repUserChangeL, IRepository<UserLekceLogIn> repUserChangeLI)
        {
            cache = new CacheHelper(memoryCache, dbContext.SubDomain);
            this.repLekceUser = reposLekceUser;
            this.repTransUser = reposTransUser;
            this.repLekce = reposLekce;
            this.repUser = userRep;
            this.repUserCreditsTime = repUserCreditsTime;
            this.repUserCredits = repUserCredits;
            this.repUserChange = repUserChange;
            this.repUserChangeLogOut = repUserChangeL;
            this.repUserChangeLogIn = repUserChangeLI;
            this.repUserText = repUserText;
            this.repPrijmoveDoklady = repPrijmoveDoklady;
            this.repUserCreditsTimeLog = repUserCreditsTimeLog;
        }

        public IList<LekceUser> GetByLekce(int id, bool inc = true)
        {
            IList<LekceUser> data;
            Lekce lek = null;
            if (inc)
            {
                data = repLekceUser.Table.Where(d => d.LekceId == id).Include(d => d.User).ToList();

            }
            else
            {
                data = repLekceUser.Table.Where(d => d.LekceId == id).ToList();
            }

            foreach (var lu in data)
            {
                if (lu.ZbytekTyp == "")
                {
                    if (lek == null)
                    {
                        lek = repLekce.Table.Where(d => d.Id == id).FirstOrDefault();
                    }
                    if (lek != null)
                    {
                        UserZbytek zbytek = GetZbytekUzivatele(lu.UserId);
                        zbytek.SetZbytekToActualLekceUser(lu, lek);
                        repLekceUser.Update(lu);
                    }
                }
            }

            return data;
        }

        public LekceUser GetById(int id, bool inc = true)
        {
            LekceUser data;

            if (inc)
            {
                data = repLekceUser.Table.Where(d => d.Id == id).Include(d => d.User).FirstOrDefault();
            }
            else
            {
                data = repLekceUser.Table.Where(d => d.Id == id).FirstOrDefault();
            }
            return data;
        }

        public LekceUser GetBy(int userId, int lekceId, int kontCisloId)
        {
            var data = repLekceUser.Table.Where(d => d.LekceId == lekceId && d.UserId == userId && d.KontCislo == kontCisloId).FirstOrDefault();
            return data;
        }

        public void Delete(LekceUser obj)
        {
            repLekceUser.Delete(obj);
        }

        public void Insert(LekceUser obj)
        {
            repLekceUser.Insert(obj);
        }

        public void Update(LekceUser obj)
        {
            repLekceUser.Update(obj);
        }

        public void Update(IEnumerable<LekceUser> objs)
        {
            foreach (var obj in objs)
            {
                repLekceUser.Update(obj);
            }

        }

        public List<KeyValuePair<string, int>> GetTrzbyRok(int poklId, int rok)
        {

            List<KeyValuePair<string, int>> kp = new List<KeyValuePair<string, int>>();


            var data = repTransUser.Table.Where(d => d.PokladnaId == poklId && d.Datum.Year == rok && d.Status == "+").Select(d => new { Datum = d.Datum, sum = d.Castka.Value }).ToList();

            var query = from pro in data
                        group pro by pro.Datum.ToString("yyyy-MM") into g
                        select new
                        {
                            date = g.Key,
                            suma = g.Sum(d => d.sum)
                        };

            foreach (var o in query)
            {
                kp.Add(new KeyValuePair<string, int>(o.date, o.suma));
            }

            return kp;

        }

        public List<KeyValuePair<string, int>> GetTrzbyMesic(int poklId, int rok, int mes)
        {

            List<KeyValuePair<string, int>> kp = new List<KeyValuePair<string, int>>();


            var data = repTransUser.Table.Where(d => d.PokladnaId == poklId && d.Datum.Year == rok && d.Datum.Month == mes && d.Status == "+").Select(d => new { Datum = d.Datum, sum = d.Castka.Value }).ToList();

            var query = from pro in data
                        group pro by pro.Datum.ToString("yyyy-MM-dd") into g
                        select new
                        {
                            date = g.Key,
                            suma = g.Sum(d => d.sum)
                        };

            foreach (var o in query)
            {
                kp.Add(new KeyValuePair<string, int>(o.date, o.suma));
            }

            return kp;

        }

        public List<KeyValuePair<string, int>> GetTrzbyDen(int poklId, DateTime dt)
        {

            List<KeyValuePair<string, int>> kp = new List<KeyValuePair<string, int>>();


            var data = repTransUser.Table.Where(d => d.PokladnaId == poklId && d.Datum.Date == dt.Date && d.Status == "+").Select(d => new { Datum = d.Datum, sum = d.Castka.Value }).ToList();

            var query = from pro in data
                        group pro by pro.Datum.ToString("yyyy-MM-dd") into g
                        select new
                        {
                            date = g.Key,
                            suma = g.Sum(d => d.sum)
                        };

            foreach (var o in query)
            {
                kp.Add(new KeyValuePair<string, int>(o.date, o.suma));
            }

            return kp;

        }

        public IList<Zakaznik> GetByCountOrder(DateTime date)
        {
            var unix = ((DateTimeOffset)date).ToUnixTimeSeconds();

            string sql = $@"SELECT o.userID, COUNT(o.spinID) AS pocet
                FROM spinuser o
                JOIN spinhod s ON s.ID = o.spinID
                WHERE o.unix > {unix}
                AND s.zauctovano = 1
                GROUP BY o.userID
                ORDER BY COUNT(o.spinID) DESC";

            var data2 = (from lu in repLekceUser.Table
                         join le in repLekce.TableUntracked.Where(d => d.Datum > date && d.Zauctovano == true) on lu.LekceId equals le.Id
                         select new { a = lu.UserId, b = lu.LekceId }).AsEnumerable().GroupBy(d => d.a).ToDictionary(e => e.Key, e => e.ToList().Count());


            List<Zakaznik> list = new List<Zakaznik>();
            if (data2.Any())
            {
                var zaks = repUser.TableUntracked.Where(d => data2.Keys.Any(d2 => d2 == d.Id)).ToLookup(d => d.Id);

                foreach (var aa in data2.OrderByDescending(d => d.Value))
                {
                    if (aa.Value > 2)
                    {
                        User8 us = zaks[aa.Key].FirstOrDefault();
                        if (us != null)
                        {
                            Zakaznik zak = new Zakaznik(us);

                            zak.PocetLekci = aa.Value;
                            list.Add(zak);
                        }
                    }
                }
            }

            return list;
        }

        public void ClearObjednaneLekceUzivatele(int id)
        {
            string cacheItemKey = $"ObjednaneLekceUzivatele-{id}";
            cache.ClearDataSub(cacheItemKey);
        }

        public IList<LekceUser> GetObjednaneLekceUzivatele(int id)
        {
            string cacheItemKey = $"ObjednaneLekceUzivatele-{id}";
            var list = cache.GetDataSub<IList<LekceUser>>(cacheItemKey);

            if (list == null)
            {
                list = repLekceUser.Table.Where(d => d.UserId == id && d.Lekce.Zauctovano == false).Include(d => d.Lekce).ToList();
                cache.SetDataSub(cacheItemKey, list, 1);
            }

            return list;
        }

        public void ClearUskutecneneLekceUzivatele(int id)
        {
            string cacheItemKey = $"UskutecneneLekceUzivatele-{id}";
            cache.ClearDataSub(cacheItemKey);
        }

        public IList<LekceUser> GetUskutecneneLekceUzivatele(int id)
        {
            string cacheItemKey = $"UskutecneneLekceUzivatele-{id}";
            var list = cache.GetDataSub<IList<LekceUser>>(cacheItemKey);

            if (list == null)
            {
                list = repLekceUser.Table.Where(d => d.UserId == id && d.Lekce.Zauctovano == true).Include(d => d.Lekce).ToList();
                cache.SetDataSub(cacheItemKey, list, 1);
            }

            return list;
        }

        public void ClearZbytekVObjednavkachUzivatele(int id)
        {
            ClearObjednaneLekceUzivatele(id);
            var objednavky = GetObjednaneLekceUzivatele(id);
            foreach (var obj in objednavky)
            {
                obj.Zbytek = 0;
                obj.ZbytekTyp = "";
                obj.ZbyvaDni = 0;
                Update(obj);
            }
        }

        public void ClearHistorieRoky(int id)
        {
            string cacheItemKey = $"HistorieRoky-{id}";
            cache.ClearDataSub(cacheItemKey);
        }
        public IList<int> GetHistorieRoky(int id)
        {
            string cacheItemKey = $"HistorieRoky-{id}";
            var list = cache.GetDataSub<IList<int>>(cacheItemKey);

            if (list == null)
            {
                list = repTransUser.Table.Where(d => d.UserId == id && d.Status == "+").Select(d => d.Datum.Year).ToList();
                cache.SetDataSub(cacheItemKey, list, 1);
            }

            return list;
        }

        public void ClearHistoriTransakci(int id)
        {
            string cacheItemKey = $"HistorieUzivatele-{id}";
            cache.ClearDataSub(cacheItemKey);
        }

        public IList<UserTransakce> GetHistoriTransakci(int id, bool fromCache = true)
        {
            string cacheItemKey = $"HistorieUzivatele-{id}";
            IList<UserTransakce> list = null;
            if (fromCache)
            {
                list = cache.GetDataSub<IList<UserTransakce>>(cacheItemKey);
            }
            if (list == null)
            {
                list = repTransUser.Table.Where(d => d.UserId == id).ToList();
                cache.SetDataSub(cacheItemKey, list, 1);
            }
            return list;
        }

        public void ClearZbytekUzivatele(int id)
        {
            string cacheItemKey = $"ZbytekUzivatele-{id}";
            cache.ClearDataSub(cacheItemKey);
        }
        public UserZbytek GetZbytekUzivatele(int id, bool fromCache = true)
        {
            string cacheItemKey = $"ZbytekUzivatele-{id}";
            UserZbytek zb = null;

            if (fromCache)
            {
                zb = cache.GetDataSub<UserZbytek>(cacheItemKey);
            }

            if (zb == null)
            {
                zb = new UserZbytek();
                zb.Kredity = repUserCredits.Table.Where(d => d.UserId == id).ToDictionary(d => d.PokladnaId, d => d.Kredit);
                zb.KredityCas = repUserCreditsTime.Table.Where(d => d.UserId == id && d.Platny == true && d.Kredit > 0).OrderBy(d => d.PlatnostOdUnix).ToList();
                cache.SetDataSub(cacheItemKey, zb, 1);
            }
            return zb;
        }

        public bool AddUser(LekceUser le)
        {
            bool ret = false;

            do
            {
                if (!(le.UserId > 0 && le.LekceId > 0))
                {
                    break;
                }

            } while (false);

            return ret;
        }


        public int GetRandom(IList<LekceUser> obj)
        {
            bool ok = false;
            int kontCislo = 0;
            do
            {
                Random rnd = new Random();
                kontCislo = rnd.Next(1, 999);

                ok = !(obj.Any(d => d.KontCislo == kontCislo));


            } while (ok == false);

            return kontCislo;
        }

        #region UserLekceChange

        public void AddUserChange(UserLekceChange ch)
        {
            repUserChange.Insert(ch);
        }

        public IEnumerable<UserLekceChange> GetChangesByLesson(int id)
        {
            return repUserChange.Table.Where(d => d.LekceId == id).Include(d => d.User).ToList();
        }

        public void AddUserChangeLog(UserLekceLogOut chl)
        {
            repUserChangeLogOut.Insert(chl);
        }

        public IPagedList<UserLekceLogOut> GetLogOutAll(int page, int size)
        {
            var query = repUserChangeLogOut.TableUntracked.OrderByDescending(d => d.Id);
            var result = new PagedList<UserLekceLogOut>(query, page, size);
            return result;
        }


        public void AddUserChangeLogIn(UserLekceLogIn chl)
        {
            repUserChangeLogIn.Insert(chl);
        }


        public IPagedList<UserLekceLogIn> GetLogInAll(int page, int size)
        {
            var query = repUserChangeLogIn.TableUntracked.OrderByDescending(d => d.Id);
            var result = new PagedList<UserLekceLogIn>(query, page, size);
            return result;
        }

        #endregion

        public int GetPocetObjednavekUzivatele(int id)
        {
            int count = repLekceUser.Table.Where(d => d.UserId == id).Select(d => d.Id).Count();

            return count;
        }

        public void Insert(UserText obj)
        {
            repUserText.Insert(obj);
        }

        public void Delete(UserText obj)
        {
            repUserText.Delete(obj);
        }

        public IList<UserText> GetUserTextByLesson(int userId, int LekceId)
        {
            return repUserText.Table.Where(d => d.LekceId == LekceId && d.UserId == userId).ToList();
        }

        public IList<UserText> GetUserTextByLesson(int LekceId)
        {
            return repUserText.Table.Where(d => d.LekceId == LekceId).ToList();
        }

        public UserText GetUserTextByLessonNumber(int userId, int LekceId, int konCislo)
        {
            return repUserText.Table.Where(d => d.LekceId == LekceId && d.UserId == userId && d.KontrolniCislo == konCislo).FirstOrDefault();
        }

        public IList<LekceUser> GetByDay(DateTime dt, int pobId)
        {
            var data = (from lu in repLekceUser.Table
                        join l in repLekce.Table on lu.LekceId equals l.Id
                        join u in repUser.Table on lu.UserId equals u.Id
                        where l.Datum.Date == dt.Date && l.PobockaId == pobId

                        select lu).Include(d => d.User).ToList();

            return data;


        }

        public void ApplyTreansaction(UserTransakce t)
        {
            repTransUser.Insert(t);
        }

        public void InsertTransactionStandard(UserTransakce t)
        {

            ZauctujDoklad(t);

            t.Timestamp = DateTime.Now;
            t.Typ = "k";
            repTransUser.Insert(t);

            int unix = DateTimeExtensions.ToUnix(DateTime.Now);

            var zb = repUserCredits.Table.Where(d => d.UserId == t.UserId && d.PokladnaId == t.PokladnaId).FirstOrDefault();
            if (zb != null)
            {
                zb.Kredit = t.Zbytek;
                zb.KreditUnixTime = unix;
                repUserCredits.Update(zb);
            }
            else
            {
                repUserCredits.Insert(new UserZbytekKredit() { PokladnaId = t.PokladnaId, Kredit = t.Zbytek, UserId = t.UserId, KreditUnixTime = unix });
            }
        }

        public void InsertTransactionTime(UserTransakce t)
        {
            ZauctujDoklad(t);

            t.Timestamp = DateTime.Now;
            t.Typ = "kc";
            repTransUser.Insert(t);
        }

        public UserTransakce GetTransakciById(int id, bool includeOthers)
        {
            return repTransUser.Table.Where(d => d.Id == id).Include(d => d.User).FirstOrDefault();
        }

        private void ZauctujDoklad(UserTransakce t)
        {

            if (t.DoPokladny == 1 || t.ZBanky == true)
            {
                var pd = GetValuPrijmoveDoklady(t.Datum.Year);
                if (pd == null)
                {
                    pd = new PrijmoveDoklady() { Rok = (ushort)t.Datum.Year, Hodnota = 1 };
                    Insert(pd);
                }
                else
                {
                    pd.Hodnota++;
                    Update(pd);
                }
                t.Doklad = pd.Hodnota;
            }

        }

        public void ReturnKreditCas(UserTransakce tran)
        {



        }

        public bool RemoveKredit(UserTransakce tran, int inc, bool jitDoMinusu)
        {
            bool zaucovano = true;

            int zbyvaZauctovat = tran.Kredit;
            int utraciSe = 0;

            do
            {
                TypTransakceKreditu tk = TypTransakceKreditu.k;

                ClearZbytekUzivatele(tran.UserId);
                UserZbytek zbytek = GetZbytekUzivatele(tran.UserId, false);

                var aktivniCasove = zbytek.KredityCas.Where(d => d.PlatnostDoUnix > tran.UnixTime && d.PlatnostOdUnix < tran.UnixTime && d.Kredit > 0);
                bool maKreditCasovy = aktivniCasove.Count() > 0;

                int zbKr = 0;
                if (zbytek.Kredity.ContainsKey(tran.PokladnaId))
                {
                    zbKr = zbytek.Kredity[tran.PokladnaId];
                }


                if (zbKr >= zbyvaZauctovat)
                {
                    tk = TypTransakceKreditu.k;
                    UserTransakce tran1 = tran.Clone() as UserTransakce;
                    utraciSe = zbyvaZauctovat;
                    tran1.Kredit = utraciSe;
                    tran1.Zbytek = zbKr - zbyvaZauctovat;
                    tran1.ZbyvaDni = 0;
                    tran1.Increment = ++inc;
                    InsertTransactionStandard(tran1);
                    zbyvaZauctovat = 0;
                    continue;
                }
                else
                {
                    if (maKreditCasovy)
                    {
                        if (zbKr > 0)
                        {
                            // odeberu trochu -- zbytek z csoveho
                            UserTransakce tran1 = tran.Clone() as UserTransakce;
                            tk = TypTransakceKreditu.k;
                            utraciSe = zbKr;
                            tran1.Kredit = utraciSe;
                            tran1.Zbytek = zbKr - utraciSe;
                            tran1.ZbyvaDni = 0;
                            tran1.Increment = ++inc;
                            InsertTransactionStandard(tran1);
                            zbyvaZauctovat -= utraciSe;
                        }
                    }
                    else
                    {
                        if (jitDoMinusu == false)
                        {
                            if (zbKr < zbyvaZauctovat)
                            {
                                // nemuzu jit do minusu
                                zaucovano = false;
                                break;
                            }
                        }

                        UserTransakce tran1 = tran.Clone() as UserTransakce;
                        tk = TypTransakceKreditu.k;
                        utraciSe = zbyvaZauctovat;
                        tran1.Kredit = utraciSe;
                        tran1.Zbytek = zbKr - zbyvaZauctovat;
                        tran1.ZbyvaDni = 0;
                        tran1.Increment = ++inc;
                        InsertTransactionStandard(tran1);
                        zbyvaZauctovat = 0;
                        continue;
                    }
                }

                UserZbytekKreditCas zbkc = aktivniCasove.OrderByDescending(d => d.Aktivni).ThenBy(d => d.PlatnostDoUnix).FirstOrDefault();

                tk = TypTransakceKreditu.kc;

                if (!zbkc.Aktivni)
                {
                    AktivujKreditCasovy(zbkc, tran);
                }

                if (zbkc.PlatnostUnixBreak.HasValue && zbkc.PlatnostUnixBreak.Value > 0) {

                    SpustiKreditCasovy(zbkc, tran);
                }

                if (zbkc.Kredit >= zbyvaZauctovat)
                {
                    UserTransakce tran1 = tran.Clone() as UserTransakce;
                    utraciSe = zbyvaZauctovat;
                    tran1.Kredit = utraciSe;
                    tran1.Zbytek = zbkc.Kredit - utraciSe;
                    tran1.ZbyvaDni = (zbkc.PlatnostDoUnix - tran.UnixTime) / (60 * 60 * 24);
                    tran1.Increment = ++inc;
                    zbkc.Kredit -= utraciSe;
                    InsertTransactionTime(tran1);
                    repUserCreditsTime.Update(zbkc);
                    zbyvaZauctovat = 0;
                    break;
                }
                else
                {
                    if (zbkc.Kredit > 0)
                    {
                        UserTransakce tran1 = tran.Clone() as UserTransakce;
                        utraciSe = zbkc.Kredit;
                        tran1.Kredit = utraciSe;
                        tran1.Zbytek = zbkc.Kredit - utraciSe;
                        tran1.ZbyvaDni = 0;
                        tran1.Increment = ++inc;
                        zbkc.Kredit -= utraciSe;
                        InsertTransactionTime(tran1);
                        zbyvaZauctovat -= utraciSe;
                        repUserCreditsTime.Update(zbkc);
                    }
                    else
                    {
                        break;
                    }
                }

            } while (zbyvaZauctovat > 0);

            return zaucovano;

        }



        public void AddKredit(UserTransakce tran)
        {

            ClearZbytekUzivatele(tran.UserId);
            UserZbytek zbytek = GetZbytekUzivatele(tran.UserId);

            var zb = zbytek.KredityItem(tran.PokladnaId);

            tran.Zbytek = zb + tran.Kredit;


            InsertTransactionStandard(tran);
        }

        /// <summary>
        /// Odebere kredity při převodu
        /// </summary>
        /// <param name="tran"></param>
        public void RemoveKredit(UserTransakce tran)
        {

            ClearZbytekUzivatele(tran.UserId);
            UserZbytek zbytek = GetZbytekUzivatele(tran.UserId);

            var zb = zbytek.KredityItem(tran.PokladnaId);

            tran.Zbytek = zb - tran.Kredit;


            InsertTransactionStandard(tran);

        }


        public void AddKreditTime(UserTransakce tran)
        {
            InsertTransactionTime(tran);

            var pl = tran.PlatbaTime;
            UserZbytekKreditCas zbkc = new UserZbytekKreditCas()
            {
                Kredit = pl.Kredity,
                PlatbaId = tran.PlatbaId,
                Aktivni = false,
                PocetLidi = pl.PocetLidi,
                PocetMesicu = pl.PocetMesicu,
                UserId = tran.UserId,
                PokladnaId = tran.PokladnaId,
                KreditUnixTime = DateTimeExtensions.ToUnix(DateTime.Now),
                Platny = true,
            };

            if (tran.PlatnostOd.HasValue)
            {
                zbkc.PlatnostOdUnix = DateTimeExtensions.ToUnix(tran.PlatnostOd.Value);
                zbkc.PlatnostDoUnix = DateTimeExtensions.ToUnix(tran.PlatnostDo.Value);
                zbkc.Aktivni = true;
            }
            else
            {
                zbkc.PlatnostOdUnix = 1204153200;
                zbkc.PlatnostDoUnix = 2051218740;
            }

            repUserCreditsTime.Insert(zbkc);
        }

        public UserZbytekKreditCas GetUserCreditsTimeById(int id)
        {
            return repUserCreditsTime.Table.Where(d => d.Id == id).FirstOrDefault();
        }

        public bool ReturnKredit(int typPlatbyId, UserTransakce t)
        {
            bool ret = false;
            var kr = GetUserCreditsTimeById(typPlatbyId);
            if (kr != null)
            {

                Delete(kr);
                ret = true;

                InsertTransactionTime(t);
            }
            return ret;
        }

        public void Delete(UserZbytekKreditCas pla)
        {
            repUserCreditsTime.Delete(pla);
        }

        public void Update(UserZbytekKreditCas pla)
        {
            repUserCreditsTime.Update(pla);
        }

        public void AddPausal(UserTransakce tran)
        {
            throw new NotImplementedException();
        }

        private void AktivujKreditCasovy(UserZbytekKreditCas zbkc, UserTransakce tran)
        {

            zbkc.Aktivni = true;
            zbkc.PlatnostOdUnix = tran.UnixTime - (60 * 60 * 3);
            zbkc.PlatnostDoUnix = tran.UnixTime + ((60 * 60 * 24 * 30) * zbkc.PocetMesicu);
            repUserCreditsTime.Update(zbkc);

        }


        private void SpustiKreditCasovy(UserZbytekKreditCas zbkc, UserTransakce tran)
        {
            // 

            int prodlouzitO = DateTimeExtensions.ToUnix(DateTime.Now) - zbkc.PlatnostUnixBreak.Value;

            zbkc.PlatnostDoUnix = prodlouzitO + zbkc.PlatnostDoUnix;

            zbkc.PlatnostUnixBreak = null;

            repUserCreditsTime.Update(zbkc);
        }


        #region PrijmoveDokaldy
        public PrijmoveDoklady GetValuPrijmoveDoklady(int year)
        {
            var pd = repPrijmoveDoklady.Table.Where(d => d.Rok == year).FirstOrDefault();
            return pd;
        }

        public void Insert(PrijmoveDoklady pd)
        {
            repPrijmoveDoklady.Insert(pd);
        }

        public void Update(PrijmoveDoklady pd)
        {
            repPrijmoveDoklady.Update(pd);
        }

        public void Insert(UserZbytekKreditCasLog log)
        {
            repUserCreditsTimeLog.Insert(log);
        }

        public IEnumerable<UserZbytekKreditCasLog> GetLogsByPlatbaId(int plaId)
        {
            return repUserCreditsTimeLog.Table.Where(d => d.PlatbaId == plaId);
        }


        public IEnumerable<UserZbytekKreditCasLog> GetUpFromDate(int from)
        {
            return repUserCreditsTimeLog.Table.Where(d => d.UnixFrom == from);
        }



        #endregion
    }
}
