using Diva2.Core;
using Diva2.Core.Main;
using Diva2.Core.Main.PayGates;
using Diva2.Core.Main.Platby;
using Diva2.Core.Main.Videa;
using Diva2.Core.Model.Json;
using Diva2.Core.Model.Money;
using Diva2.Data;
using Diva2.Services;
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

    public class VideoController : BaseAdminController
    {
        private readonly ILogger<HomeController> _logger;

        private IPageService pageServ;
        private IVideoService videoServ;
        private IWorkContext workContext;


        public VideoController(ApplicationDbContext dbContext,
            IMemoryCache memoryCache, ILogger<HomeController> logger,
            IHttpContextAccessor httpContextAccessor,
            ILekceService lekceSer, ILekceTypService lekTypSer, IPobockaService pobSer,
            IObjednavkyService objSer, IUser8Service userSer, ILogs8Service logServ,
            ILekceAddonsService leAddSe, IPageService pageSer, IVideoService vidSer,
            ILektorService lkService, IPlatbaService plService, IWorkContext workContext) : base(dbContext, httpContextAccessor, memoryCache, userSer, pobSer, logServ, objSer)
        {
            objServ = objSer;
            videoServ = vidSer;
            this.workContext = workContext;
            pageServ = pageSer;
            _logger = logger;


            cache = new CacheHelper(memoryCache, dbContext.SubDomain);
        }

        private void LoadForPublic()
        {

            aa.Pages = pageServ.GetVisibleForMenu();

        }

        public IActionResult Index()
        {

            SetMainPageValues();
            LoadUserProperty(base.objServ);
            LoadForPublic();

            aa.Page = new Diva2Web.Models.Content.PageModel(pageServ.GetByType(Core.Main.Content.PageType.video));

            List<VideoModel> videos = new List<VideoModel>();
            foreach (var vi in videoServ.GetVisible())
            {
                var vim = new VideoModel(vi);
                if (vim.Image == null || vim.Image == "")
                {
                    vim.Image = "/images/default-video-thumbnail.jpg";
                }
                videos.Add(vim);
            }

            aa.Videos = videos;

            return View(aa);
        }


        public IActionResult Detail(int id)
        {

            TrySetUserFromSess(aa.User);


            VideoModel vi = new VideoModel(videoServ.GetById(id));
            vi.UserId = (aa.User.Id.HasValue) ? aa.User.Id.Value : 0;
            if (vi.Image == null || vi.Image == "")
            {
                vi.Image = "/images/default-video-thumbnail.jpg";
            }

            return PartialView("Detail", vi);
        }


        public IActionResult MyVideos()
        {

            SetMainPageValues();
            LoadUserProperty(base.objServ);
            LoadForPublic();

            IList<UserVideoModel> videos = new List<UserVideoModel>();
            if (aa.User.Id.HasValue)
            {
                var vids = videoServ.GetUserVideos(aa.User.Id.Value, true);
                foreach (var video in vids)
                {
                    if (video.Image == null || video.Image == "")
                    {
                        video.Image = "/images/default-video-thumbnail.jpg";
                    }
                    videos.Add(new UserVideoModel(video));
                }
            }
            ViewBag.UserVideos = videos;

            return View(aa);
        }

        public IActionResult MyVideoView(int id)
        {
            SetMainPageValues();
            LoadUserProperty(objServ);
            LoadForPublic();

            UserVideoModel vi = new UserVideoModel();
            if (id > 0)
            {

                var video = videoServ.GetUserVideoById(id);

                if (video != null)
                {

                    if (video.Aktivovano == true)
                    {
                        if (video.Zobrazeno == false)
                        {
                            video.Zobrazeno = true;
                            video.ZobrazenoDt = DateTime.Now;
                            videoServ.Update(video);
                        }
                    }

                    if (video.Image == null || video.Image == "")
                    {
                        video.Image = "/images/default-video-thumbnail.jpg";
                    }

                    vi = new UserVideoModel(video);
                }
                ViewBag.UserVideo = vi;
                return View(aa);
            }
            else {

                return Redirect("/Video/Index");

            }
            
        }


        [HttpPost]
        public IActionResult Active(int id)
        {

            JsonStatus resp = new JsonStatus();

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
                    resp.Messages.Add(new JsonMessage() { Text = "Není id videa" });
                    break;
                }

                var video = videoServ.GetUserVideoById(id);

                if (video == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není video" });
                    break;
                }


                video.Aktivovano = true;
                video.AktivovanoDt = DateTime.Now;

                videoServ.Update(video);

                resp.Status = true;


            } while (false);

            return Json(resp);
        }
    }
}
