using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diva2.Controllers;
using Diva2.Core.Main.Users;
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

namespace Diva2Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeActionFilterAttribute]
    public class HelpController : BaseAdminController
    {
        public HelpController(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache,
                    IUser8Service userSer, ILogs8Service logSer,
                    IPobockaService pobSer, IRuleService rulSer, IObjednavkyService objSer) : base(dbContext, httpContextAccessor, memoryCache, userSer, pobSer, logSer, objSer)
        {
           
        }

        public ActionResult Index()
        {
            SetMainPageValues();

            return View(aa);
        }

        public ActionResult Rozvrh()
        {
            SetMainPageValues();

            return View(aa);
        }

        public ActionResult Zakaznik()
        {
            SetMainPageValues();

            return View(aa);
        }

        public ActionResult GenerovaniRozvrhu()
        {
            SetMainPageValues();

            return View(aa);
        }

        public ActionResult Video()
        {
            SetMainPageValues();

            return View(aa);
        }

        public ActionResult Platby()
        {
            SetMainPageValues();

            return View(aa);
        }

        public ActionResult Pages()
        {
            SetMainPageValues();

            return View(aa);
        }
    }
}