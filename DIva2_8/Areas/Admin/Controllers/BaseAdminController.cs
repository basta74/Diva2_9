using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Diva2.Core.Extensions;
using Diva2.Core.Main;
using Diva2.Core.Main.Calendar;
using Diva2.Core.Main.Main;
using Diva2.Core.Main.Pobocky;
using Diva2.Data;
using Diva2.Services;
using Diva2.Services.Managers;
using Diva2.Services.Managers.Calendar;
using Diva2.Services.Managers.Customers;
using Diva2.Services.Managers.Emails;
using Diva2.Services.Managers.Mains;
using Diva2.Services.Managers.Platby;
using Diva2.Services.Managers.Pobocky;
using Diva2.Services.Managers.Setting;
using Diva2.Services.Managers.Users;
using Diva2.Services.Managers.Videa;
using Diva2Web.Models;
using Diva2Web.Models.Admin;
using Diva2Web.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;


namespace Diva2Web.Areas.Admin.Controllers
{

    public class BaseAdminController : Controller
    {
        protected AdminPageModel aa;

        protected ApplicationDbContext dbContext;

        protected string subDomain;
        protected Pobocka pobocka;

        protected IHttpContextAccessor httpContextAccessor;

        protected CacheHelper cache;
        protected IPobockaService pobServ;
        protected IUser8Service userServ;
        protected ILogs8Service logServ;
        protected IObjednavkyService objServ;
        protected IVideoService videoServ;

        protected ISkupinaZakaznikaService skupZakServ;
        protected ILekceService lekceServ;

        protected ILekceTypService lekceTypServ;
        protected ILekceMustrService lekceMustrServ;
        protected ISkupinaZakaznikaService skupinaZakaznikaServ;
        protected ILektorService lektorServ;
        protected IPlatbaService platbaServ;
        protected IComunicationService comServ;
        protected ICalendarService calServ;
        public BaseAdminController(ApplicationDbContext context, IHttpContextAccessor httpContextAcc, IMemoryCache memoryCache,
            IUser8Service userSer, IPobockaService pobSer, Diva2.Services.Managers.Mains.ILogs8Service logSer, IObjednavkyService objSer)
        {
            this.dbContext = context;
            this.subDomain = dbContext.SubDomain;

            var host = httpContextAcc.HttpContext.Request.Host.Host;

            if (host.Contains("localhost")) {
                host = "localhost";
            }
   

            this.subDomain = host.Split('.')[0];

            this.httpContextAccessor = httpContextAcc;
            this.cache = new CacheHelper(memoryCache, dbContext.SubDomain);
            this.pobServ = pobSer;
            this.userServ = userSer;
            this.logServ = logSer;
            this.objServ = objSer;
            this.aa = new AdminPageModel();

        }


        protected void SetMainPageValues()
        {

            //SetUserFromIdentity(aa.User);
            TrySetUserFromSess(aa.User);

            SetSessions();

            aa.PopulateAdminMenu();
            aa.PopulatePublicMenu(this.Url);
            aa.PopulateDays();
            
            var routes = httpContextAccessor.HttpContext.GetRouteData();
            aa.ControllerName = routes.Values["controller"]?.ToString() as string;
            aa.ActionName = routes.Values["action"]?.ToString() as string;

            aa.SubDomain = subDomain;
            aa.Pobocky = pobServ.GetPobocky();

            aa.Style = new StyleModel(this.pobocka);

            int pobockaId = aa.Pobocka.Id;

            var zacatky = pobServ.GetZacatky();
            var zacatkyPob = zacatky.Where(d => d.PobockaId == aa.Pobocka.Id);
            if (zacatkyPob.Count() == 0)
            {
                List<CasZacatek> insertCasy = new List<CasZacatek>();
                insertCasy.Add(new CasZacatek() { PobockaId = pobockaId, Poradi = 1, Minuta = 780, Value = "13:00" });
                insertCasy.Add(new CasZacatek() { PobockaId = pobockaId, Poradi = 2, Minuta = 840, Value = "14:00" });
                insertCasy.Add(new CasZacatek() { PobockaId = pobockaId, Poradi = 3, Minuta = 900, Value = "15:00" });
                insertCasy.Add(new CasZacatek() { PobockaId = pobockaId, Poradi = 4, Minuta = 960, Value = "16:00" });
                insertCasy.Add(new CasZacatek() { PobockaId = pobockaId, Poradi = 5, Minuta = 1020, Value = "17:00" });
                pobServ.Insert(insertCasy);
                zacatky = pobServ.GetZacatky();
            }

            aa.IniMinutes = pobServ.GetIniMinutes();
            if (aa.IniMinutes.Where(d => d.PobockaId == pobockaId).Count() == 0)
            {
                List<CalIniMinute> list = new List<CalIniMinute>();
                
                
                    foreach (var min in aa.Pobocka.CalendarSettingObj.GetMinutes())
                    {
                        TimeSpan ts = new TimeSpan(0, min, 0);
                        bool visible = zacatky.Where(d => d.Minuta == min && d.PobockaId == pobockaId).Count() > 0;
                        list.Add(new CalIniMinute() { PobockaId = pobockaId, Minute = min, Visible = visible, Time = ts });
                    }
                
                pobServ.Insert(list);
                pobServ.ClearIniMinutes();
                aa.IniMinutes = pobServ.GetIniMinutes();

            }


            aa.AplyRulesToAdminMenu();

        }

        public void LoadUserProperty(IObjednavkyService objServ)
        {
            if (aa.User.Id.HasValue && aa.User.Id > 0)
            {
                aa.User.ObjednaneLekce = objServ.GetObjednaneLekceUzivatele(aa.User.Id.Value);
                aa.User.Zbytek = objServ.GetZbytekUzivatele(aa.User.Id.Value);
                aa.User.HistorieLekci = objServ.GetHistoriTransakci(aa.User.Id.Value);
                aa.User.HistorieLekciYears = aa.User.HistorieLekci.Select(d => d.Datum.Year).Distinct();
            }
        }

        protected void SetUserFromIdentity(UserModel u)
        {

            /*
            if (User.Identity.IsAuthenticated)
            {
                if (int.TryParse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value, out int uid))
                {
                    u.Id = uid;
                    u.Nick = User.Identity.Name;
                    //u.Email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;


                    List<string> rules = userService.GetRules(uid);
                }
            } /**/
        }

        protected void SetUserToSession(UserModel u)
        {

            SessionSetInt("userId", u.Id.Value);
            SessionSetString("userNick", u.Nick);
            SessionSetString("userName", u.CeleJmeno);
            SessionSetString("userEmail", u.Email);
            int gdprB = u.GdprBase == true ? 1 : 0;
            int gdprN = u.GdprNews == true ? 1 : 0;
            SessionSetInt("userGdpr", gdprB);
            SessionSetString("userGdprDt", u.GdprBaseDT.HasValue == true ? u.GdprBaseDT.Value.ToString("dd.MM.yyyy HH:mm") : "");
            SessionSetInt("userGdprNews", gdprN);
            SessionSetString("userGdprNewsDt", u.GdprNewsDT.HasValue == true ? u.GdprNewsDT.Value.ToString("dd.MM.yyyy HH:mm") : "");
            SessionSetInt("userPocetObjednavek", u.PocetObjednavek);
            SessionSetInt("userMayCheaper", u.MayCheaper ? 1 : 0);
            SessionSetString("userRules", String.Join("~", u.Rules.ToArray()));
        }

        protected void TrySetUserFromSess(UserModel u)
        {
            int? userId = SessionGetInt("userId");

            if (!(userId.HasValue))
            {
                if (User.Identity.IsAuthenticated)
                {
                    var email = User.Identity.Name;
                    var users = userServ.GetByEmail(email);
                    if (users.Count == 1)
                    {
                        var user = users.FirstOrDefault();
                        AddUserProperties(user);
                        userId = user.Id;
                    }

                }
            }


            if (userId.HasValue)
            {
                u.Id = userId;
                u.Nick = SessionGetString("userNick");
                u.CeleJmeno = SessionGetString("userName");
                u.Email = SessionGetString("userEmail");
                u.GdprBase = SessionGetBool("userGdpr");
                u.GdprNews = SessionGetBool("userGdprNews");
                u.MayCheaper = SessionGetBool("userMayCheaper");

                if (DateTime.TryParse(SessionGetString("userGdprDt"), out DateTime dateGdpr))
                {
                    u.GdprBaseDT = dateGdpr;
                }
                if (DateTime.TryParse(SessionGetString("userGdprNewsDt"), out DateTime dateNews))
                {
                    u.GdprNewsDT = dateNews;
                }

                u.Rules = SessionGetString("userRules").Split('~').ToList();
                u.PocetObjednavek = SessionGetInt("userPocetObjednavek").Value;
            }
        }

        protected void AddUserProperties(Diva2.Core.Main.Users.User8 user)
        {
            UserModel u = new UserModel();
            u.CopyFromDb(user);
            u.Rules = userServ.GetRules(u.Id.Value);

            user.LastLogin = DateTimeExtensions.ToUnix(DateTime.Now);
            userServ.Update(user);

            objServ.ClearObjednaneLekceUzivatele(user.Id);
            objServ.ClearZbytekUzivatele(user.Id);

            u.PocetObjednavek = objServ.GetPocetObjednavekUzivatele(user.Id);

            SetUserToSession(u);
        }

        protected void ClearSessions()
        {
            httpContextAccessor.HttpContext.Session.Clear();
        }

        protected void SetSessions()
        {

            MainIniCover main = pobServ.GetMainIni();

            aa.MainIniCover = main;


            int? pobId = SessionGetInt("sessPobId");

            if (pobId > 0)
            {
                aa.Pobocka = pobServ.GetPobocky().Where(d => d.Id == pobId).FirstOrDefault();
            }
            else
            {
                aa.Pobocka = pobServ.GetPobocky().Where(d => d.Visible == true).FirstOrDefault();
            }

            pobocka = aa.Pobocka;
            if (pobId != aa.Pobocka.Id)
            {
                SessionSetInt("sessPobId", aa.Pobocka.Id);
            }

            bool setDatetoSess = false;
            var strDt = SessionGetString("sessCurrentDate");
            if (strDt == null)
            {
                aa.CurrentDate = DateTime.Now;
                aa.Monday = DateTimeExtensions.StartOfWeek(aa.CurrentDate, DayOfWeek.Monday);
                setDatetoSess = true;
            }
            else
            {

                DateTime result;
                String format = "yyyy-MM-dd";
                if (DateTime.TryParseExact(strDt, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                {
                    aa.CurrentDate = result;
                    aa.Monday = DateTimeExtensions.StartOfWeek(aa.CurrentDate, DayOfWeek.Monday);
                }
                else
                {
                    aa.CurrentDate = DateTime.Now;
                    aa.Monday = DateTimeExtensions.StartOfWeek(aa.CurrentDate, DayOfWeek.Monday);
                    setDatetoSess = true;
                }
            }
            if (setDatetoSess)
            {
                var date = aa.CurrentDate.ToString("yyyy-MM-dd");
                SessionSetString("sessCurrentDate", date);
            }



            var list = pobServ.GetPobockaInis(aa.Pobocka.Id);
            aa.InisPob = new Dictionary<string, string>();
            foreach (var ini in list)
            {
                aa.InisPob.Add(ini.Name, ini.Value);
            }

        }

        protected void SessionSetCurrentDate(DateTime dt)
        {
            SessionSetString("sessCurrentDate", dt.ToString("yyyy-MM-dd"));
        }

        protected string SessionGetString(string key)
        {
            return httpContextAccessor.HttpContext.Session.GetString($"{subDomain}-{key}");
        }
        protected int? SessionGetInt(string key)
        {
            var val = httpContextAccessor.HttpContext.Session.GetInt32($"{subDomain}-{key}");
            return val;
        }
        protected bool SessionGetBool(string key)
        {
            var val = httpContextAccessor.HttpContext.Session.GetInt32($"{subDomain}-{key}");
            return val > 0;
        }

        protected void SessionSetString(string key, string value)
        {
            httpContextAccessor.HttpContext.Session.SetString($"{subDomain}-{key}", value);
        }
        protected void SessionSetInt(string key, int value)
        {

            httpContextAccessor.HttpContext.Session.SetInt32($"{subDomain}-{key}", value);

            var val = httpContextAccessor.HttpContext.Session.GetInt32($"{subDomain}-{key}");
        }

        protected int? SessionGetPobInt(string key)
        {
            return SessionGetInt($"{pobocka.Id}-{key}");
        }

        protected string SessionGetPobString(string key)
        {
            return SessionGetString($"{pobocka.Id}-{key}");
        }

        protected bool SessionGetPobBool(string key)
        {
            return SessionGetBool($"{pobocka.Id}-{key}");
        }

        protected void SessionSetPobString(string key, string value)
        {
            SessionSetString($"{pobocka.Id}-{key}", value);
        }

        protected void SessionSetPobInt(string key, int value)
        {
            SessionSetInt($"{pobocka.Id}-{key}", value);
        }

        public IActionResult Style()
        {

            SetSessions();

            return PartialView("Style", aa);
        }


        protected void LoadEnviroment()
        {
            aa.Users = userServ.GetAll();
        }
    }
}