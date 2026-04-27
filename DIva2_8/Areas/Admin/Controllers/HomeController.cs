using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Diva2.Controllers;
using Diva2.Data;
using Diva2.Services.Managers.Mains;
using Diva2.Services.Managers.Platby;
using Diva2.Services.Managers.Pobocky;
using Diva2.Services.Managers.Users;
using Diva2Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Diva2Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeActionFilterAttribute]
    public class HomeController : BaseAdminController
    {
        private IRuleService pravaServ;

        public HomeController(ApplicationDbContext dbContext,
                   IMemoryCache memoryCache, ILogger<HomeController> logger, IUser8Service userSer,
                   IHttpContextAccessor httpContextAccessor, ILogs8Service logSer, IRuleService rulSer,
                   IPobockaService pobSer, IObjednavkyService objSer) : base(dbContext, httpContextAccessor,memoryCache,userSer, pobSer, logSer, objSer)
        {
            pravaServ = rulSer;
        }

        // GET: Home
        public ActionResult Index()
        {
            
            SetMainPageValues();
            var prava = pravaServ.GetAll();
            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }

            return View(aa);
        }

        // GET: Home
        public ActionResult NoRule()
        {

            SetMainPageValues();

            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }

            return View(aa);
        }

        public IActionResult SetDate(string dat)
        {
            DateTime dtNow = DateTime.Now;
            if(DateTime.TryParse(dat, out dtNow)){
                SessionSetCurrentDate(dtNow);
            }

            string urlAnterior = Request.Headers["Referer"].ToString();

            return Redirect(urlAnterior);
        }

        public IActionResult SetDateBoard(string id)
        {
            DateTime dtNow = DateTime.Now;
            if (DateTime.TryParse(id, out dtNow))
            {
                SessionSetCurrentDate(dtNow);
            }

            return Redirect("/Admin/Board");
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



            string urlAnterior = Request.Headers["Referer"].ToString().ToLower();

            if (urlAnterior.Contains("admin/board/index/")) { 
                int pos = urlAnterior.LastIndexOf("/");
                urlAnterior = urlAnterior.Substring(0, pos);
            }

            return Redirect(urlAnterior);
        }

    }
}