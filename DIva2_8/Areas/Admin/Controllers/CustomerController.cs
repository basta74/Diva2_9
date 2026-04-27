using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Diva2.Controllers;
using Diva2.Core.Extensions;
using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Trans;
using Diva2.Core.Main.Users;
using Diva2.Core.Main.Zakaznik;
using Diva2.Core.Model.Json;
using Diva2.Data;
using Diva2.Services.Managers.Customers;
using Diva2.Services.Managers.Mains;
using Diva2.Services.Managers.Platby;
using Diva2.Services.Managers.Pobocky;
using Diva2.Services.Managers.Setting;
using Diva2.Services.Managers.Users;
using Diva2Web.Models;
using Diva2Web.Models.Admin;
using Diva2Web.Models.Export;
using Diva2Web.Models.Helpers;
using Diva2Web.Models.Lekces;
using Diva2Web.Models.Responses;
using Diva2Web.Models.Trans;
using Diva2Web.Models.Users;
using Diva2Web.Models.Zakaznici;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Diva2Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeActionFilterAttribute]
    public class CustomerController : BaseAdminController
    {
        public CustomerController(ApplicationDbContext dbContext,
                   IMemoryCache memoryCache, ILogger<HomeController> logger, IUser8Service userSer,
                   IHttpContextAccessor httpContextAccessor, IPobockaService pobSer,
                   ISkupinaZakaznikaService skupZakServ, IObjednavkyService objSer, ILogs8Service logSer, ILekceService lekSer
                   ) : base(dbContext, httpContextAccessor, memoryCache, userSer, pobSer, logSer, objSer)
        {
            this.skupZakServ = skupZakServ;
            this.lekceServ = lekSer;
        }

        // GET: Home
        public ActionResult Index()
        {
            SetMainPageValues();

            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }

            aa.SkupinyZakaznika = skupZakServ.GetAll();


            return View(aa);
        }

        public ActionResult Deleted()
        {
            SetMainPageValues();

            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }

            aa.Users = userServ.GetDeletedUsers();


            return View(aa);
        }

        public ActionResult DetailM(int id)
        {

            ZakaznikDetailModel model = new ZakaznikDetailModel();

            SetMainPageValues();

            if (id > 0)
            {
                var mu = userServ.GetById(id);
                if (mu != null)
                {
                    model.User = aa.User;
                    model.Zakaznik = new ZakaznikModel(mu);
                    model.Zakaznik.Zbytek = objServ.GetZbytekUzivatele(id);
                    model.Id = id;
                    model.Zakaznik.Transakce = objServ.GetHistoriTransakci(id, false).Where(d => d.PokladnaId == aa.Pobocka.PokladnaId);
                    model.Zakaznik.TransakceYears = model.Zakaznik.Transakce.Select(d => d.Datum.Year).Distinct();
                    model.Pobocka = aa.Pobocka;

                    Dictionary<int, List<UserTransakce>> dict = model.Zakaznik.Transakce.GroupBy(d => d.Datum.Year).ToDictionary(d => d.Key, d => d.ToList());
                    var entries = dict.Select(d => string.Format("\"{0}\": {1}", d.Key.ToString().Trim(), string.Join(",", JsonSerializer.Serialize(d.Value))));
                    model.JsonTransaction = "{" + string.Join(",", entries) + "}";

                    model.SkupinyZakaznika = skupZakServ.GetAll();

                    model.Zakaznik.Groups = userServ.GetUsersGroup(id);

                    model.IniDict = new Dictionary<string, bool>();
                    model.IniDict.Add("kreditCasovy", aa.GetIniPobBool("kreditCasovy"));
                    model.IniDict.Add("kredit_casovy_zobraz_do", aa.GetIniPobBool("kredit_casovy_zobraz_do"));
                    model.IniDict.Add("kredit_casovy_zobraz_od", aa.GetIniPobBool("kredit_casovy_zobraz_od"));

                }
            }

            return PartialView("DetailM", model);

        }

        public ActionResult Extend(string date = null, int weak = 1)
        {
            SetMainPageValues();

            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }

            DateTime from = DateTime.Now;
            ViewBag.DateFrom = from;

            if (date != null && date.Length == 10) {

                if (DateTime.TryParse(date, out from)) {

                    ViewBag.DateFrom = from;
                }
               
                var users = objServ.GetUpFromDate(DateTimeExtensions.ToUnix(from));

            }
            


            return View(aa);
        }

        public IActionResult GetAjaxData(GetModel mo)
        {
            JsonZakaznici resp = new JsonZakaznici();

            do
            {
                // id role zakaznika
                int.TryParse(mo.Value, out int id);
                if (!(id > 0))
                {
                    id = 6; // default role zakaznik
                }
                int.TryParse(mo.Meta, out int meta);

                IEnumerable<User8> data = new List<User8>();

                bool included = (meta == 1);

                if (mo.method == "id")
                {
                    data = userServ.GetCustomers().Where(d => d.Deleted == false).OrderByDescending(d => d.Id);
                    resp.Status = true;
                }
                else if (mo.method == "email")
                {
                    data = userServ.GetCustomers().Where(d => d.Platnost == true && d.Deleted == false && d.GdprNewsDT.HasValue && d.GdprNews == true);
                    resp.Status = true;
                }
                else if (mo.method == "number")
                {
                    data = userServ.GetByRole(id).OrderBy(d => d.InternalNumber);
                    resp.Status = true;
                }
                else if (mo.method == "last")
                {
                    data = userServ.GetCustomers(included).Where(d => d.Deleted == false).OrderByDescending(d => d.LastLogin);
                    resp.Status = true;
                }
                else if (mo.method == "abc")
                {
                    included = true;
                    data = userServ.GetCustomers(included).Where(d => d.Deleted == false && d.Prijmeni.ToLower().StartsWith(mo.Value.ToLower()));
                    resp.Status = true;
                }
                // posledni hodina
                else if (mo.method == "000")
                {
                    if (int.TryParse(mo.Value, out int idhod))
                    {
                        Lekce lek = lekceServ.GetById(idhod);
                        if (lek == null)
                        {
                            resp.Messages.Add(new JsonMessage() { Text = "Nenačetla se hodina obj", Type = JsonMessageType.Warning });
                        }
                        else
                        {

                            Lekce lek2 = lekceServ.GetBy(lek.Rok, lek.Tyden - 1, lek.Den, lek.MinutaKey);
                            if (lek2 == null)
                            {
                                resp.Messages.Add(new JsonMessage() { Text = "Předchozí hodina neexistuje", Type = JsonMessageType.Warning });
                            }
                            else
                            {
                                var ids = objServ.GetByLekce(lek2.Id).Select(d => d.User.Id).ToList();
                                if (ids.Count > 0)
                                {
                                    data = userServ.GetCustomers(ids, included);
                                }

                                resp.Status = true;
                            }
                        }
                    }
                    else
                    {

                        resp.Messages.Add(new JsonMessage() { Text = "Nenačetla se hodina", Type = JsonMessageType.Warning });
                    }



                }
                else if (mo.method == "cat" || mo.method == "cat-email")
                {
                    if (id > 0)
                    {
                        data = userServ.GetAllByGroup(id, included).Where(d => d.Deleted == false);
                        resp.Status = true;
                    }
                    else
                    {

                    }
                }
                else if (mo.method == "topM" || mo.method == "top3M" || mo.method == "top6M" || mo.method == "top1Y")
                {

                    DateTime date = DateTime.Now;

                    if (mo.method == "topM")
                    {
                        date = date.AddMonths(-1);
                    }
                    else if (mo.method == "top3M")
                    {
                        date = date.AddMonths(-3);
                    }
                    else if (mo.method == "top6M")
                    {
                        date = date.AddMonths(-6);
                    }
                    else if (mo.method == "top1Y")
                    {
                        date = date.AddYears(-1);
                    }

                    var aa = objServ.GetByCountOrder(date);
                    foreach (var us in aa)
                    {
                        var mod = new ZakaznikModel(us.User);
                        mod.PocetLekci = us.PocetLekci;
                        resp.Items.Add(mod);
                    }

                    resp.Status = true;
                    break;
                }

                if (data.Count() > 0)
                {
                    if (included)
                    {
                        SetSessions();
                    }

                    foreach (var u in data)
                    {
                        var mod = new ZakaznikModel(u);

                        if (included)
                        {
                            if (u.KredityCas != null)
                            {

                                bool isSet = false;
                                int now = DateTimeExtensions.ToUnix(DateTime.Now);

                                var krcs = u.KredityCas.Where(d => d.PokladnaId == aa.Pobocka.PokladnaId && d.PlatnostDoUnix > now && d.PlatnostOdUnix < now && d.Kredit > 0);

                                mod.TypKreditu = "k";

                                if (krcs.Count() == 0)
                                {

                                }
                                else if (krcs.Count() == 1)
                                {
                                    var kr = krcs.FirstOrDefault();
                                    var zb = kr.PlatnostDoUnix - now;
                                    var d = (int)(zb / (60 * 60 * 24));
                                    mod.KreditCas = kr.Kredit;
                                    mod.KreditCasStr = $"{kr.Kredit} / {d}";
                                    mod.Color = "green";
                                    mod.TypKreditu = "kc";
                                    if (kr.PlatnostUnixBreak.HasValue && kr.PlatnostUnixBreak > 0) {
                                        mod.TypKreditu = "kb";
                                        mod.Color = "blue";
                                        mod.KreditCasStr = $"{kr.Kredit}/ st";
                                    }
                                    isSet = true;

                                }
                                // vice
                                else
                                {
                                    mod.KreditCas = krcs.Sum(d => d.Kredit);
                                    mod.KreditCasStr = $"{mod.KreditCas} / +";
                                    mod.Color = "green";
                                }
                                if (!isSet)
                                {

                                    mod.Kredit = 0;
                                    if (u.Kredity.Where(d => d.PokladnaId == aa.Pobocka.PokladnaId).Count() > 0)
                                    {
                                        var kr = u.Kredity.FirstOrDefault();
                                        mod.Kredit = kr.Kredit;
                                        if (kr.Kredit > 0)
                                        {
                                            mod.Color = "green";
                                        }
                                        else if (kr.Kredit < 0)
                                        {
                                            mod.Color = "red";
                                        }
                                        else
                                        {
                                            mod.Color = "silver";
                                        }
                                    }
                                }
                            }

                        }
                        resp.Items.Add(mod);

                    }
                }
                else
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nenalezeny záznamy", Type = JsonMessageType.Info });
                }

            } while (false);

            return Json(resp);

        }

        public IActionResult GetObjednavky(int? id)
        {
            JsonObjednavky resp = new JsonObjednavky();

            var data = objServ.GetObjednaneLekceUzivatele(id.Value);

            foreach (var lu in data)
            {
                resp.Items.Add(new JsonObjednavka(lu));
            }
            resp.Status = true;

            return Json(resp);

        }

        public IActionResult Receipt(int id)
        {

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                ViewBag.UserTransakce = objServ.GetTransakciById(id, true);

            } while (false);

            return PartialView(aa);

        }

        public IActionResult AddRemoveGroup(AddRemoveGroupModel m)
        {

            JsonStatus resp = new JsonStatus();

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                if (!aa.User.Id.HasValue)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není přihlášen uživatel", Type = JsonMessageType.Danger });
                    break;
                }

                if (!(m.UserId > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id uživatele", Type = JsonMessageType.Danger });
                    break;
                }

                if (!(m.GroupId > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id skupiny", Type = JsonMessageType.Danger });
                    break;
                }

                if (m.Value)
                {
                    User8GroupUser ug = new User8GroupUser() { GroupId = m.GroupId, UserId = m.UserId };
                    userServ.AddUserGroup(ug);
                    resp.Status = true;
                }
                else
                {
                    User8GroupUser ug = new User8GroupUser() { GroupId = m.GroupId, UserId = m.UserId };
                    resp.Status = userServ.RemoveUserGroup(ug);
                }

                if (resp.Status)
                {
                    resp.Messages.Add(new JsonMessage() { Text = $"Skupina změněna", Type = JsonMessageType.Success });
                }
                else
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Něco se pokazilo", Type = JsonMessageType.Danger });
                }

            } while (false);
            return Json(resp);


        }

        public IActionResult EditNote(ChangeUserModelBase m)
        {

            JsonStatus resp = new JsonStatus();

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                if (!aa.User.Id.HasValue)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není přihlášen uživatel", Type = JsonMessageType.Danger });
                    break;
                }

                if (!(m.UserId > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id uživatele", Type = JsonMessageType.Danger });
                    break;
                }

                var zak = userServ.GetById(m.UserId);
                if (zak == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nenačetl se uživatel", Type = JsonMessageType.Danger });
                    break;
                }

                zak.Poznamka = m.Temp;
                userServ.Update(zak);

                resp.Status = true;
                resp.Messages.Add(new JsonMessage() { Text = $"Poznámka změněna", Type = JsonMessageType.Success });


            } while (false);
            return Json(resp);


        }

        public IActionResult EditNote2(ChangeUserModelBase m)
        {

            JsonStatus resp = new JsonStatus();

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                if (!aa.User.Id.HasValue)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není přihlášen uživatel", Type = JsonMessageType.Danger });
                    break;
                }

                if (!(m.UserId > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id uživatele", Type = JsonMessageType.Danger });
                    break;
                }

                var zak = userServ.GetById(m.UserId);
                if (zak == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nenačetl se uživatel", Type = JsonMessageType.Danger });
                    break;
                }

                zak.Poznamka2 = m.Temp;
                userServ.Update(zak);

                resp.Status = true;
                resp.Messages.Add(new JsonMessage() { Text = $"Poznámka 2 změněna", Type = JsonMessageType.Success });


            } while (false);
            return Json(resp);


        }

        [HttpPost]
        public IActionResult SetDetailSwitch(ZakaznikSetSwitchModel model)
        {
            var resp = new JsonStatus();

            if (ModelState.IsValid)
            {
                var user = userServ.GetById(model.User);
                if (user != null)
                {
                    if (model.Item == "zakAkt")
                    {
                        user.Platnost = (model.Value == 1);
                        userServ.Update(user);
                        resp.Status = true;
                    }
                    else if (model.Item == "zakMayCheaper")
                    {
                        user.MayCheaper = (model.Value == 1);
                        userServ.Update(user);
                        resp.Status = true;

                    }
                    else
                    {
                        resp.Messages.Add(new JsonMessage() { Text = "Špataný typ", Type = JsonMessageType.Danger });
                    }
                }
                else
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není zákazník", Type = JsonMessageType.Danger });
                }

                if (resp.Status)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Změněno :)", Type = JsonMessageType.Success });
                }

            }

            return Json(resp);
        }

        public IActionResult DeleteUser(AddRemoveGroupModel m)
        {

            JsonStatus resp = new JsonStatus();

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                if (!aa.User.Id.HasValue)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není přihlášen uživatel", Type = JsonMessageType.Danger });
                    break;
                }

                if (!(m.UserId > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id uživatele", Type = JsonMessageType.Danger });
                    break;
                }

                User8 user = userServ.GetById(m.UserId);
                if (user == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id uživatele", Type = JsonMessageType.Danger });
                    break;
                }

                var objs = objServ.GetObjednaneLekceUzivatele(m.UserId);
                if (objs.Count > 0)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Zákazník má objednané lekce, nelze smazat", Type = JsonMessageType.Danger });
                    break;
                }

                var transakce = objServ.GetHistoriTransakci(m.UserId);
                var absolvovane = objServ.GetUskutecneneLekceUzivatele(m.UserId);
                if (transakce == null && absolvovane == null)
                {
                    userServ.Delete(user);
                    resp.Status = true;
                }
                else
                {
                    user.Email += "*";
                    user.UserName += "*";
                    user.Deleted = true;
                    user.DeletedDt = DateTime.Now;
                    user.DeletedByUserId = aa.User.Id;
                    userServ.Update(user);
                    resp.Status = true;
                }




                if (resp.Status)
                {
                    resp.Messages.Add(new JsonMessage() { Text = $"Zákazník byl smazán", Type = JsonMessageType.Success });
                }
                else
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Něco se pokazilo", Type = JsonMessageType.Danger });
                }

            } while (false);
            return Json(resp);


        }

        public IActionResult Edit(ZakaznikModel m)
        {

            JsonStatus resp = new JsonStatus();

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                if (!aa.User.Id.HasValue)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není přihlášen uživatel", Type = JsonMessageType.Danger });
                    break;
                }

                if (!(m.Id > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id uživatele", Type = JsonMessageType.Danger });
                    break;
                }

                User8 user = userServ.GetById(m.Id);
                if (user == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id uživatele", Type = JsonMessageType.Danger });
                    break;
                }



            } while (false);
            return Json(resp);


        }

        public IActionResult RestoreUser(int id)
        {

            JsonStatus resp = new JsonStatus();

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                if (!aa.User.Id.HasValue)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není přihášen uživatel", Type = JsonMessageType.Danger });
                    break;
                }

                if (!(id > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id uživatele", Type = JsonMessageType.Danger });
                    break;
                }

                User8 user = userServ.GetById(id);
                if (user == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id uživatele", Type = JsonMessageType.Danger });
                    break;
                }


                user.Email.TrimEnd('*');
                user.UserName.TrimEnd('*');
                user.Deleted = false;
                user.DeletedDt = null;
                user.DeletedByUserId = null;
                userServ.Update(user);
                resp.Status = true;


                if (resp.Status)
                {
                    resp.Messages.Add(new JsonMessage() { Text = $"Zákazník byl obnoven", Type = JsonMessageType.Success });
                }
                else
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Něco se pokazilo", Type = JsonMessageType.Danger });
                }

            } while (false);
            return Json(resp);


        }

        public ActionResult Excel()
        {
            SetMainPageValues();

            List<Excel> list = new List<Excel>();
            var zak = userServ.GetAll().Where(d => d.Poznamka != null);

            foreach (var z in zak)
            {

                string po = z.Poznamka;

                int count = po.Split(',').Length - 1;
                if (count == 0)
                {
                    string[] words = po.Split('-');
                    if (words.Length >= 4)
                    {
                        list.Add(new Excel(words));
                    }
                }
                else
                {
                    string[] users = po.Split(',');

                    foreach (var u in users)
                    {
                        string[] words = u.Split('-');
                       if (words.Length >= 4)
                        {
                            list.Add(new Excel(words));
                        }
                    }
                }

            }

            ViewBag.Excel = list.OrderBy(d => d.Col_01).ThenBy(d => d.Col_02).ToList();

            return View(aa);

        }

    }
}