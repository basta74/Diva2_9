using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diva2.Controllers;
using Diva2.Core.Main.Users;
using Diva2.Core.Model.Json;
using Diva2.Data;
using Diva2.Services.Managers.Mains;
using Diva2.Services.Managers.Platby;
using Diva2.Services.Managers.Pobocky;
using Diva2.Services.Managers.Users;
using Diva2Web.Models.Admin;
using Diva2Web.Models.Helpers;
using Diva2Web.Models.Inis;
using Diva2Web.Models.Responses;
using Diva2Web.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Diva2.Core.Main.Lessons;

namespace Diva2Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeActionFilterAttribute]
    public class RulesController : BaseAdminController
    {
        private readonly IRuleService rulServ;

        private Dictionary<string, string> caches = new Dictionary<string, string>();


        public RulesController(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache,
                    IUser8Service userSer, ILogs8Service logSer,
                    IPobockaService pobSer, IRuleService rulSer, IObjednavkyService objSer) : base(dbContext, httpContextAccessor, memoryCache, userSer, pobSer, logSer, objSer)
        {
            rulServ = rulSer;


            caches.Add("ClearPobocky", "ClearPobocky");
            caches.Add("ClearZacatky", "ClearZacatky");
            caches.Add("ClearMain", "ClearMainIni");
        }

        // GET: Home
        public ActionResult Index()
        {
            SetMainPageValues();

            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }

            return View(aa);
        }

        public ActionResult Roles(int? id)
        {
            SetMainPageValues();

            if (!aa.User.HasRule("role_view"))
            {
                if (!aa.User.HasRule("all_able"))
                {
                    return Redirect("/Admin/Home/NoRule");
                }
            }

            aa.Id = id;
            ViewBag.Roles = rulServ.GetRoleAll();
            ViewBag.Rules = rulServ.GetAll();
            ViewBag.RulesRole = new List<int>();
            if (id.HasValue)
            {
                ViewBag.RulesRole = rulServ.GetRulesByRole(id.Value);
            }
            return View(aa);
        }

        public ActionResult Accounts(int? id)
        {

            SetMainPageValues();

            if (!aa.User.HasRule("users_role_view"))
            {
                if (!aa.User.HasRule("all_able"))
                {
                    return Redirect("/Admin/Home/NoRule");
                }

            }
            aa.Id = id;

            ViewBag.Roles = rulServ.GetRoleAll();

            if (id.HasValue)
            {
                aa.Users = userServ.GetByRole(id.Value);
            }


            return View(aa);
        }

        [HttpPost]
        public IActionResult SetRolesRule(SetRolesRuleModel model)
        {
            var resp = new JsonStatus();

            if (ModelState.IsValid)
            {
                RoleRule8 rr = rulServ.GetRoleRule(model.RoleId, model.RuleId);
                // pridavam
                if (model.Value == 1)
                {
                    if (rr == null)
                    {
                        rulServ.AddRuleToRole(new RoleRule8() { RoleId = model.RoleId, PravoId = model.RuleId });
                    }
                    else
                    {
                        resp.Messages.Add(new JsonMessage() { Text = "Nelze vložit, existuje" });
                    }
                }
                else
                {
                    if (rr != null)
                    {
                        rulServ.RemoveRuleFromRole(rr);
                    }
                    else
                    {
                        resp.Messages.Add(new JsonMessage() { Text = "Nelze odebrat, neexistuje" });
                    }

                }
                resp.Status = true;
            }

            return Json(resp);
        }

        public ActionResult AccountEdit(int id)
        {
            AccountEditModel model = new AccountEditModel();

            if (id > 0)
            {
                var mu = userServ.GetById(id);
                if (mu != null)
                {
                    model.User = mu;
                    model.Roles = rulServ.GetRoleAll();
                    model.UserRoles = rulServ.GetUserRoles(id);
                }
            }

            return PartialView("AccountEdit", model);

        }

        [HttpPost]
        public IActionResult SetUserRole(SetUserRoleModel model)
        {
            var resp = new JsonStatus();

            if (ModelState.IsValid)
            {
                do
                {
                    if (!(model.UserId > 0))
                    {
                        resp.Messages.Add(new JsonMessage() { Type = JsonMessageType.Warning, Text = "Není user" });
                        continue;
                    }

                    UserRoles8 rr = rulServ.GetUserRoles(model.UserId).Where(d => d.RoleId == model.RoleId).FirstOrDefault();
                    // pridavam
                    if (model.Value == 1)
                    {
                        if (rr == null)
                        {
                            rulServ.AddRoleToUser(new UserRoles8() { RoleId = model.RoleId, UserId = model.UserId });
                            resp.Messages.Add(new JsonMessage() { Type = JsonMessageType.Success, Text = "Přidáno" });
                        }
                        else
                        {
                            resp.Messages.Add(new JsonMessage() { Type = JsonMessageType.Warning, Text = "Nelze vložit, existuje" });
                            continue;
                        }
                    }
                    else
                    {
                        if (rr != null)
                        {
                            rulServ.RemoveRoleFromUser(rr);
                            resp.Messages.Add(new JsonMessage() { Type = JsonMessageType.Success, Text = "Odebráno" });

                        }
                        else
                        {
                            resp.Messages.Add(new JsonMessage() { Type = JsonMessageType.Warning, Text = "Nelze odebrat, neexistuje" });
                            continue;
                        }

                    }
                    resp.Status = true;

                } while (false);
            }



            return Json(resp);
        }


        public ActionResult AdminThings()
        {
            SetMainPageValues();

            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }
            if (!aa.User.HasRule("all_able"))
            {
                return Redirect("/Admin/Home/NoRule");
            }



            ViewBag.CaheItems = caches;

            return View(aa);
        }

        [HttpGet("/Admin/Rules/Logs")]
        public async Task<IActionResult> Logs(int? userId, int? lessonId, string text, CancellationToken cancellationToken)
        {
            SetMainPageValues();

            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }

            if (!aa.User.HasRule("all_able"))
            {
                return Redirect("/Admin/Home/NoRule");
            }

            var query = dbContext.MobileAppErrors.AsNoTracking().AsQueryable();
            if (userId.HasValue)
            {
                query = query.Where(x => x.UserId == userId.Value);
            }

            if (lessonId.HasValue)
            {
                query = query.Where(x => x.LessonId == lessonId.Value);
            }

            if (!string.IsNullOrWhiteSpace(text))
            {
                var search = text.Trim();
                query = query.Where(x => x.Message.Contains(search)
                    || x.ExceptionType.Contains(search)
                    || x.Page.Contains(search)
                    || x.Action.Contains(search));
            }

            var errors = await query
                .OrderByDescending(x => x.ReceivedAt)
                .Take(500)
                .ToListAsync(cancellationToken);
            var userIds = errors.Select(x => x.UserId).Distinct().ToList();
            var lessonIds = errors.Where(x => x.LessonId.HasValue).Select(x => x.LessonId!.Value).Distinct().ToList();
            var users = await dbContext.Users.AsNoTracking()
                .Where(x => userIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);
            var lessons = await dbContext.Set<Lekce>().AsNoTracking()
                .Where(x => lessonIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            var logs = errors.Select(error =>
            {
                users.TryGetValue(error.UserId, out var user);
                var lesson = error.LessonId.HasValue && lessons.TryGetValue(error.LessonId.Value, out var foundLesson)
                    ? foundLesson
                    : null;
                return new MobileAppErrorLogModel
                {
                    Id = error.Id,
                    UserId = error.UserId,
                    UserName = user == null ? string.Empty : $"{user.Prijmeni} {user.Jmeno}".Trim(),
                    UserEmail = user?.Email ?? string.Empty,
                    LessonId = error.LessonId,
                    LessonName = lesson?.Nazev ?? string.Empty,
                    LessonStartsAt = lesson?.DatumHodina,
                    OccurredAt = error.OccurredAt,
                    ReceivedAt = error.ReceivedAt,
                    Platform = error.Platform,
                    OsVersion = error.OsVersion,
                    DeviceManufacturer = error.DeviceManufacturer,
                    DeviceModel = error.DeviceModel,
                    AppVersion = error.AppVersion,
                    AppBuild = error.AppBuild,
                    Page = error.Page,
                    Action = error.Action,
                    ExceptionType = error.ExceptionType,
                    Message = error.Message,
                    StackTrace = error.StackTrace,
                    InnerException = error.InnerException
                };
            }).ToList();

            aa.Title = "Chyby mobilní aplikace";
            ViewBag.Logs = logs;
            ViewBag.UserId = userId;
            ViewBag.LessonId = lessonId;
            ViewBag.Text = text;
            return View(aa);
        }


        [HttpPost]
        public IActionResult ClearCache(string id)
        {
            var resp = new JsonStatus();

            if (caches.ContainsKey(id))
            {
                if (id == "ClearMainIni")
                {
                    pobServ.ClearMainIni();
                    resp.Status = true;
                }
                else if (id == "ClearPobocky")
                {
                    pobServ.ClearPobocky();
                    resp.Status = true;
                }
                else if (id == "ClearZacatky")
                {
                    pobServ.ClearZacatky();
                    resp.Status = true;
                }


            }


            return Json(resp);
        }
    }
}
