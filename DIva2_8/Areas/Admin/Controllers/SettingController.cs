using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Diva2.Core.Extensions;
using Diva2.Core.Main.Calendar;
using Diva2.Core.Main.Lektori;
using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Main;
using Diva2.Core.Main.Platby;
using Diva2.Core.Main.Users;
using Diva2.Core.Model.Json;
using Diva2.Data;
using Diva2.Services;
using Diva2.Services.Managers.Customers;
using Diva2.Services.Managers.Mains;
using Diva2.Services.Managers.Platby;
using Diva2.Services.Managers.Pobocky;
using Diva2.Services.Managers.Setting;
using Diva2.Services.Managers.Users;
using Diva2Web.Models;
using Diva2Web.Models.Admin;
using Diva2Web.Models.Helpers;
using Diva2Web.Models.Inis;
using Diva2Web.Models.Lekces;
using Diva2Web.Models.Platby;
using Diva2Web.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Diva2Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeActionFilterAttribute]
    public class SettingController : BaseAdminController
    {


        private CultureInfo ci = CultureInfo.InvariantCulture;

        public SettingController(ApplicationDbContext dbContext,
           IMemoryCache memoryCache, ILogger<HomeController> logger,
           IHttpContextAccessor httpContextAccessor,
           ISkupinaZakaznikaService skupZakServ, IPobockaService pobSer, IUser8Service userSer, IObjednavkyService objSer,
           ILekceTypService ltService, ILekceMustrService lmuService, ILekceService lekceService, ILogs8Service logSer,
            ILektorService lkService, IPlatbaService plService) : base(dbContext, httpContextAccessor, memoryCache, userSer, pobSer, logSer, objSer)
        {
            lektorServ = lkService;
            lekceMustrServ = lmuService;
            lekceTypServ = ltService;
            skupinaZakaznikaServ = skupZakServ;
            lekceServ = lekceService;
            platbaServ = plService;

            cache = new CacheHelper(memoryCache, dbContext.SubDomain);
        }

        // GET: Setting
        public ActionResult Index()
        {
            aa.Title = "Nastavení";
            SetMainPageValues();

            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }
            if (!aa.User.HasRule("spinstat_view"))
            {
                return Redirect("/Admin/Home/NoRule");
            }


            return View(aa);
        }

        #region Mustr

        public ActionResult Mustr()
        {
            aa.Title = "Hlavní šablona";

            SetMainPageValues();
            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }
            if (!aa.User.HasRule("spinstat_view"))
            {
                return Redirect("/Admin/Home/NoRule");
            }

            List<LekceMustr> lekce = lekceMustrServ.GetAll().Where(d => d.PobockaId == aa.Pobocka.Id).ToList();
            // poked neni nic vygeneruju
            if (lekce.Count == 0)
            {
                for (int den = 1; den < 8; den++)
                {
                    foreach (var hod in aa.IniMinutes.Where(d => d.PobockaId == aa.Pobocka.Id && d.Visible == true))
                    {
                        var le = new LekceMustr()
                        {
                            Hodina = 0,
                            Cas = hod.Time.ToString(@"hh\:mm"),
                            MinutaKey = hod.Minute,
                            Aktivni = false,
                            Den = den,
                            Kredit = aa.Pobocka.Kredity,
                            Lektor = 1,
                            Minuty = aa.Pobocka.Minuty,
                            PocetMist = aa.Pobocka.PocetMist,
                            PocetNahradniku = 1,
                            Lektor2 = 0,
                            PobockaId = aa.Pobocka.Id,
                            Typ = 0,
                            Zdroj = 1
                        };
                        try{
                        lekceMustrServ.Insert(le);
                        }
                        catch(Exception ex){ 
                        }
                    }
                }
                lekce = lekceMustrServ.GetAll().Where(d => d.PobockaId == aa.Pobocka.Id).ToList();
            }


            aa.Lektori = lektorServ.GetAll();
            aa.TypyLekci = lekceTypServ.GetAll();

            

            aa.LekceMustr = new List<LekceMustrModel>();

            var casy = pobocka.CalendarSettingObj.GetMinutes();
            List<LekceMustr> forUpdate = new List<LekceMustr>();
            foreach (var l in lekce)
            {
                LekceMustrModel lmm = new LekceMustrModel();
                lmm.CopyFromDb(l);

                if (!casy.Contains(l.MinutaKey))
                {

                    var maxMin = casy.Where(d => d < l.MinutaKey).Max();
                    l.MinutaKey = maxMin;
                    forUpdate.Add(l);
                }


                var lek = aa.Lektori.Where(d => d.Id == l.Lektor).FirstOrDefault();

                lmm.LektorString = (lek != null) ? lek.Nick : "!Nezadáno!";
                lmm.Lektor = (lek != null) ? lek.Id : 0;

                if (l.Lektor2 > 0)
                {
                    lmm.Lektor2String = (lek != null) ? lek.Nick : "!Nezadáno!";
                    lmm.Lektor2 = (lek != null) ? lek.Id : 0;
                }

                if (l.Typ > 0)
                {
                    var typ = aa.TypyLekci.Where(d => d.Id == l.Typ).FirstOrDefault();
                    lmm.TypString = (typ != null) ? typ.NazevAdmin : "!Nezadáno!";
                    lmm.Typ = (typ != null) ? typ.Id : 0;
                }
                aa.LekceMustr.Add(lmm);
            }

            if (forUpdate.Count > 0)
            {

                lekceMustrServ.Update(forUpdate);
            }

            aa.LekceMustrTypy = lekceMustrServ.GetTypAll().Where(d => d.PobockaId == pobocka.Id && d.Aktivni == true).ToList();

            return View(aa);
        }

        public ActionResult MustrEdit(int id)
        {
            LekceMustrModel model = new LekceMustrModel();

            if (id > 0)
            {
                var mu = lekceMustrServ.GetById(id);
                if (mu != null)
                {
                    model.CopyFromDb(mu);
                    var leks = lektorServ.GetAll();
                    foreach (var le in leks)
                    {
                        model.Lektori.Add(new ListItem() { Id = le.Id, Text = le.Nick });
                    }
                    var typs = lekceTypServ.GetAll();
                    model.Typy.Add(new ListItem() { Id = 0, Text = "Nezadáno" });
                    foreach (var t in typs)
                    {
                        model.Typy.Add(new ListItem() { Id = t.Id, Text = t.NazevAdmin });
                    }
                }
            }

            return PartialView("MustrEdit", model);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MustrEdit(LekceMustrModel m)
        {

            if (ModelState.IsValid)
            {
                SetSessions();

                do
                {
                    var db = lekceMustrServ.GetById(m.Id);
                    if (db != null)
                    {
                        m.MinutaKey = db.MinutaKey;

                        if (aa.Pobocka.Typ == 3)
                        {
                            TimeSpan tsFrom = new TimeSpan(0, m.MinutaKey, 0);
                            TimeSpan tsTo = new TimeSpan(0, m.MinutaKey + aa.Pobocka.CalendarSettingObj.Minutes, 0);
                            DateTime dateTime = DateTime.ParseExact(m.Cas, "HH:mm", CultureInfo.InvariantCulture);
                            int total = (dateTime.Hour * 60) + dateTime.Minute;

                            if (!(tsFrom.TotalMinutes <= total && total < tsTo.TotalMinutes))
                            {
                                string tsfrom = tsFrom.ToString(@"hh\:mm");
                                string tsto = tsTo.ToString(@"hh\:mm");
                                ModelState.AddModelError(string.Empty, $"Čas musí být v rozmezí >= {tsfrom} a menší než {tsto}.");
                                continue;
                            }
                        }
                        m.CopyToDb(db);
                        lekceMustrServ.Update(db);
                    }

                } while (false);

            }
            return PartialView("MustrEdit", m);
        }

        public ActionResult MustrFromSab(int? id, int? sabId)
        {
            var resp = new JsonStatus();

            do
            {
                if (!id.HasValue)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Neexistuje ID" });
                    continue;
                }
                LekceMustr mu = lekceMustrServ.GetById(id.Value);
                if (mu == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Neexistuje mustr" });
                    continue;
                }

                var sab = lekceMustrServ.GetTypById(sabId.Value);
                if (sab == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nepodařilo se načíst šablonu lekce" });
                    continue;
                }

                mu.CopyFromSablona(sab);
                lekceMustrServ.Update(mu);

                resp.Status = true;

            } while (false);

            return Json(resp);

        }

        // vytvori z sablony
        public ActionResult MustrFromSabCreate(int? min, int? day, int? sabId, int? dr)
        {
            var resp = new JsonStatus();

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                if (!min.HasValue)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Neexistuje min" });
                    continue;
                }

                if (!day.HasValue)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Neexistuje den" });
                    continue;
                }

                var sab = lekceMustrServ.GetTypById(sabId.Value);
                if (sab == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nepodařilo se načíst šablonu lekce" });
                    continue;
                }

                LekceMustr mu = new LekceMustr();

                mu.CopyFromSablona(sab);
                mu.MinutaKey = min.Value;
                mu.Den = day.Value;
                mu.PobockaId = aa.Pobocka.Id;
                mu.Zdroj = dr.Value;
                mu.Hodina = 0; //fake
                TimeSpan ts = new TimeSpan(0, min.Value, 0);
                mu.Cas = ts.ToString(@"hh\:mm");
                mu.Aktivni = true;

                lekceMustrServ.Insert(mu);

                resp.Status = true;

            } while (false);

            return Json(resp);

        }

        public ActionResult MustrCreate(int min, int day, int dr = 1)
        {

            TrySetUserFromSess(aa.User);
            SetSessions();

            LekceMustrModel m = new LekceMustrModel() { Den = day, MinutaKey = min, Zdroj = dr, PobockaId = aa.Pobocka.Id, Minuty = 60, Kredit = 10, Aktivni = true, PocetMist = aa.Pobocka.PocetMist };
            TimeSpan ts = new TimeSpan(0, min, 0);
            m.Cas = ts.ToString(@"hh\:mm");
            var leks = lektorServ.GetAll();
            foreach (var le in leks)
            {
                m.Lektori.Add(new ListItem() { Id = le.Id, Text = le.Nick });
            }
            var typs = lekceTypServ.GetAll();
            m.Typy.Add(new ListItem() { Id = 0, Text = "Nezadáno" });
            foreach (var t in typs)
            {
                m.Typy.Add(new ListItem() { Id = t.Id, Text = t.Nazev });
            }

            return PartialView("MustrCreate", m);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MustrCreate(LekceMustrModel m)
        {

            if (ModelState.IsValid)
            {
                TrySetUserFromSess(aa.User);
                SetSessions();

                do
                {

                    LekceMustr db = new LekceMustr();
                    m.CopyToDb(db);
                    db.PobockaId = m.PobockaId;
                    db.Zdroj = m.Zdroj;
                    db.MinutaKey = m.MinutaKey;
                    db.Den = m.Den;

                    TimeSpan tsFrom = new TimeSpan(0, m.MinutaKey, 0);
                    TimeSpan tsTo = new TimeSpan(0, m.MinutaKey + aa.Pobocka.CalendarSettingObj.Minutes, 0);
                    DateTime dateTime = DateTime.ParseExact(m.Cas, "HH:mm", CultureInfo.InvariantCulture);
                    int total = (dateTime.Hour * 60) + dateTime.Minute;

                    if (!(tsFrom.TotalMinutes <= total && total < tsTo.TotalMinutes))
                    {
                        string tsfrom = tsFrom.ToString(@"hh\:mm");
                        string tsto = tsTo.ToString(@"hh\:mm");
                        ModelState.AddModelError(string.Empty, $"Čas musí být v rozmezí >= {tsfrom} a menší než {tsto}.");
                        continue;
                    }

                    lekceMustrServ.Insert(db);

                } while (false);

            }
            return PartialView("MustrCreate", m);
        }

        #endregion

        #region WeakCopy
        public ActionResult WeakCopy()
        {
            aa.Title = "Týdenní kopie";
            SetMainPageValues();
            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }
            if (!aa.User.HasRule("spinstat_view"))
            {
                return Redirect("/Admin/Home/NoRule");
            }

            aa.ShowCalendar = true;

            aa.LekceTyden = lekceServ.GetWeakByDay(aa.Pobocka.Id, aa.CurrentDate);
            aa.TypyLekci = lekceTypServ.GetAll();
            aa.Lektori = lektorServ.GetAll();
            aa.LekceMustrTypy = lekceMustrServ.GetTypAll().Where(d => d.PobockaId == pobocka.Id && d.Aktivni == true).ToList();
            ViewBag.LekceMustr = lekceMustrServ.GetAll().Where(d => d.PobockaId == aa.Pobocka.Id);
            return View(aa);
        }

        public ActionResult WeakCreate(string id)
        {
            var resp = new JsonStatus();
            int count = 0;
            aa.Title = "Hlavní šablona";
            SetMainPageValues();

            var lekce = lekceMustrServ.GetAll().Where(d => d.PobockaId == aa.Pobocka.Id && d.Aktivni == true).ToList();

            if (lekce.Count() > 0)
            {

                aa.Monday = DateTimeExtensions.StartOfWeek(aa.CurrentDate, DayOfWeek.Monday);

                DateTime sunday = aa.Monday.AddDays(7);


                DateTime aktDay = aa.Monday;
                for (int i = 1; i < 8; i++)
                {

                    var den = lekce.Where(d => d.Den == i);
                    if (den != null)
                    {
                        foreach (var le in den.OrderBy(d => d.Hodina))
                        {
                            Lekce l = new Lekce();
                            l.CopyFromMustr(le);
                            l.Rok = sunday.Year;
                            l.Tyden = DateTimeExtensions.GetWeekNumber(aktDay);
                            l.Datum = aktDay;
                            lekceServ.Insert(l);
                            count++;
                        }

                    }
                    aktDay = aktDay.AddDays(1);
                }

                if (count > 0)
                {
                    resp.Messages.Add(new JsonMessage() { Text = $"Vygenerovalo se {count} hodin" });
                    resp.Status = true;
                }
                else
                {
                    resp.Messages.Add(new JsonMessage() { Text = $"Z {lekce.Count()} lekcí se nevygenerovala ani jedna lekce :(" });
                    resp.Status = true;
                }
            }
            else
            {
                resp.Messages.Add(new JsonMessage() { Text = $"Není aktivní žádná lekce" });
                resp.Status = false;
            }

            return Json(resp);

        }

        public ActionResult CreateLesson(string id)
        {
            var resp = new JsonStatus();

            do
            {
                var items = id.Split("-");

                if (items.Count() != 2)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Řetězec se nepodařilo zpracovat" });
                    continue;
                }

                int.TryParse(items[0], out int d);
                int.TryParse(items[1], out int m);

                if (!(d > 0 && m > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Řetězec se nepodařilo zpracovat 2" });
                    continue;
                }

                aa.Title = "Hlavní šablona";
                SetMainPageValues();

                var mustr = lekceMustrServ.GetByParams(aa.Pobocka.Id, d, m, 1);
                if (mustr == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nepodařilo se načíst mustr lekci" });
                    continue;
                }

                var Monday = DateTimeExtensions.StartOfWeek(aa.CurrentDate, DayOfWeek.Monday);
                DateTime sunday = aa.Monday.AddDays(7);

                var day = Monday.AddDays(d - 1);

                var inDay = lekceServ.GetByDay(mustr.PobockaId, day).Where(d => d.CelyDen == true);

                if (inDay.Any())
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Ve dni už je celodenní lekce" });
                    continue;
                }


                Lekce ln = new Lekce();
                ln.CopyFromMustr(mustr);
                ln.Datum = day;
                ln.Rok = sunday.Year;
                ln.Tyden = DateTimeExtensions.GetWeekNumber(day);
                lekceServ.Insert(ln);


                resp.Status = true;

            } while (false);

            return Json(resp);
        }

        public ActionResult CreateLessonFromSab(string id, int? sabId)
        {
            var resp = new JsonStatus();

            do
            {

                if (id == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Řetězec neexistuje" });
                    continue;
                }

                var items = id.Split("-");

                if (items.Count() != 2)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Řetězec se nepodařilo zpracovat" });
                    continue;
                }

                if (!sabId.HasValue)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nesprávná šablona" });
                    continue;
                }


                int.TryParse(items[0], out int d);
                int.TryParse(items[1], out int m);

                if (!(d > 0 && m > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Řetězec se nepodařilo zpracovat 2" });
                    continue;
                }

                aa.Title = "Hlavní šablona";
                SetMainPageValues();


                var sab = lekceMustrServ.GetTypById(sabId.Value);
                if (sab == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nepodařilo se načíst šablonu lekce" });
                    continue;
                }

                var Monday = DateTimeExtensions.StartOfWeek(aa.CurrentDate, DayOfWeek.Monday);
                var day = Monday.AddDays(d - 1);

                Lekce ln = new Lekce();
                ln.CopyFromSablona(sab);
                ln.Datum = day;
                ln.Rok = day.Year;
                ln.Tyden = DateTimeExtensions.GetWeekNumber(day);
                ln.MinutaKey = m;
                ln.Den = d;
                ln.PobockaId = aa.Pobocka.Id;
                ln.Zdroj = 1;
                ln.HodinaPoradi = 0; //fake
                lekceServ.Insert(ln);


                resp.Status = true;

            } while (false);

            return Json(resp);
        }

        public ActionResult DeleteLesson(int id)
        {
            var resp = new JsonStatus();

            if (id > 0)
            {
                var mu = lekceServ.GetById(id);
                if (mu != null)
                {
                    var objs = objServ.GetByLekce(id);
                    if (objs != null && objs.Count > 0)
                    {
                        resp.Messages = new List<JsonMessage>();
                        resp.Messages.Add(new JsonMessage() { Text = "Nelze smazat, v hodině jsou zákazníci" });
                        resp.Type = JsonStatusType.Warning;
                    }
                    else
                    {
                        lekceServ.Delete(mu);
                        resp.Status = true;
                    }
                }
            }

            return Json(resp);
        }

        public ActionResult WeakCopyEdit(int id)
        {
            LekceModel model = new LekceModel();

            if (id > 0)
            {
                var mu = lekceServ.GetById(id);
                if (mu != null)
                {
                    model.CopyFromDb(mu);
                    model.PocetPrihlasenych = objServ.GetByLekce(id).Count;
                    var leks = lektorServ.GetAll();
                    foreach (var le in leks)
                    {
                        model.Lektori.Add(new ListItem() { Id = le.Id, Text = le.Nick });
                    }
                    var typs = lekceTypServ.GetAll();
                    model.Typy.Add(new ListItem() { Id = 0, Text = "Nezadáno" });
                    foreach (var t in typs)
                    {
                        model.Typy.Add(new ListItem() { Id = t.Id, Text = t.NazevAdmin });
                    }
                }
            }

            return PartialView("WeakCopyEdit", model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult WeakCopyEdit(LekceModel m)
        {

            if (ModelState.IsValid)
            {
                SetMainPageValues();

                do
                {
                    var dbObj = lekceServ.GetById(m.Id);
                    if (dbObj != null)
                    {
                        m.MinutaKey = dbObj.MinutaKey;

                        var inDay = lekceServ.GetByDay(dbObj.PobockaId, dbObj.Datum);

                        if (m.CelyDen)
                        {
                            if (inDay.Count() > 1)
                            {
                                ModelState.AddModelError("CelyDen", "Ve dni může být jen jedna celodenní hodina. Smažte ostatní hodiny.");
                                break;
                            }
                            else if (inDay.Count() == 1)
                            {
                                if (inDay.First().Id != dbObj.Id)
                                {
                                    ModelState.AddModelError("CelyDen", "Ve dni může být jen jedna celodenní hodina");
                                    break;
                                }
                            }
                        }

                        if (aa.Pobocka.Typ == 3)
                        {
                            TimeSpan tsFrom = new TimeSpan(0, m.MinutaKey, 0);
                            TimeSpan tsTo = new TimeSpan(0, m.MinutaKey + aa.Pobocka.CalendarSettingObj.Minutes, 0);

                            int total = (m.Cas.Hours * 60) + m.Cas.Minutes;

                            if (!(tsFrom.TotalMinutes <= total && total < tsTo.TotalMinutes))
                            {
                                string tsfrom = tsFrom.ToString(@"hh\:mm");
                                string tsto = tsTo.ToString(@"hh\:mm");
                                ModelState.AddModelError(string.Empty, $"Čas musí být v rozmezí >= {tsfrom} a menší než {tsto}.");
                                continue;
                            }
                        }
                        m.CopyToDbForUpdate(dbObj);
                        lekceServ.Update(dbObj);
                    }
                } while (false);
            }
            return PartialView("WeakCopyEdit", m);
        }

        #endregion

        #region Typ Muster
        public ActionResult MusterTypes()
        {
            aa.Title = "Typy šablon";
            SetMainPageValues();
            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }
            if (!aa.User.HasRule("spinstat_view"))
            {
                return Redirect("/Admin/Home/NoRule");
            }

            aa.TypyLekci = lekceTypServ.GetAll();

            foreach (var typ in aa.TypyLekci) {
                typ.Nazev = $"{typ.PobockaId}-{typ.Nazev}";
            }


            aa.Lektori = lektorServ.GetAll();
            aa.LekceMustrTypy = lekceMustrServ.GetTypAll().Where(d => d.PobockaId == pobocka.Id).ToList();
            return View(aa);
        }

        public IActionResult MusterTypeCreate()
        {
            aa.Title = "Typy lekcí";
            SetSessions();

            var model = new LekceMustrTypModel();
            model.PobockaId = pobocka.Id;
            var leks = lektorServ.GetAll();
            foreach (var le in leks)
            {
                model.Lektori.Add(new ListItem() { Id = le.Id, Text = le.Nick });
            }
            var typs = lekceTypServ.GetAll();
            model.Typy.Add(new ListItem() { Id = 0, Text = "Nezadáno" });
            foreach (var t in typs)
            {
                model.Typy.Add(new ListItem() { Id = t.Id, Text = t.Nazev });
            }
            return PartialView("MusterTypeCreate", model);
        }

        [HttpPost]
        public IActionResult MusterTypeCreate(LekceMustrTypModel model)
        {
            if (ModelState.IsValid)
            {
                LekceMustrTyp le = new LekceMustrTyp();
                model.CopyToDb(le);
                le.Aktivni = true;
                lekceMustrServ.Insert(le);
            }
            else
            {

                var leks = lektorServ.GetAll();
                foreach (var le in leks)
                {
                    model.Lektori.Add(new ListItem() { Id = le.Id, Text = le.Nick });
                }
                var typs = lekceTypServ.GetAll();
                model.Typy.Add(new ListItem() { Id = 0, Text = "Nezadáno" });
                foreach (var t in typs)
                {
                    model.Typy.Add(new ListItem() { Id = t.Id, Text = t.Nazev });
                }

            }
            return PartialView("MusterTypeCreate", model);
        }

        public ActionResult MusterTypeEdit(int id)
        {
            LekceMustrTypModel model = new LekceMustrTypModel();

            if (id > 0)
            {
                var db = lekceMustrServ.GetTypById(id);
                if (db != null)
                {
                    model.CopyFromDb(db);
                    var leks = lektorServ.GetAll();
                    foreach (var le in leks)
                    {
                        model.Lektori.Add(new ListItem() { Id = le.Id, Text = le.Nick });
                    }
                    var typs = lekceTypServ.GetAll();
                    model.Typy.Add(new ListItem() { Id = 0, Text = "Nezadáno" });
                    foreach (var t in typs)
                    {
                        model.Typy.Add(new ListItem() { Id = t.Id, Text = t.Nazev });
                    }
                }
            }

            return PartialView("MusterTypeEdit", model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MusterTypeEdit(LekceMustrTypModel model)
        {

            if (ModelState.IsValid)
            {
                var dbObj = lekceMustrServ.GetTypById(model.Id);
                if (dbObj != null)
                {
                    model.CopyToDb(dbObj);
                    lekceMustrServ.Update(dbObj);
                }
            }
            else
            {

                var leks = lektorServ.GetAll();
                foreach (var le in leks)
                {
                    model.Lektori.Add(new ListItem() { Id = le.Id, Text = le.Nick });
                }
                var typs = lekceTypServ.GetAll();
                model.Typy.Add(new ListItem() { Id = 0, Text = "Nezadáno" });
                foreach (var t in typs)
                {
                    model.Typy.Add(new ListItem() { Id = t.Id, Text = t.Nazev });
                }

            }
            return PartialView("MusterTypeEdit", model);
        }

        #endregion

        public ActionResult Times()
        {
            aa.Title = "Nastavení časů";
            SetMainPageValues();



            return View(aa);
        }

        public ActionResult TimeActive(int pob, int min)
        {
            aa.Title = "Nastavení časů";
            SetMainPageValues();

            JsonStatus resp = new JsonStatus();

            do
            {

                var minute = aa.IniMinutes.Where(d => d.PobockaId == pob && d.Minute == min).FirstOrDefault();

                if (minute != null)
                {
                    string txt = "jako aktivní";
                    if (minute.Visible == false)
                    {
                        minute.Visible = true;
                    }
                    else
                    {
                        minute.Visible = false;
                        txt = "jako NE-aktivní";
                    }
                    pobServ.Update(minute);

                    resp.MsgAddSuccess($" Položka {minute.Time.ToString(@"hh\:mm")} byla nastavena {txt}");


                    resp.Status = true;
                }
                else
                {
                    resp.MsgAddDanger("Nenačetlo se nastavení");
                }

            } while (false);


            return Json(resp);
        }

        #region Mzdy a tržby
        public ActionResult Sales(string id = "d")
        {
            aa.Title = "Tržby";
            SetMainPageValues();
            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }
            if (!aa.User.HasRule("spinstat_view"))
            {
                return Redirect("/Admin/Home/NoRule");
            }

            aa.ShowCalendar = true;

            List<KeyValuePair<string, int>> kp = new List<KeyValuePair<string, int>>();

            if (id == "m")
            {
                kp = objServ.GetTrzbyMesic(aa.Pobocka.PokladnaId, aa.CurrentDate.Year, aa.CurrentDate.Month);
            }
            else if (id == "y")
            {
                kp = objServ.GetTrzbyRok(aa.Pobocka.PokladnaId, aa.CurrentDate.Year);
            }
            else
            {
                kp = objServ.GetTrzbyDen(aa.Pobocka.PokladnaId, aa.CurrentDate);
            }

            ViewBag.Type = id;
            ViewBag.Kp = kp;

            return View(aa);
        }

        public ActionResult Wages(int id = 0, int p = 0, int f = 0)
        {
            aa.Title = "Mzdy";
            SetMainPageValues();
            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }
            if (!aa.User.HasRule("spinstat_view"))
            {
                return Redirect("/Admin/Home/NoRule");
            }
            aa.ShowCalendar = true;

            List<int> aktLekts = lekceServ.GetLektoryMesic(aa.CurrentDate.Year, aa.CurrentDate.Month);
            IEnumerable<Lekce> lekce = null;

            aa.Lektori = lektorServ.GetAll().Where(d => aktLekts.Any(a => a == d.Id)).ToList();
            Lektor le = null;
            if (id > 0)
            {
                le = aa.Lektori.Where(d => d.Id == id).FirstOrDefault();
            }
            aa.Id = 0;
            if (le != null)
            {
                aa.Id = le.Id;
                lekce = lekceServ.GetMzdyMesic(le.Id, aa.CurrentDate.Year, aa.CurrentDate.Month);
            }
            else
            {
                lekce = lekceServ.GetMzdyMesic(aa.CurrentDate.Year, aa.CurrentDate.Month);
            }

            if (p > 0)
            {
                lekce = lekce.Where(d => d.PobockaId == p);
            }

            // future
            if (f == 0)
            {
                lekce = lekce.Where(d => d.Zauctovano == true);
            }

            ViewBag.Mzda = lektorServ.GetMzdyuByPokladna(aa.Pobocka.PokladnaId);


            ViewBag.Lekce = lekce;
            ViewBag.Lektor = le;
            ViewBag.PobockaId = p;
            ViewBag.Future = f == 1;


            return View(aa);
        }


        public ActionResult WageEdit(int id)
        {
            Mzda model = new Mzda();

            aa.Title = "Mzdy";
            SetMainPageValues();


            if (id > 0)
            {
                model = lektorServ.GetMzdyuByPokladna(id);
            }

            return PartialView("WageEdit", model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult WageEdit(Mzda m)
        {

            if (ModelState.IsValid)
            {
                var db = lektorServ.GetMzdyuByPokladna(m.PokladnaId);
                if (db != null)
                {
                    db.Mesic = m.Mesic;
                    db.Hodina = m.Hodina;
                    db.Zakaznik = m.Zakaznik;
                    db.Pro100 = m.Pro100;
                    db.Zak01 = m.Zak01;
                    db.Zak02 = m.Zak02;
                    db.Zak03 = m.Zak03;
                    db.Zak04 = m.Zak04;
                    db.Zak05 = m.Zak05;
                    db.Zak06 = m.Zak06;
                    db.Zak07 = m.Zak07;
                    db.Zak08 = m.Zak08;
                    db.Zak09 = m.Zak09;
                    db.Zak10 = m.Zak10;
                    db.Zak11 = m.Zak11;
                    db.Zak12 = m.Zak12;
                    db.Zak13 = m.Zak13;
                    db.Zak14 = m.Zak14;
                    db.Zak15 = m.Zak15;
                    db.Zak16 = m.Zak16;
                    db.Zak17 = m.Zak17;
                    db.Zak18 = m.Zak18;
                    db.Zak19 = m.Zak19;
                    db.Zak20 = m.Zak20;
                    lektorServ.Update(db);
                }
            }
            return PartialView("WageEdit", m);
        }

        #endregion

        #region Lektor

        public ActionResult Lectors()
        {
            aa.Title = "Lektoři";
            SetMainPageValues();
            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }
            if (!aa.User.HasRule("spinstat_view"))
            {
                return Redirect("/Admin/Home/NoRule");
            }

            aa.Lektori = lektorServ.GetAll();
            return View(aa);
        }

        public IActionResult LectorCreate()
        {
            var model = new LektorModel() { };

            return PartialView("LectorCreate", model);
        }

        [HttpPost]
        public IActionResult LectorCreate(LektorModel model)
        {
            if (ModelState.IsValid)
            {
                Lektor le = new Lektor();
                model.CopyToDb(le);
                lektorServ.Insert(le);
            }

            return PartialView("LectorCreate", model);
        }

        public IActionResult LectorEdit(int id)
        {
            LektorModel le = new LektorModel();
            var lektor = lektorServ.GetById(id);
            if (lektor != null)
            {
                le.CopyFromDb(lektor);
            }

            return PartialView("LectorEdit", le);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LectorEdit(LektorModel model)
        {

            if (ModelState.IsValid)
            {
                var lektor = lektorServ.GetById(model.Id);
                if (lektor != null)
                {

                    model.CopyToDb(lektor);
                    lektorServ.Update(lektor);
                }

            }
            return PartialView("LectorEdit", model);
        }
        #endregion

        #region TypLekce
        public ActionResult LessonsType()
        {
            aa.Title = "Typy lekcí";
            SetMainPageValues();
            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }
            if (!aa.User.HasRule("spinstat_view"))
            {
                return Redirect("/Admin/Home/NoRule");
            }

            aa.TypyLekci = lekceTypServ.GetAll().Where(d => d.PobockaId == pobocka.Id).ToList();
            return View(aa);
        }

        public IActionResult LessonTypeCreate()
        {
            aa.Title = "Typy lekcí";
            SetSessions();

            var model = new LekceTypModel();
            model.PobockaId = pobocka.Id;

            return PartialView("LessonTypeCreate", model);
        }

        [HttpPost]
        public IActionResult LessonTypeCreate(LekceTypModel model)
        {
            if (ModelState.IsValid)
            {
                LekceTyp le = new LekceTyp();
                model.CopyToDb(le);

                lekceTypServ.Insert(le);
            }

            return PartialView("LessonTypeCreate", model);
        }

        public ActionResult LessonTypeEdit(int id)
        {
            LekceTypModel model = new LekceTypModel();

            if (id > 0)
            {
                var le = lekceTypServ.GetById(id);
                if (le != null)
                {
                    model.CopyFromDb(le);
                }
            }

            return PartialView("LessonTypeEdit", model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LessonTypeEdit(LekceTypModel model)
        {

            if (ModelState.IsValid)
            {
                var dbObj = lekceTypServ.GetById(model.Id);
                if (dbObj != null)
                {
                    model.CopyToDb(dbObj);
                    lekceTypServ.Update(dbObj);
                }
            }
            return PartialView("LessonTypeEdit", model);
        }

        #endregion

        #region Skupina zakazniku
        public ActionResult CustomersGroup()
        {
            aa.Title = "Skupina zákazníka";
            SetMainPageValues();
            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }
            if (!aa.User.HasRule("spinstat_view"))
            {
                return Redirect("/Admin/Home/NoRule");
            }

            aa.SkupinyZakaznika = skupinaZakaznikaServ.GetAll();
            return View(aa);
        }

        public IActionResult CustomerGroupCreate()
        {
            SetMainPageValues();

            var model = new SkupinaZakaznikaModel();
            model.PobockaId = pobocka.Id;

            return PartialView("CustomerGroupCreate", model);
        }

        [HttpPost]
        public IActionResult CustomerGroupCreate(SkupinaZakaznikaModel model)
        {
            if (ModelState.IsValid)
            {
                var le = new User8Group();
                model.CopyToDb(le);
                skupinaZakaznikaServ.Insert(le);
            }

            return PartialView("CustomerGroupCreate", model);
        }

        public ActionResult CustomerGroupEdit(int id)
        {
            SkupinaZakaznikaModel model = new SkupinaZakaznikaModel();

            if (id > 0)
            {
                var le = skupinaZakaznikaServ.GetById(id);
                if (le != null)
                {
                    model.CopyFromDb(le);
                }
            }

            return PartialView("CustomerGroupEdit", model);

        }

        [HttpPost]
        public IActionResult CustomerGroupEdit(SkupinaZakaznikaModel model)
        {

            if (ModelState.IsValid)
            {
                var dbObj = skupinaZakaznikaServ.GetById(model.Id);
                if (dbObj != null)
                {
                    model.CopyToDb(dbObj);
                    skupinaZakaznikaServ.Update(dbObj);
                }
            }
            return PartialView("CustomerGroupEdit", model);
        }
        #endregion

        #region Platby
        public ActionResult Payments(int id = 1)
        {
            aa.Title = "Platby";
            SetMainPageValues();
            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }
            if (!aa.User.HasRule("spinstat_view"))
            {
                return Redirect("/Admin/Home/NoRule");
            }

            ViewData["typeId"] = id;

            if (id == 1 || id == 2)
            {
                aa.PlatbyKredit = platbaServ.GetKreditAll(aa.Pobocka.PokladnaId).Where(d => d.Kategorie == id).ToList();
            }
            else if (id == 3 || id == 4)
            {
                aa.PlatbyKreditCas = platbaServ.GetKreditCasAll(aa.Pobocka.PokladnaId).Where(d => d.Kategorie == (id - 2)).ToList();
            }
            else if (id == 5 || id == 6)
            {
                aa.PlatbyCas = platbaServ.GetCasAll(aa.Pobocka.PokladnaId).Where(d => d.Kategorie == (id - 4)).ToList();
            }


            return View(aa);
        }

        public IActionResult PaymentCreditCreate(int id)
        {
            var model = new PlatbaKreditCiselnikModel { Kategorie = id, Visible = true, Platnost = true, Poradi = 1, Slevnena = (id == 2) };

            return PartialView("PaymentCreditCreate", model);
        }

        [HttpPost]
        public IActionResult PaymentCreditCreate(PlatbaKreditCiselnikModel model)
        {
            if (ModelState.IsValid)
            {
                SetMainPageValues();

                PlatbaKredit pl = new PlatbaKredit();
                model.Id = 0;
                model.CopyToDb(pl);
                pl.PokladnaId = aa.Pobocka.PokladnaId;
                pl.AutorID = aa.User.Id.Value;
                pl.Unix = DateTimeExtensions.ToUnix(DateTime.Now);
                platbaServ.Insert(pl);
            }

            return PartialView("PaymentCreditCreate", model);
        }

        public IActionResult PaymentCreditEdit(int id)
        {
            var model = new PlatbaKreditCiselnikModel();
            if (id > 0)
            {

                var db = platbaServ.GetKreditById(id);
                if (db != null)
                {
                    model.CopyFromDb(db);
                }
            }
            return PartialView("PaymentCreditEdit", model);
        }

        [HttpPost]
        public IActionResult PaymentCreditEdit(PlatbaKreditCiselnikModel model)
        {
            if (ModelState.IsValid)
            {
                PlatbaKredit le = platbaServ.GetKreditById(model.Id);
                model.CopyToDb(le);
                try
                {
                    platbaServ.Update(le);
                }
                catch (Exception ex)
                {

                }

            }

            return PartialView("PaymentCreditEdit", model);
        }

        #region kredity casove
        public IActionResult PaymentCreditTimeCreate(int id)
        {
            var model = new PlatbaKreditCasCiselnikModel { Kategorie = id, Platnost = true, Visible = true, Poradi = 1, PocetLidi = 1, Kredity = 10 };

            return PartialView("PaymentCreditTimeCreate", model);
        }

        [HttpPost]
        public IActionResult PaymentCreditTimeCreate(PlatbaKreditCasCiselnikModel model)
        {
            if (ModelState.IsValid)
            {
                SetMainPageValues();

                model.Kategorie = (model.Kategorie == 3) ? 1 : 2;
                model.Slevnena = (model.Kategorie == 3);
                PlatbaKreditCas pl = new PlatbaKreditCas();
                model.Id = 0;
                model.CopyToDb(pl);
                pl.Id = 0;
                pl.PokladnaId = aa.Pobocka.PokladnaId;
                pl.AutorID = aa.User.Id.Value;
                pl.Ts = DateTimeExtensions.ToUnix(DateTime.Now);

                try
                {
                    platbaServ.Insert(pl);
                }
                catch (Exception ex)
                {

                }
            }

            return PartialView("PaymentCreditTimeCreate", model);
        }

        public IActionResult PaymentCreditTimeEdit(int id)
        {
            var model = new PlatbaKreditCasCiselnikModel();
            if (id > 0)
            {

                var db = platbaServ.GetKreditCasById(id);
                if (db != null)
                {
                    model.CopyFromDb(db);
                }
            }
            return PartialView("PaymentCreditTimeEdit", model);
        }

        [HttpPost]
        public IActionResult PaymentCreditTimeEdit(PlatbaKreditCasCiselnikModel model)
        {
            if (ModelState.IsValid)
            {
                PlatbaKreditCas le = platbaServ.GetKreditCasById(model.Id);
                model.CopyToDb(le);

                platbaServ.Update(le);
            }

            return PartialView("PaymentCreditTimeEdit", model);
        }
        #endregion

        #endregion

        #region Settings
        public ActionResult SettingPob()
        {
            SetMainPageValues();

            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }
            if (!aa.User.HasRule("spinstat_view"))
            {
                return Redirect("/Admin/Home/NoRule");
            }

            aa.Title = "Nastavení pobočky " + pobocka.Name;
            ViewBag.Defaults = pobServ.GetDefaults();

            return View(aa);
        }

        [HttpPost]
        public IActionResult SetIniValue(SetIniBool model)
        {
            var resp = new JsonStatus();

            if (ModelState.IsValid)
            {
                var ini = pobServ.GetIni(model.Pob, model.Item);
                if (ini != null)
                {

                    if (model.Value.ToString() != ini.Value)
                    {
                        ini.Value = model.Value.ToString();
                        pobServ.Update(ini);
                        pobServ.ClearPobockaInis(model.Pob);
                        resp.Status = true;
                    }

                }
            }

            return Json(resp);
        }



        public ActionResult SettingMain()
        {
            aa.Title = "Nastavení systému";
            SetMainPageValues();
            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }
            if (!aa.User.HasRule("spinstat_view"))
            {
                return Redirect("/Admin/Home/NoRule");
            }

            return View(aa);
        }

        public IActionResult SettingMainEdit()
        {
            aa.Title = "Nastavení systému";
            SetMainPageValues();


            return PartialView("SettingMainEdit", pobServ.GetMainIni().MainIniObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SettingMainEdit(MainIni model)
        {

            if (ModelState.IsValid)
            {
                var cv = pobServ.GetMainIni();
                cv.MainIniObj = model;
                pobServ.UpdateMainIni(cv);

            }
            return PartialView("SettingMainEdit", model);
        }

        [HttpPost]
        public IActionResult SetShowRestPlacesOnBoard(MainIni m)
        {
            JsonStatus resp = new JsonStatus();

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                MainIniCover cv = pobServ.GetMainIni();
                cv.MainIniObj.ShowRestPlacesOnBoard = (m.ShowRestPlacesOnBoard != true);
                pobServ.UpdateMainIni(cv);
                resp.Status = true;

            } while (false);

            return Json(resp);
        }

        public IActionResult SettingMainStyleEdit()
        {
            aa.Title = "Nastavení systému";
            SetMainPageValues();


            return PartialView("SettingMainStyleEdit", pobServ.GetMainIni().MainStyleObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SettingMainStyleEdit(MainIniStyle model)
        {

            if (ModelState.IsValid)
            {
                var cv = pobServ.GetMainIni();
                cv.MainStyleObj = model;
                pobServ.UpdateMainIni(cv);

            }
            return PartialView("SettingMainStyleEdit", model);
        }
        #endregion

        #region PayGates
        public ActionResult SettingMainPayGates()
        {
            aa.Title = "Nastavení paltebních bran";
            MainIniCover cv = pobServ.GetMainIni();
            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }
            if (!aa.User.HasRule("spinstat_view"))
            {
                return Redirect("/Admin/Home/NoRule");
            }

            return View(aa);
        }

        public IActionResult SettingMainPayGatePaysEdit()
        {
            aa.Title = "Nastavení systému";
            SetMainPageValues();


            return PartialView("SettingMainPayGatePaysEdit", pobServ.GetMainIni().MainGatePaysObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SettingMainPayGatePaysEdit(MainIniPayGates model)
        {

            if (ModelState.IsValid)
            {
                var cv = pobServ.GetMainIni();
                cv.MainGatePaysObj = model;
                pobServ.UpdateMainIni(cv);

            }
            return PartialView("SettingMainPayGatePaysEdit", model);
        }
        #endregion

        #region BankAccount
        public ActionResult SettingMainBankAccount()
        {
            aa.Title = "Nastavení BankAccount";
            SetMainPageValues();
            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }
            if (!aa.User.HasRule("spinstat_view"))
            {
                return Redirect("/Admin/Home/NoRule");
            }
            aa.MainIniCover = pobServ.GetMainIni();

            return View(aa);
        }

        public IActionResult SettingMainBankAccountEdit()
        {
            aa.Title = "Nastavení systému";
            SetMainPageValues();


            return PartialView("SettingMainBankAccountEdit", pobServ.GetMainIni().BankAccountObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SettingMainBankAccountEdit(MainIniBankAccount model)
        {

            if (ModelState.IsValid)
            {
                var cv = pobServ.GetMainIni();
                cv.BankAccountObj = model;
                pobServ.UpdateMainIni(cv);
                pobServ.ClearMainIni();

            }
            return PartialView("SettingMainBankAccountEdit", model);
        }


        public ActionResult PaymentQr(int id, int ty)
        {
            var resp = new JsonStatus();

            SetMainPageValues();
            
            int suma = 0;
            string account = "";
            string bank = "";
            string typ = "";
            string cur = "CZK";

            aa.MainIniCover = pobServ.GetMainIni();
            if (aa.MainIniCover.BankAccountObj.Active)
            {
                var bankO = aa.MainIniCover.BankAccountObj;
                account = bankO.AccountNumber;
                bank = bankO.BankCode;

                if (aa.User.Id.HasValue)
                {

                    if (ty == 1)
                    {
                        var db = platbaServ.GetKreditById(id);
                        if (db != null)
                        {
                            suma = db.Castka;
                            typ = "kr-neom:" + db.Id;
                        }
                    }
                    else if (ty == 2)
                    {
                        var db = platbaServ.GetKreditCasById(id);
                        if (db != null)
                        {
                            suma = db.Castka;
                            typ = "kr-cas:" + db.Id;
                        }
                    }

                    if (suma > 0)
                    {
                        resp.Meta = $"https://api.paylibo.com/paylibo/generator/czech/image?accountNumber={account}&bankCode={bank}&amount={suma}.00&currency={cur}&vs={aa.User.Id}&message={typ} {aa.User.Email.ToLowerInvariant()}";

                        string ucet = $"{account}/{bank}";

                        if (bankO.AccountPrefix != null && bankO.AccountPrefix.Length > 0)
                        {
                            resp.Meta += $"&accountPrefix={bankO.AccountPrefix}";
                            ucet = bankO.AccountPrefix + "-" + ucet;
                        }

                        ViewBag.Ucet = ucet;
                        ViewBag.Castka = suma + " " + cur;
                        ViewBag.VS = aa.User.Id;
                        ViewBag.Message = typ + " " + aa.User.Email.ToLowerInvariant();

                    }
                    else {
                        ViewBag.Castka = "Suma není zadána";
                    }


                }
                else
                {
                    resp.MsgAdd("Uživatel není přihlášen");
                    ViewBag.Ucet = "Uživatel není přihlášen";
                }
            }
            else {
                resp.MsgAdd("Účet není nastaven");
                ViewBag.Ucet = "Účet není nastaven";
            }



            return PartialView("PaymentQr", resp);
        }

        #endregion

        public ActionResult NoClosedLessons()
        {
            aa.Title = "Neuzavřené hodiny";
            SetMainPageValues();
            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }
            if (!aa.User.HasRule("spin_close"))
            {
                return Redirect("/Admin/Home/NoRule");
            }


            ViewData["lekce"] = lekceServ.GetNotClossed(aa.Pobocka.Id);


            return View(aa);
        }

    }
}
