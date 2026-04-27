using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diva2.Controllers;
using Diva2.Core;
using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Trans;
using Diva2.Core.Model.Json;
using Diva2.Data;
using Diva2.Services.Managers.Customers;
using Diva2.Services.Managers.Mains;
using Diva2.Services.Managers.Platby;
using Diva2.Services.Managers.Pobocky;
using Diva2.Services.Managers.Setting;
using Diva2.Services.Managers.Users;
using Diva2Web.Models.Admin;
using Diva2Web.Models.Export;
using Diva2Web.Models.Lekces;
using Diva2Web.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Diva2Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeActionFilter]
    public class BoardController : BaseAdminController
    {
        private ILekceAddonsService lekceAddServ;

        public BoardController(ApplicationDbContext dbContext,
                   IMemoryCache memoryCache, ILogger<HomeController> logger, IUser8Service userSer,
                   IHttpContextAccessor httpContextAccessor,
                   ILekceService lekceSer, ILekceTypService lekTypSer, IPobockaService pobSer, IObjednavkyService objSer,
                   ILogs8Service logSer, ILekceAddonsService leAddSer, IRuleService rulSer, ISkupinaZakaznikaService skupZakServ,
            ILektorService lkService, IPlatbaService plService, IWorkContext workContext) : base(dbContext, httpContextAccessor, memoryCache, userSer, pobSer, logSer, objSer)
        {
            lekceServ = lekceSer;
            objServ = objSer;
            lektorServ = lkService;
            lekceAddServ = leAddSer;
            lekceTypServ = lekTypSer;
            this.skupZakServ = skupZakServ;
        }

        // GET: Home
        public ActionResult Index(int? id)
        {

            SetMainPageValues();

            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }

            Lekce lekce = null;
            if (id.HasValue)
            {
                aa.Id = id;
                lekce = lekceServ.GetById(aa.Id.Value);
                if (lekce != null)
                {
                    if (lekce.PobockaId != aa.Pobocka.Id)
                    {
                        aa.Pobocka = aa.Pobocky.FirstOrDefault(d => d.Id == lekce.PobockaId);
                    }

                    aa.CurrentDate = lekce.Datum;
                    SessionSetCurrentDate(lekce.Datum);


                }
            }

            List<Lekce> lekces = lekceServ.GetByDay(aa.Pobocka.Id, aa.CurrentDate);
            ViewBag.LekceDen = lekces;

            if (!id.HasValue)
            {
                if (lekces.Count > 0)
                {
                    aa.Id = lekces.FirstOrDefault().Id;
                }
            }

            if (lekce == null && lekces.Count > 0)
            {
                lekce = lekces.FirstOrDefault();

            }

            aa.LekceOneBoard = new LekceBoardModel();
            aa.LekceOneBoard.Zakaznici = new List<LekceUserModel>();
            aa.RozvrhyObj = lekceServ.GetWeaksFromMonday(aa.Pobocka.Id, aa.CurrentDate);
            aa.Lektori = lektorServ.GetAll();
            aa.TypyLekci = lekceTypServ.GetAll();
            aa.SkupinyZakaznika = skupZakServ.GetAll();

            if (aa.Id.HasValue)
            {
                if (lekce != null)
                {
                    aa.LekceOneBoard.CopyFromDb(lekce);
                    aa.Messages = objServ.GetUserTextByLesson(aa.LekceOneBoard.Id);
                    aa.LekceOneBoard.Video = lekceAddServ.GetVideoById(lekce.Id);
                    var te = lekceAddServ.GetTextById(lekce.Id);
                    aa.LekceOneBoard.Text = te != null ? te.Text : "";
                    var zak = objServ.GetByLekce(aa.Id.Value);
                    foreach (var z in zak)
                    {
                        aa.LekceOneBoard.Zakaznici.Add(new LekceUserModel(z));
                    }
                }
            }

            return View(aa);
        }

        public IActionResult LessonUsers(int? id)
        {
            SetMainPageValues();

            Lekce lekce = null;
            if (id.HasValue)
            {
                lekce = lekceServ.GetById(id.Value);

                if (lekce != null)
                {
                    aa.LekceOneBoard = new LekceBoardModel(lekce);
                    aa.Messages = objServ.GetUserTextByLesson(aa.LekceOneBoard.Id);
                    var zak = objServ.GetByLekce(id.Value);

                    foreach (var z in zak)
                    {
                        aa.LekceOneBoard.Zakaznici.Add(new LekceUserModel(z));
                    }
                }
            }

            return PartialView(aa);
        }

        public IActionResult ShowLessonHistory(int? id)
        {

            JsonLessonHistory resp = new JsonLessonHistory();

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                if (!aa.User.Id.HasValue)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není přihášen uživatel" });
                    break;
                }

                if (!(id > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id hodiny" });
                    break;
                }

                var objs = objServ.GetChangesByLesson(id.Value);
                if (objs == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nenačetla se data" });
                    break;
                }

                resp.LekceUserChange = objs;
                resp.Status = true;

            } while (false);

            return Json(resp);
        }

        [HttpPost]
        public IActionResult ShowLessonUsers(int? id)
        {

            JsonUserLessonOrDay resp = new JsonUserLessonOrDay();

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                if (!(id > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id hodiny" });
                    break;
                }

                resp.Zakaznici = new List<LekceUserModel>();

                var zak = objServ.GetByLekce(id.Value);
                var messages = objServ.GetUserTextByLesson(id.Value).ToLookup(d => d.UserId);
                foreach (var z in zak.OrderBy(d => d.Poradi))
                {
                    var lum = new LekceUserModel(z);

                    var aaa = messages[z.UserId].FirstOrDefault();
                    if (aaa != null)
                    {
                        lum.Message = aaa.Text;
                    }
                    resp.Zakaznici.Add(lum);
                }

                resp.Status = true;

            } while (false);

            return Json(resp);
        }

        [HttpGet]
        public IActionResult ShowUserDay()
        {

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                DateTime dt = aa.CurrentDate;
                ViewBag.LookUp = objServ.GetByDay(dt, aa.Pobocka.Id).ToLookup(d => d.LekceId);

            } while (false);

            return PartialView();
        }

        [HttpGet]
        public IActionResult ShowLessonUserEmails(int? id)
        {

            JsonStatus resp = new JsonStatus();

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                if (!(id > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id hodiny" });
                    break;
                }


                resp.Meta = string.Join("; ", objServ.GetByLekce(id.Value).Select(d => d.User.Email).Distinct());
                resp.Status = true;

            } while (false);

            return Json(resp);
        }

        [HttpGet]
        public IActionResult ShowUserDayEmails()
        {
            JsonStatus resp = new JsonStatus();

            SetSessions();

            do
            {
                DateTime dt = aa.CurrentDate;
                resp.Meta = string.Join("; ", objServ.GetByDay(dt, aa.Pobocka.Id).Select(d => d.User.Email).Distinct());

            } while (false);

            return Json(resp);
        }

        [HttpGet]
        public IActionResult ShowLessonUserSms(int? id)
        {

            JsonStatus resp = new JsonStatus();

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                if (!(id > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id hodiny" });
                    break;
                }


                resp.Meta = string.Join("; ", objServ.GetByLekce(id.Value).Where(d => d.User.PhoneNumber != null).Select(d => d.User.PhoneNumber.Replace(" ", "")).Distinct());
                resp.Status = true;

            } while (false);

            return Json(resp);
        }

        [HttpGet]
        public IActionResult ShowUserDaySms()
        {
            JsonStatus resp = new JsonStatus();

            SetSessions();

            do
            {
                DateTime dt = aa.CurrentDate;
                resp.Meta = string.Join("; ", objServ.GetByDay(dt, aa.Pobocka.Id).Where(d => d.User.PhoneNumber != null).Select(d => d.User.PhoneNumber.Replace(" ","")).Distinct());

            } while (false);

            return Json(resp);
        }

        #region VideoEdit
        public ActionResult LessonVideoEdit(int id)
        {
            LekceVideoModel m = new LekceVideoModel();

            if (id > 0)
            {
                m.Id = id;
                var vi = lekceAddServ.GetVideoById(id);
                if (vi != null)
                {
                    m.Url = vi.Url;
                    m.TextUrl = vi.TextUrl;
                    m.Text = vi.Text;
                }
            }
            else
            {
                m.Text = "Připravili jsme pro vás speciální video k této lekci";
                m.TextUrl = "odkaz na video";
            }

            return PartialView("LessonVideoEdit", m);

        }

        [HttpPost]
        public IActionResult LessonVideoEdit(LekceVideoModel m)
        {

            if (ModelState.IsValid)
            {
                var dbObj = lekceAddServ.GetVideoById(m.Id);
                if (dbObj != null)
                {
                    if (m.Url == null)
                    {
                        lekceAddServ.Delete(dbObj);
                    }
                    else
                    {
                        dbObj.Url = m.Url;
                        dbObj.Text = m.Text;
                        dbObj.TextUrl = m.TextUrl;
                        lekceAddServ.Update(dbObj);
                    }
                }
                else
                {
                    dbObj = new LekceVideo();
                    dbObj.Id = m.Id;
                    dbObj.Url = m.Url;
                    dbObj.Text = m.Text;
                    dbObj.TextUrl = m.TextUrl;
                    dbObj.Dt = DateTime.Now;
                    lekceAddServ.Insert(dbObj);
                }
            }
            return PartialView("LessonVideoEdit", m);
        }
        #endregion

        public ActionResult LessonTextEdit(int id)
        {
            LekceTextModel model = new LekceTextModel();

            if (id > 0)
            {
                model.Id = id;
                var db = lekceAddServ.GetTextById(id);
                if (db != null)
                {
                    model.Text = db.Text;
                }
            }

            return PartialView("LessonTextEdit", model);

        }

        [HttpPost]
        public IActionResult LessonTextEdit(LekceTextModel model)
        {

            if (ModelState.IsValid)
            {
                var db = lekceAddServ.GetTextById(model.Id);
                if (db != null)
                {
                    if (model.Text == null || model.Text == "")
                    {
                        lekceAddServ.Delete(db);
                    }
                    else
                    {
                        db.Text = model.Text;
                        lekceAddServ.Update(db);
                    }
                }
                else
                {
                    db = new LekceText();
                    db.Id = model.Id;
                    db.Text = model.Text;
                    lekceAddServ.Insert(db);
                }
            }
            return PartialView("LessonTextEdit", model);
        }

        public ActionResult Excel(int id)
        {
            SetMainPageValues();

            Lekce lekce = null;
            if (id > 0)
            {
                lekce = lekceServ.GetById(id);

                List<int> ids = new List<int>();
                List<Excel> list = new List<Excel>();

                if (lekce != null)
                {
                    aa.LekceOneBoard = new LekceBoardModel(lekce);
                    var zak = objServ.GetByLekce(id);
                    foreach (var z in zak)
                    {
                        if (ids.Contains(z.UserId)) {
                            continue;
                        }
                        ids.Add(z.UserId);
                        string po = z.User.Poznamka;
                        if (po == null)
                        {
                            Excel ex = new Excel() { Col_01 = z.User.Prijmeni, Col_02 = z.User.Jmeno };
                            list.Add(ex);
                        }
                        else
                        {
                            int count = po.Split(',').Length - 1;
                            if (count == 0)
                            {
                                

                                string[] words = po.Split('-');
                                if (words.Length >= 4) {

                                    list.Add(new Excel(words));
                                }
                            }
                            else
                            {
                                string[] users = po.Split(',');

                                foreach(var u in users)
                                {
                                    string[] words = u.Split('-');
                                    if (words.Length >= 4)
                                    {
                                        list.Add(new Excel(words));
                                    }
                                }
                            }

                        }

                    }

                    ViewBag.Excel = list;
                }
            }

            return View(aa);

        }
    }
}