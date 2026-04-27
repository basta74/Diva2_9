using Diva2.Core;
using Diva2.Core.Main.Users;
using Diva2.Core.Model.Json;
using Diva2.Data;
using Diva2.Services.Emailing;
using Diva2.Services.Managers.Emails;
using Diva2.Services.Managers.Mains;
using Diva2.Services.Managers.Platby;
using Diva2.Services.Managers.Pobocky;
using Diva2.Services.Managers.Setting;
using Diva2.Services.Managers.Users;
using Diva2Web.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Diva2Web.Controllers
{
    public class PdaController : BaseAdminController
    {
        private IEmailSenderService emailSender;
        private IRuleService rulServ;

        public PdaController(ApplicationDbContext dbContext,
                   IMemoryCache memoryCache, ILogger<HomeController> logger, IUser8Service userSer, IObjednavkyService objSer,
                   IHttpContextAccessor httpContextAccessor, ILogs8Service logSer,
                   IPobockaService pobSer, IEmailSenderService emailSender, IComunicationService emailServ,
                   IRuleService rulSer,
                   ILekceTypService leTySe, ILektorService lekSe, IPlatbaService plaSe) : base(dbContext, httpContextAccessor, memoryCache, userSer, pobSer, logSer, objSer)
        {
            this.emailSender = emailSender;
            this.userServ = userSer;
            this.comServ = emailServ;
            this.rulServ = rulSer;

            this.lekceTypServ = leTySe;
            this.lektorServ = lekSe;
            this.platbaServ = plaSe;
        }

        public IActionResult Login(string email, string pass)
        {
            JsonUser resp = new JsonUser();
            if (email != null && email.Length > 0 & pass != null && pass.Length > 0)
            {
                User8 user = userServ.GetByNamePassword(email, pass);
                if (user != null)
                {
                    resp.Status = true;
                    resp.Id = user.Id;
                    resp.Name = $"{user.Prijmeni} {user.Jmeno}";
                    resp.Valid = user.Platnost;
                }
            }
            return Json(resp);
        }


        public IActionResult Company()
        {

            Company comp = pobServ.GetCompany();
            comp.Lectors = lektorServ.GetAll();
            comp.LessonTypes = lekceTypServ.GetAll();

            foreach (var br in comp.Branches)
            {
                br.PaymentsCredit = platbaServ.GetKreditAll(br.PokladnaId);
                br.PaymentsTime = platbaServ.GetKreditCasAll(br.PokladnaId);
            }

            return Json(comp);

        }
    }
}
