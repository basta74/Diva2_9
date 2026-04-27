using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diva2.Controllers;
using Diva2.Core.Main.Users;
using Diva2.Core.Model.Json;
using Diva2.Data;
using Diva2.Services.Managers.Content;
using Diva2.Services.Managers.Mains;
using Diva2.Services.Managers.Platby;
using Diva2.Services.Managers.Pobocky;
using Diva2.Services.Managers.Users;
using Diva2Web.Models.Admin;
using Diva2Web.Models.Content;
using Diva2Web.Models.Helpers;
using Diva2Web.Models.Inis;
using Diva2Web.Models.Responses;
using Diva2Web.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Diva2Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeActionFilterAttribute]
    public class ContentController : BaseAdminController
    {
        private readonly IRuleService rulServ;

        private Dictionary<string, string> caches = new Dictionary<string, string>();
        private IPageService pageServ;

        public ContentController(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache,
                    IUser8Service userSer, ILogs8Service logSer,
                    IPobockaService pobSer, IPageService pageSer, IObjednavkyService objSer) : base(dbContext, httpContextAccessor, memoryCache, userSer, pobSer, logSer, objSer)
        {
            this.pageServ = pageSer;
        }

        public ActionResult Index()
        {
            SetMainPageValues();


            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }

            return View(aa);
        }

        public ActionResult Pages()
        {
            SetMainPageValues();


            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }

            aa.Pages = pageServ.GetAll();

            return View(aa);
        }

        #region PageEdit
        public ActionResult PageEdit(int? id)
        {
            SetMainPageValues();


            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }

            if (id.HasValue)
            {
                aa.Page = new PageModel(pageServ.GetById(id.Value));
            }

            return View(aa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PageEdit(PageModel m)
        {
            JsonStatus resp = new JsonStatus();


            if (ModelState.IsValid)
            {
                var db = pageServ.GetById(m.Id);
                if (db != null)
                {
                    m.CopyToDb(db);
                    pageServ.Update(db);

                    resp.Status = true;
                }

            }

            return Json(resp);
        }
        #endregion

       
    }
}