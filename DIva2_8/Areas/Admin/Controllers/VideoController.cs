using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diva2.Controllers;
using Diva2.Core.Main.Users;
using Diva2.Core.Main.Videa;
using Diva2.Core.Model.Json;
using Diva2.Data;
using Diva2.Services.Managers.Content;
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
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Diva2Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeActionFilterAttribute]
    public class VideoController : BaseAdminController
    {
        private readonly IRuleService rulServ;

        
        

        public VideoController(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache,
                    IUser8Service userSer, ILogs8Service logSer,
                    IPobockaService pobSer, IVideoService vidSer, IObjednavkyService objSer) : base(dbContext, httpContextAccessor, memoryCache, userSer, pobSer, logSer, objSer)
        {
            this.videoServ = vidSer;
        }

        public ActionResult Index()
        {
            SetMainPageValues();


            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }

            var vids = videoServ.GetAll();
            IList<VideoModel> list = new List<VideoModel>();
            foreach (var vi in vids) {
                list.Add(new VideoModel(vi));
            }

            aa.Videos = list;

            return View(aa);
        }

        public ActionResult Sold()
        {
            SetMainPageValues();


            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Account/Login");
            }

            var vids = videoServ.GetUserVideosAll();


            ViewBag.SoldVideos = vids;

            return View(aa);
        }


        #region Video Create-Edit
        public ActionResult VideoCreate()
        {
            VideoModel m = new VideoModel();

            return PartialView("VideoCreate", m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VideoCreate(VideoModel m)
        {
            if (ModelState.IsValid)
            {
                Video db = new Video();
                m.CopyToDb(db);
                videoServ.Insert(db);
            }

            return PartialView("VideoCreate", m);
        }

        public ActionResult VideoEdit(int? id)
        {
            VideoModel m = new VideoModel();
            if (id.HasValue) {

                var db = videoServ.GetById(id.Value);
                if (db != null) {
                    m.CopyFromDb(db);
                }
            }

            return PartialView("VideoEdit", m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VideoEdit(VideoModel m)
        {
            if (ModelState.IsValid)
            {
                var db = videoServ.GetById(m.Id);
                if (db != null)
                {
                    m.CopyToDb(db);
                    videoServ.Update(db);
                }
            }

            return PartialView("VideoEdit", m);
        }
        #endregion


    }
}