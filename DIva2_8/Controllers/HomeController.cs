using Diva2.Core;
using Diva2.Core.Main;
using Diva2.Core.Main.Calendar;
using Diva2.Core.Main.PayGates;
using Diva2.Core.Main.Platby;
using Diva2.Core.Main.Videa;
using Diva2.Core.Model.Json;
using Diva2.Core.Model.Money;
using Diva2.Data;
using Diva2.Services;
using Diva2.Services.Managers.Calendar;
using Diva2.Services.Managers.Content;
using Diva2.Services.Managers.Mains;
using Diva2.Services.Managers.Platby;
using Diva2.Services.Managers.Pobocky;
using Diva2.Services.Managers.Setting;
using Diva2.Services.Managers.Users;
using Diva2.Services.Managers.Videa;
using Diva2Web.Areas.Admin;
using Diva2Web.Areas.Admin.Controllers;
using Diva2Web.Models.Admin;
using Diva2Web.Models.Lekces;
using Diva2Web.Models.Platby;
using Diva2Web.Models.Videos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Diva2.Controllers
{

    public class HomeController : BaseAdminController
    {
        private readonly ILogger<HomeController> _logger;
        private IWorkContext workContext;

        private ILekceTypService lekceTypeServ;
        private ILekceAddonsService lekAddServ;
        private IPageService pageServ;

        public HomeController(ApplicationDbContext dbContext,
            IMemoryCache memoryCache, ILogger<HomeController> logger,
            IHttpContextAccessor httpContextAccessor,
            ILekceService lekceSer, ILekceTypService lekTypSer, IPobockaService pobSer,
            IObjednavkyService objSer, IUser8Service userSer, ILogs8Service logServ,
            ILekceAddonsService leAddSe, IPageService pageSer, IVideoService vidSer, ICalendarService calSer,
            ILektorService lkService, IPlatbaService plService, IWorkContext workContext) : base(dbContext, httpContextAccessor, memoryCache, userSer, pobSer, logServ, objSer)
        {
            lekceServ = lekceSer;
            platbaServ = plService;
            lektorServ = lkService;
            lekceTypeServ = lekTypSer;
            objServ = objSer;
            lekAddServ = leAddSe;
            pageServ = pageSer;
            videoServ = vidSer;
            calServ = calSer;


            this.workContext = workContext;

            _logger = logger;


            cache = new CacheHelper(memoryCache, dbContext.SubDomain);
        }

        private void LoadForPublic()
        {

            aa.Pages = pageServ.GetVisibleForMenu();

        }

        public IActionResult IndexApi(int id)
        {
            SetMainPageValues();
            LoadUserProperty(base.objServ);

            var api = lekceServ.GetWeaksApi(id);

            return Json(api);
        }
        public IActionResult Index()
        {

            var bb = base.RouteData.Values;
            SetMainPageValues();
            LoadUserProperty(base.objServ);
            LoadForPublic();

            if (aa.Pobocka.Typ == 0)
            {
                aa.Pobocka.Typ = 1;
            }


            string view = "Rozvrh";

            if (aa.Pobocka.Typ == 1)
            {
               // aa.Pobocka.Typ = 2;
            }


            if (aa.Pobocka.Typ == 1)
            {
                aa.RozvrhyObj = lekceServ.GetWeaksFuture(aa.Pobocka.Id, DateTime.Now);
            }
            else if (aa.Pobocka.Typ == 2)
            {
                aa.Rozvrhy2Obj = lekceServ.GetWeaksFuture2(aa.Pobocka.Id, DateTime.Now);
                view = "Rozvrh2";
            }
            else if (aa.Pobocka.Typ == 3)
            {
                aa.Rozvrhy3Obj = lekceServ.GetWeaksFuture3(aa.Pobocka.Id);
                view = "Calendar";
            }
            else if (aa.Pobocka.Typ == 4)
            {
                DateTime dt = new DateTime(2025, 8, 31);
                aa.Rozvrhy2Obj = lekceServ.GetWeaksFuture2(aa.Pobocka.Id, dt);
                view = "Rozvrh2Krouzek";
            }

            aa.Lektori = lektorServ.GetAll().ToList();
            aa.TypyLekci = lekceTypeServ.GetAll().ToList();
            aa.Title = aa.Pobocka.Name;


            return View(view, aa);

        }


        public IActionResult Calendar()
        {
            SetMainPageValues();
            LoadUserProperty(objServ);
            LoadForPublic();

            aa.RozvrhyObj = lekceServ.GetWeaksFuture(aa.Pobocka.Id, DateTime.Now);

            return View(aa);

        }

        public IActionResult CalendarForCreate(int m, int dr, int d)
        {

            LoadUserProperty(objServ);



            return PartialView("CalendarView", aa);
        }

        public IActionResult CalendarView(int id)
        {

            SetMainPageValues();
            LoadUserProperty(objServ);

            GetDataForOneLesson(id);

            ViewBag.ShowButtons = true;


            return PartialView("CalendarView", aa);
        }

        /// <summary>
        /// Vola se jsonem
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult LessonViewTable(int id)
        {
            SetMainPageValues();
            LoadUserProperty(objServ);

            GetDataForOneLesson(id);



            ViewBag.ShowButtons = false;

            return PartialView("LessonViewTable", aa);
        }

        /// <summary>
        /// nacita se ze stranky
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IActionResult LessonViewTable(AdminPageModel model)
        {
            return PartialView("LessonViewTable", model);
        }

        /// <summary>
        /// jedna lekce
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult LessonView(int id)
        {
            SetMainPageValues();
            LoadUserProperty(objServ);

            GetDataForOneLesson(id);

            ViewBag.ShowButtons = true;
            return PartialView("LessonView", aa);
        }

        private void GetDataForOneLesson(int id)
        {

            aa.LekceOneBoard = new LekceBoardModel();

            var lekce = lekceServ.GetById(id);
            if (lekce != null)
            {
                aa.LekceOneBoard.CopyFromDb(lekce);
                aa.LekceOneBoard.Zakaznici = new List<LekceUserModel>();
                aa.LekceOneBoard.Video = lekAddServ.GetVideoById(id);


                var zak = objServ.GetByLekce(id);
                foreach (var z in zak)
                {
                    aa.LekceOneBoard.Zakaznici.Add(new LekceUserModel(z));
                }
                var leks = lektorServ.GetAll();
                if (aa.LekceOneBoard.Lektor1 > 0)
                {
                    var lek1 = leks.Where(d => d.Id == aa.LekceOneBoard.Lektor1).FirstOrDefault();
                    if (lek1 != null)
                    {
                        aa.LekceOneBoard.Lektor1_O = new LektorModel(lek1);
                    }
                }
                if (aa.LekceOneBoard.Lektor1 > 0)
                {
                    var lek2 = leks.Where(d => d.Id == aa.LekceOneBoard.Lektor1).FirstOrDefault();
                    if (lek2 != null)
                    {
                        aa.LekceOneBoard.Lektor2_O = new LektorModel(lek2);
                    }
                }
                if (lekce.TypHodiny > 0)
                {
                    var typ = lekceTypeServ.GetAll().Where(d => d.Id == lekce.TypHodiny).FirstOrDefault();
                    if (typ != null)
                    {
                        LekceTypModel l = new LekceTypModel(typ);
                        aa.LekceOneBoard.TypHodiny_O = l;
                    }
                }
            }

            aa.Messages = objServ.GetUserTextByLesson(aa.LekceOneBoard.Id);
        }


        public IActionResult Error(int? id)
        {
            //  return View(new ErrorViewModel  { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            SetMainPageValues();
            LoadForPublic();

            return View(aa);
        }


        public IActionResult SetPob(int? id)
        {

            if (id.HasValue)
            {
                var pob = pobServ.GetPobocky().Where(d => d.Id == id).FirstOrDefault();
                if (pob != null)
                {
                    SessionSetInt("sessPobId", pob.Id);
                }
            }

            return RedirectToAction("index", "Home");
        }

        public IActionResult Prices()
        {
            DateTime dt = DateTime.Now;

            SetMainPageValues();
            LoadUserProperty(objServ);
            LoadForPublic();

            aa.PlatbyKredit = platbaServ.GetKreditAll(aa.Pobocka.PokladnaId).Where(d => d.Visible == true && d.Platnost == true).OrderBy(d => d.Castka).ToList();
            aa.PlatbyKreditCas = platbaServ.GetKreditCasAll(aa.Pobocka.PokladnaId).Where(d => d.Visible == true && d.Platnost == true).OrderBy(d => d.Castka).ToList();
            aa.PlatbyCas = new List<PlatbaCas>(); //  platbaService.GetCasAll();

            aa.Title = "Ceník";

            return View(aa);
        }

        public IActionResult MyData(int? id)
        {
            int Y = DateTime.Now.Year;
            if (id.HasValue)
            {
                Y = id.Value;
            }

            SetMainPageValues();
            LoadForPublic();

            if (aa.User.Id.HasValue)
            {
                LoadUserProperty(objServ);

            }

            aa.Id = Y;
            aa.Title = "Moje data";

            return View(aa);
        }

        public IActionResult Lectors()
        {
            DateTime dt = DateTime.Now;

            SetMainPageValues();
            LoadUserProperty(objServ);
            LoadForPublic();

            aa.Lektori = lektorServ.GetAll().Where(d => d.Viditelnost == true).ToList();

            aa.Title = "Lektoři";

            return View(aa);
        }

        public IActionResult Info()
        {

            SetMainPageValues();
            LoadUserProperty(base.objServ);
            LoadForPublic();

            aa.Page = new Diva2Web.Models.Content.PageModel(pageServ.GetByType(Core.Main.Content.PageType.info));

            return View(aa);
        }


        public IActionResult About()
        {

            SetMainPageValues();
            LoadUserProperty(base.objServ);
            LoadForPublic();

            aa.Page = new Diva2Web.Models.Content.PageModel(pageServ.GetByType(Core.Main.Content.PageType.about));

            return View(aa);
        }

        public IActionResult Help()
        {
            DateTime dt = DateTime.Now;

            SetMainPageValues();
            LoadUserProperty(objServ);
            LoadForPublic();

            aa.Title = "Pomoc";

            return View(aa);
        }

        public IActionResult Gdpr()
        {
            DateTime dt = DateTime.Now;


            SetMainPageValues();
            LoadUserProperty(objServ);
            LoadForPublic();


            aa.Title = "Gdpr";

            return View(aa);
        }

        public IActionResult PaymentOk()
        {
            SetMainPageValues();
            LoadUserProperty(objServ);
            LoadForPublic();

            aa.Title = "Poděkování";
            return View(aa);
        }

        public IActionResult PaymentConfirm(PaysResponseModel m)
        {
            JsonStatus resp = new JsonStatus();

            SetMainPageValues();

            Core.Model.Json.JsonAddMoney js = new Core.Model.Json.JsonAddMoney();
            do
            {
                string source = $"{m.PaymentOrderID}{m.MerchantOrderNumber}{m.PaymentOrderStatusID}{m.CurrencyID}{m.Amount}{m.CurrencyBaseUnits}";
                string source2 = $"{m.PaymentOrderID} {m.MerchantOrderNumber} {m.PaymentOrderStatusID} {m.CurrencyID} {m.Amount} {m.CurrencyBaseUnits}";
                var data = Encoding.UTF8.GetBytes(source);
                var key = Encoding.UTF8.GetBytes(aa.MainIniCover.MainGatePaysObj.Pays_Pass);
                var hmac = new HMACMD5(key);
                var hashBytes = hmac.ComputeHash(data);
                var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                js.MsgAdd(source);
                js.MsgAdd(source2);
                js.MsgAdd(hash);
                js.MsgAdd(m.Hash);

                if (!hash.Equals(m.Hash))
                {
                    js.MsgAdd("hash ne souhlasi");
                    continue;
                }
                /**/
                int idp;
                if (int.TryParse(m.MerchantOrderNumber, out idp))
                {
                    js.MsgAdd("MerchantOrderNumber parsed int OK");
                    PaysItem gp = platbaServ.GetPaysById(idp);
                    if (gp != null)
                    {
                        js.MsgAdd($"Pays nactena ");

                        gp.Status = m.PaymentOrderStatusID;
                        gp.StatusDesc = m.PaymentOrderStatusDescription;
                        gp.PaymentOrderId = m.PaymentOrderID;
                        gp.UpdatedDt = DateTime.Now;
                        gp.Hash = m.Hash;
                        platbaServ.Update(gp);

                        if (gp.Status != 3)
                        {
                            js.MsgAdd($"Platba nerealizovana");
                            continue;
                        }
                        PlatbaBase plb = new PlatbaBase();
                        js.MsgAdd($"Hledá se platba item:{gp.ItemId} type:{gp.TypeId}");
                        if (gp.TypeId == 1 || gp.TypeId == 2)
                        {
                            plb = platbaServ.GetKreditById(gp.ItemId);
                            if (plb == null)
                            {
                                js.MsgAdd($"nenačetla se platba item:{gp.ItemId} type:{gp.TypeId}");
                                continue;
                            }
                        }
                        else if (gp.TypeId == 3 || gp.TypeId == 4)
                        {
                            plb = platbaServ.GetKreditCasById(gp.ItemId);
                            if (plb == null)
                            {
                                js.MsgAdd($"nenačetla se platba item:{gp.ItemId} type:{gp.TypeId}");
                                continue;
                            }
                        }
                        else
                        {
                            js.MsgAdd($"Spatny {gp.TypeId} ");
                        }
                        try
                        {

                            AddMoneyTrans t = new AddMoneyTrans()
                            {
                                PlatbaId = gp.ItemId,
                                TypPlatbyId = gp.TypeId,
                                PokladnaId = gp.PokladnaId,
                                UserId = gp.UserId,
                                ProvedlId = gp.UserId,
                                DoPokladny = 0,
                                ZBanky = true
                            };

                            js.MsgAdd($"Try Add money");

                            var trans = platbaServ.AddMoney(t, js);
                            if (trans.IsOk)
                            {

                                if (t.TypPlatbyId == 1 || t.TypPlatbyId == 2)
                                {
                                    objServ.AddKredit(trans);
                                }
                                else if (t.TypPlatbyId == 3 || t.TypPlatbyId == 4)
                                {
                                    objServ.AddKreditTime(trans);
                                }
                                gp.AddedCredit = true;
                                platbaServ.Update(gp);
                                UpdateZbytek(t.UserId, t.PokladnaId, js);
                            }
                            else
                            {

                                js.MsgAdd($"no Ok - (Try Add money)");
                            }
                        }
                        catch (Exception ex)
                        {
                            logServ.Insert(new Core.Main.Main.Log8() { Category = Core.Main.Main.LogCategory.pay_gate, Text = ex.Message.ToString(), Created = DateTime.Now });
                        }

                    }
                    else
                    {
                        js.MsgAdd("! Platba pay_items nenactena");
                    }
                }
                else
                {
                    js.MsgAdd("! neparsroval se MerchantOrderNumber");
                }
            } while (false);

            logServ.Insert(new Core.Main.Main.Log8() { Category = Core.Main.Main.LogCategory.pay_gate, Text = js.MsgToString(), Created = DateTime.Now });

            return Json(resp);
        }

        private void UpdateZbytek(int userId, int pokId, Core.Model.Json.JsonAddMoney resp)
        {
            objServ.ClearZbytekUzivatele(userId);
            objServ.ClearHistoriTransakci(userId);
            objServ.ClearHistorieRoky(userId);
            objServ.ClearZbytekVObjednavkachUzivatele(userId);



            var Zbytek = objServ.GetZbytekUzivatele(userId);
            resp.Kredity = Zbytek.KredityItem(pokId);
            var zb = Zbytek.KredityTimeItem(pokId, DateTime.Now);
            if (zb != null)
            {
                resp.KredityCasove = $"{zb.Kredit} / {zb.ZbyvaDni}";
            }

            resp.Status = true;
        }
        /*
        public IActionResult PaymentErr()
        {
            SetMainPageValues();
            LoadUserProperty(objServ);

            aa.Title = "Platba neproběhla";
            return View(aa);
        }
        /**/
        public IActionResult PaymentErr(PaysResponseModel m)
        {
            SetMainPageValues();
            LoadUserProperty(objServ);
            LoadForPublic();

            aa.Title = "Platba neproběhla";
            do
            {
                int idp;
                if (int.TryParse(m.MerchantOrderNumber, out idp))
                {
                    PaysItem gp = platbaServ.GetPaysById(idp);
                    if (gp != null)
                    {
                        if (m.PaymentOrderStatusID == 2)
                        {
                            if (gp.Status != 0)
                            {
                                logServ.Insert(new Core.Main.Main.Log8() { Category = Core.Main.Main.LogCategory.pay_gate, Text = $"Nelze rušit platbu s uloženým statusem {gp.Status}", Created = DateTime.Now });
                            }
                            else
                            {
                                gp.Status = 2;


                                try
                                {
                                    platbaServ.Update(gp);
                                    aa.Title = "Platba - neproběhla";
                                }
                                catch (Exception ex)
                                {
                                    logServ.Insert(new Core.Main.Main.Log8() { Category = Core.Main.Main.LogCategory.pay_gate, Text = ex.Message.ToString(), Created = DateTime.Now });
                                }
                            }
                        }
                    }
                }
                else
                {
                    logServ.Insert(new Core.Main.Main.Log8() { Category = Core.Main.Main.LogCategory.pay_gate, Text = $"Nenašlo se id pro udělení statusu {m.MerchantOrderNumber}", Created = DateTime.Now });
                }

            } while (false);

            return View(aa);
        }


        /*
        public async Task<IActionResult> FixUsers()
        {

            var updated = new List<User8>();
            var users = userManager.Users.ToList();
            var groupNames = users.ToLookup(u => u.UserName.Trim().ToLower());

            FixDuplicateUsernames(updated, groupNames);

            foreach (var group in groupNames)
            {
                foreach (var user in group)
                {
                    if (String.IsNullOrWhiteSpace(user.NormalizedUserName))
                    {
                        var task = await userManager.UpdateSecurityStampAsync(user);
                        if (!task.Succeeded)
                        {
                            var errors = task.Errors;
                        }
                    }
                }
            }

            return RedirectToAction("Index");
        }

        [NonAction]
        private static void FixDuplicateUsernames(List<User8> updated, ILookup<string, User8> groupNames)
        {
            /*
            foreach (var group in groupNames)
            {
                if (group.Count() > 1)
                {
                    int counter = 1;
                    foreach (var user in group.Skip(1))
                    {
                        user.UserName = user.UserName.Trim() + ("-" + counter.ToString());
                        counter++;
                        updated.Add(user);
                    }
                }

        }
        }/**/




    }
}
