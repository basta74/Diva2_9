using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diva2.Controllers;
using Diva2.Core;
using Diva2.Core.Main.Comunications;
using Diva2.Core.Main.Notifications;
using Diva2.Core.Main.Trans;
using Diva2.Core.Main.Users;
using Diva2.Core.Main.Videa;
using Diva2.Core.Model.Json;
using Diva2.Data;
using Diva2.Services.Managers.Content;
using Diva2.Services.Managers.Emails;
using Diva2.Services.Managers.Mains;
using Diva2.Services.Managers.Platby;
using Diva2.Services.Managers.Pobocky;
using Diva2.Services.Managers.Users;
using Diva2.Services.Managers.Videa;
using Diva2Web.Models.Admin;
using Diva2Web.Models.Content;
using Diva2Web.Models.Helpers;
using Diva2Web.Models.Inis;
using Diva2Web.Models.Responses;
using Diva2Web.Models.Users;
using Diva2Web.Models.Videos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Diva2Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeActionFilterAttribute]
    public class SmsController : BaseAdminController
    {
        private readonly IRuleService rulServ;


        private readonly IComunicationService commServ;
        private int PAGE_SIZE = 100;

        public SmsController(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache,
                    IUser8Service userSer, ILogs8Service logSer, IObjednavkyService objSe,
                    IPobockaService pobSer, IComunicationService commSe, IObjednavkyService objSer) : base(dbContext, httpContextAccessor, memoryCache, userSer, pobSer, logSer, objSer)
        {
            this.commServ = commSe;
            this.objServ = objSe;
        }

        [HttpGet("/Admin/Notifications")]
        [HttpGet("/Admin/Sms/Notifications")]
        public async Task<IActionResult> Notifications(string? status, string? user, int? lessonId, CancellationToken cancellationToken)
        {
            SetMainPageValues();
            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }

            var query = dbContext.UserNotifications.AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.Lesson)
                .Include(x => x.Deliveries).ThenInclude(x => x.UserDevice)
                .AsQueryable();

            if (lessonId.HasValue)
            {
                query = query.Where(x => x.LessonId == lessonId);
            }

            if (!string.IsNullOrWhiteSpace(user))
            {
                var value = user.Trim();
                if (int.TryParse(value, out var userId))
                {
                    query = query.Where(x => x.UserId == userId);
                }
                else
                {
                    query = query.Where(x => (x.User!.Email ?? "").Contains(value)
                        || (x.User.Jmeno + " " + x.User.Prijmeni).Contains(value));
                }
            }

            query = status?.ToLowerInvariant() switch
            {
                "created" => query.Where(x => x.SentAt == null && x.Error == null),
                "sent" => query.Where(x => x.SentAt != null),
                "delivered" => query.Where(x => x.DeliveredAt != null),
                "read" => query.Where(x => x.ReadAt != null),
                "reacted" => query.Where(x => x.Reaction != NotificationReaction.None),
                "error" => query.Where(x => x.Error != null),
                _ => query
            };

            aa.Title = "Notifikace";
            ViewBag.Notifications = await query.OrderByDescending(x => x.CreatedAt).Take(500).ToListAsync(cancellationToken);
            ViewBag.Status = status;
            ViewBag.UserFilter = user;
            ViewBag.LessonId = lessonId;
            return View(aa);
        }

        public ActionResult Index(int page = 1)
        {
            SetMainPageValues();


            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }

            IPagedList<SmsLog> smss = commServ.GetAll(page, PAGE_SIZE);
            aa.SetFromPaged(smss);
            ViewBag.smss = smss;
            return View(aa);
        }

        public ActionResult Errors(int page = 1)
        {
            SetMainPageValues();


            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }

            IPagedList<SmsLog> smss = commServ.GetErrorsAll(page, PAGE_SIZE);
            aa.SetFromPaged(smss);
            ViewBag.smss = smss;
            return View(aa);
        }

        public ActionResult Detail(int? id)
        {
            SmsLog m = new SmsLog();
            if (id.HasValue)
            {

                m = commServ.GetById(id.Value);
            }

            return PartialView("Detail", m);
        }

        public ActionResult Logout(int page = 1)
        {
            SetMainPageValues();


            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }

            var logs = objServ.GetLogOutAll(page, PAGE_SIZE);
            aa.SetFromPaged(logs);

            ViewBag.logs = logs;
            return View(aa);
        }

        public ActionResult Login(int page = 1)
        {
            SetMainPageValues();


            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }

            var logs = objServ.GetLogInAll(page, PAGE_SIZE);
            aa.SetFromPaged(logs);
            ViewBag.logs = logs;
            return View(aa);
        }


    }
}
