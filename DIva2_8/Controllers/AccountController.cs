using Diva2.Core;
using Diva2.Core.Extensions;
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
using Diva2Web.Areas.Admin;
using Diva2Web.Areas.Admin.Controllers;
using Diva2Web.Models;
using Diva2Web.Models.Account;
using Diva2Web.Models.Responses;
using Diva2Web.Models.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Web.Controllers
{

    public class AccountController : BaseAdminController
    {
        /*
        private readonly SignInManager<User8> signInManager;
        private readonly UserManager<User8> userManager;
        /**/

        private readonly IEmailSenderService emailSender;
        private IComunicationService comServ;
        private IRuleService rulServ;
        private bool useIdentity = true;
        private readonly SignInManager<User8> signInManager;
        private readonly UserManager<User8> userManager;

        public AccountController(ApplicationDbContext dbContext,
                   IMemoryCache memoryCache, ILogger<HomeController> logger, IUser8Service userSer, IObjednavkyService objSer,
                   IHttpContextAccessor httpContextAccessor, ILogs8Service logSer,
                   IPobockaService pobSer, IEmailSenderService emailSender, IComunicationService emailServ,
                   IRuleService rulSer, SignInManager<User8> signInManager, UserManager<User8> userManager,
                   ILekceTypService leTySe, ILektorService lekSe, IPlatbaService plaSe) : base(dbContext, httpContextAccessor, memoryCache, userSer, pobSer, logSer, objSer)
        {
            this.emailSender = emailSender;
            this.userServ = userSer;
            this.comServ = emailServ;
            this.rulServ = rulSer;
            this.signInManager = signInManager;
            this.userManager = userManager;

            this.lekceTypServ = leTySe;
            this.lektorServ = lekSe;
            this.platbaServ = plaSe;
        }

        public IActionResult Index()
        {
            return View();
        }



        public IActionResult Login()
        {
            UserModelLogin mm = new UserModelLogin();
            return View(mm);
        }

        public IActionResult LoginModal()
        {
            UserModelLogin mm = new UserModelLogin();
            return PartialView(mm);
        }

        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = "/Home/Index" });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);

        }


        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {

            returnUrl = returnUrl ?? Url.Content("~/");

            UserModelLogin m = new UserModelLogin()
            {
                ReturnUrl = returnUrl,
                Providers = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };


            if (remoteError != null)
            {
                ModelState.AddModelError("", remoteError);
                return View("Login", m);
            }

            var info = signInManager.GetExternalLoginInfoAsync().Result;

            if (info == null)
            {
                ModelState.AddModelError("", "Chyba při nahrání externích přihlašovacích informací");
                return View("Login", m);
            }

            var signInResult = signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Result.Succeeded)
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    var user = userManager.FindByEmailAsync(email).Result;

                    AddUserProperties(user);
                    return LocalRedirect(returnUrl);
                }
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {

                    var user = userManager.FindByEmailAsync(email).Result;
                    if (user == null)
                    {
                        user = new User8()
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Name),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            NormalizedEmail = info.Principal.FindFirstValue(ClaimTypes.Email),
                            NormalizedUserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            GdprBase = true,
                            GdprBaseDT = DateTime.Now
                        };

                        await userManager.CreateAsync(user);

                    }

                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);

                    AddUserProperties(user);

                    return LocalRedirect(returnUrl);
                }
            }
            return View(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginAsync(UserModelLogin param)
        {

            if (ModelState.IsValid)
            {
                if (param.Nick != null && param.Nick.Length > 0 & param.Heslo != null && param.Heslo.Length > 0)
                {


                    if (useIdentity)
                    {
                        var result = await signInManager.PasswordSignInAsync(param.Nick, param.Heslo, false, true);
                        if (result.Succeeded)
                        {
                            var user = await userManager.FindByNameAsync(param.Nick);

                            if (user.Platnost == true)
                            {


                                AddUserProperties(user);


                                return Redirect($"/Home/Index");
                            }
                            else
                            {
                                return Redirect($"/Account/NoActive/" + user.Id);
                            }
                        }
                        else
                        {
                            param.MsgAddDanger("Nejsou zadané platné údaje");
                            return View(param);
                        }
                    }
                    else
                    {

                        User8 user = userServ.GetByNamePassword(param.Nick, param.Heslo);
                        if (user != null)
                        {

                            if (user.Platnost == true)
                            {


                                AddUserProperties(user);
                                UserModel um = new UserModel(user);
                                SetUserToSession(um);

                                return Redirect($"/Home/Index");
                            }
                            else
                            {
                                return Redirect($"/Account/NoActive/" + user.Id);
                            }
                        }
                        else
                        {
                            param.MsgAddDanger("Nejsou zadané platné údaje");
                            return View(param);
                        }
                    }
                }
            }
            else
            {
                param.MsgAddDanger("Nejsou zadané platné údaje");
                return View(param);
            }


            return View(param);
        }


        public async Task<IActionResult> LogoutAsync()
        {
            ClearSessions();
            await signInManager.SignOutAsync();
            return Redirect("/Home");

        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Lockout()
        {
            return View();
        }

        #region Register Active

        public IActionResult NoActive(int id)
        {
            return View("ActiveNo", id);
        }

        public IActionResult Register()
        {

            UserModelRegister u = new UserModelRegister();

            return PartialView("Register", u);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(UserModelRegister model)
        {
            do
            {
                if (ModelState.IsValid)
                {
                    SetMainPageValues();
                    var usrs = userServ.GetForRegister(model.Email);
                    if (usrs.Count > 0)
                    {
                        ModelState.AddModelError("Email", "Email se už používá. pokud jste majitelem zašlete si nové heslo");
                        break;
                    }

                    User8 user = new User8
                    {
                        Jmeno = model.Jmeno,
                        Prijmeni = model.Prijmeni,
                        Email = model.Email,
                        UserName = model.Email,
                        Ulice = model.Ulice,
                        Posta = model.Posta,
                        PhoneNumber = model.Telefon,
                        GdprBase = true,
                        GdprBaseDT = DateTime.Now
                    };
                    user.Nazev = $"{user.Prijmeni} {user.Jmeno}";

                    if (useIdentity)
                    {
                        var result = await userManager.CreateAsync(user, model.NewPass);
                        if (result.Succeeded)
                        {

                            if (aa.MainIniCover.MainIniObj.RegisterEmail)
                            {
                                JsonStatus resp = new JsonStatus();

                                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                                var callback = Url.Action("ConfirmEmail", "Account", new { code, id = user.Id }, Request.Scheme);

                                comServ.SendRegisterMessage(user, callback, resp);
                            }
                            else
                            {

                                user.Platnost = true;
                                userServ.Update(user);

                            }
                            rulServ.AddRoleToUser(new UserRoles8() { RoleId = 6, UserId = user.Id });
                        }

                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("NewPass", error.Description);
                            }
                            return PartialView("Register", model);
                        }
                    }
                    else
                    {
                        user.PasswordHash = User8Service.Hash(model.NewPass).ToLower();
                        if (aa.MainIniCover.MainIniObj.RegisterEmail)
                        {
                            userServ.Insert(user);
                            JsonStatus resp = new JsonStatus();
                            comServ.SendRegisterMessage(user, "", resp);
                        }
                        else
                        {
                            user.Platnost = true;
                            userServ.Update(user);
                        }
                        rulServ.AddRoleToUser(new UserRoles8() { RoleId = 6, UserId = user.Id });

                    }



                }
                else
                {
                    return PartialView("Register", model);
                }

                return Redirect("RegisterOk");
            } while (false);

            return PartialView("Register", model);

        }


        public IActionResult Active(string id)
        {

            UserModelUpdateForgotPassword model = new UserModelUpdateForgotPassword();
            model.JsonStatus = new JsonStatus();

            do
            {
                if (id.Length > 2)
                {

                    int length = 0;
                    int.TryParse(id.Substring(0, 1), out length);

                    if (!(length > 0))
                    {
                        model.JsonStatus.Messages.Add(new JsonMessage() { Text = "Nepovedlo se rozklíčovat kód 1" });
                        break;
                    }

                    int userId = 0;
                    int.TryParse(id.Substring(id.Length - length), out userId);

                    if (!(userId > 0))
                    {
                        model.JsonStatus.Messages.Add(new JsonMessage() { Text = "Nepovedlo se rozklíčovat kód 2" });
                        break;
                    }
                    model.Id = userId;
                    var user = userServ.GetById(userId);
                    if (user == null)
                    {
                        model.JsonStatus.Messages.Add(new JsonMessage() { Text = "Nepovedlo se rozklíčovat kód 4" });
                        break;
                    }

                    user.Platnost = true;
                    model.JsonStatus.Status = true;
                    userServ.Update(user);
                }
            } while (false);


            return View("Active", model);

        }



        public async Task<IActionResult> SendActiveEmailAsync(int? id)
        {
            var ret = new JsonStatus();

            if (id.HasValue)
            {
                var user = userServ.GetById(id.Value);
                if (user != null)
                {
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callback = Url.Action("ConfirmEmail", "Account", new { code, id = user.Id }, Request.Scheme);

                    comServ.SendRegisterMessage(user, callback, ret);
                }
                else
                {
                    ret.Messages.Add(new JsonMessage() { Type = JsonMessageType.Danger, Text = "Nenalezl se uživatel." });
                }
            }
            else
            {
                ret.Messages.Add(new JsonMessage() { Type = JsonMessageType.Danger, Text = "Nenalezl se uživatel." });
            }
            return Json(ret);
        }

        public IActionResult RegisterOk()
        {
            SetMainPageValues();
            return View(aa);
        }


        public async Task<IActionResult> ConfirmEmailAsync(UserConfirmEmailModel m)
        {
            m.JsonStatus = new JsonStatus();

            var user = await userManager.FindByIdAsync(m.Id.ToString());
            if (user != null)
            {
                var result = userManager.ConfirmEmailAsync(user, m.Code);
                if (!result.IsCompletedSuccessfully)
                {
                    foreach (var error in result.Result.Errors)
                    {
                        m.JsonStatus.MsgAddDanger(error.Description);
                        ModelState.TryAddModelError(error.Code, error.Description);
                    }
                    return View(m);
                }
                else
                {
                    user.Platnost = true;
                    userServ.Update(user);
                    m.JsonStatus.Status = true;
                }
            }

            return View(m);
        }

        #endregion

        #region Detail

        public IActionResult Detail()
        {
            SetMainPageValues();
            UserModel u = new UserModel();

            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Home/index");
            }
            var user = userServ.GetById(aa.User.Id.Value);
            if (user != null)
            {
                u.CopyFromDb(user);
            }
            return View(u);
        }

        public IActionResult DetailEdit()
        {
            SetMainPageValues();
            UserModel u = new UserModel();
            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Home/index");
            }
            var user = userServ.GetById(aa.User.Id.Value);
            if (user != null)
            {
                u.CopyFromDb(user);
            }
            return PartialView("DetailEdit", u);
        }


        [HttpPost]
        public IActionResult DetailEdit(UserModel model)
        {
            if (ModelState.IsValid)
            {
                do
                {
                    var user = userServ.GetById(model.Id.Value);
                    if (user != null)
                    {
                        if (user.Email != model.Email)
                        {
                            var usrs = userServ.GetByEmail(model.Email);
                            if (usrs.Count > 1)
                            {
                                ModelState.AddModelError("Email", "Email se už používá ve více případech");
                                continue;
                            }
                            else if (usrs.Count == 1)
                            {
                                if (usrs.FirstOrDefault().Id != model.Id)
                                {
                                    ModelState.AddModelError("Email", $"Email {model.Email} se už používá u jiného uživatele");
                                    continue;
                                }
                            }
                            user.NormalizedEmail = model.Email.ToUpperInvariant();
                            user.NormalizedUserName = model.Email.ToUpperInvariant();
                        }

                        model.CopyToDb(user);
                        userServ.Update(user);
                    }
                } while (false);
            }
            return PartialView("DetailEdit", model);
        }

        public IActionResult PasswordEdit()
        {
            SetMainPageValues();
            UserModelPassword u = new UserModelPassword();

            if (!aa.User.Id.HasValue)
            {
                return Redirect("/Home/index");
            }

            u.Id = aa.User.Id.Value;

            return PartialView("PasswordEdit", u);
        }

        [HttpPost]
        public async Task<IActionResult> PasswordEditAsync(UserModelPassword model)
        {
            SetMainPageValues();

            if (ModelState.IsValid)
            {


                if (useIdentity)
                {
                    var user = await userManager.GetUserAsync(User);
                    var result = await userManager.ChangePasswordAsync(user, model.OldPass, model.NewPass);
                    if (result.Succeeded)
                    {

                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                }
                else
                {
                    var us = userServ.GetById(model.Id);

                    string hash = User8Service.Hash(model.OldPass).ToLower();
                    bool isSame = us.PasswordHash == hash;

                    if (model.NewPass != model.NewPass2)
                    {
                        ModelState.AddModelError("NewPass", $"Nová hesla se neshodují");
                    }
                    else
                    {
                        if (isSame)
                        {
                            us.PasswordHash = User8Service.Hash(model.NewPass).ToLower();
                            userServ.Update(us);
                        }
                        else
                        {
                            ModelState.AddModelError("OldPass", $"Původmí heslo není platné");
                        }

                    }
                }

            }

            return PartialView("PasswordEdit", model);
        }

        #endregion

        #region Forgot Password

        public IActionResult ChangePassword(ChangePasswordModel m)
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

                if (!(m.UserId > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id uživatele", Type = JsonMessageType.Danger });
                    break;
                }


                if (!aa.User.HasRule("heslo_edit"))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nemáte práva pro zaúčtování", Type = JsonMessageType.Danger });
                    break;
                }

                if (m.KontCislo1 != m.KontCislo2)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nesouhlasí kontrolní čísla", Type = JsonMessageType.Danger });
                    break;
                }

                if (m.Heslo == null || m.Heslo.Length == 0)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není zadáno heslo", Type = JsonMessageType.Danger });
                    break;
                }

                if (m.Heslo.Length < 6)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Heslo musí mít aspoň 6 znaků", Type = JsonMessageType.Danger });
                    break;
                }

                if (useIdentity)
                {
                    var user = userManager.FindByIdAsync(m.UserId.ToString()).Result;
                    if (user == null)
                    {
                        resp.Messages.Add(new JsonMessage() { Text = "Neexistuje platný uživatel", Type = JsonMessageType.Danger });
                        break;
                    }

                    string token = userManager.GeneratePasswordResetTokenAsync(user).Result;
                    var resetPassResult = userManager.ResetPasswordAsync(user, token, m.Heslo).Result;
                    if (!resetPassResult.Succeeded)
                    {
                        foreach (var error in resetPassResult.Errors)
                        {
                            ModelState.TryAddModelError(error.Code, error.Description);
                            resp.Messages.Add(new JsonMessage() { Text = error.Description, Type = JsonMessageType.Danger });
                        }
                    }

                }
                else
                {
                    var us = userServ.GetById(m.UserId);
                    us.PasswordHash = User8Service.Hash(m.Heslo).ToLower();
                    userServ.Update(us);
                }



                resp.Messages.Add(new JsonMessage() { Text = $"Heslo bylo úspěšně změněno na <b>{m.Heslo}</b>", Type = JsonMessageType.Success });

                resp.Status = true;


            } while (false);
            return Json(resp);


        }

        public IActionResult ResetPassword(string token, string email)
        {

            SetMainPageValues();

            var model = new UserResetPasswordModel() { Token = token, Email = email };

            return View("ResetPassword", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(UserResetPasswordModel m)
        {


            if (!ModelState.IsValid)
            {
                return View(m);
            }
            var user = await userManager.FindByEmailAsync(m.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordError");
            }
            var resetPassResult = await userManager.ResetPasswordAsync(user, m.Token, m.NewPass);
            if (!resetPassResult.Succeeded)
            {
                m.JsonStatus = new JsonStatus();
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                    m.JsonStatus.Messages.Add(new JsonMessage() { Text = error.Description, Type = JsonMessageType.Danger });
                }
                return View(m);
            }
            return RedirectToAction("ResetPasswordSuccess");
        }

        [HttpGet]
        public IActionResult ResetPasswordSuccess()
        {

            return View();
        }

        public IActionResult ResetPasswordError()
        {


            return View();
        }


        public IActionResult UpdatePassword(string id)
        {

            UserModelUpdateForgotPassword model = new UserModelUpdateForgotPassword();
            model.JsonStatus = new JsonStatus();

            do
            {
                if (id.Length > 2)
                {

                    int length = 0;
                    int.TryParse(id.Substring(0, 1), out length);

                    if (!(length > 0))
                    {
                        model.JsonStatus.Messages.Add(new JsonMessage() { Text = "Nepovedlo se rozklíčovat kód 1" });
                        break;
                    }

                    int userId = 0;
                    int.TryParse(id.Substring(id.Length - length), out userId);

                    if (!(userId > 0))
                    {
                        model.JsonStatus.Messages.Add(new JsonMessage() { Text = "Nepovedlo se rozklíčovat kód 2" });
                        break;
                    }
                    model.Id = userId;
                    var user = userServ.GetById(userId);
                    if (user == null)
                    {
                        model.JsonStatus.Messages.Add(new JsonMessage() { Text = "Nepovedlo se rozklíčovat kód 4" });
                        break;
                    }

                    model.JsonStatus.Status = true;

                }
            } while (false);


            return View("UpdatePassword", model);

        }

        [HttpPost]
        public IActionResult UpdatePassword(UserModelUpdateForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                if (model.NewPass != model.NewPass2)
                {
                    ModelState.AddModelError("NewPass2", "Hesla se neshodují");
                }

                var user = userServ.GetById(model.Id);
                if (user != null)
                {

                    user.PasswordHash = User8Service.Hash(model.NewPass).ToLower();
                    userServ.Update(user);

                }
                else
                {
                    ModelState.AddModelError("NewPass2", "Systémová chyba, oslovte prosím obsluhu");
                }

                return Redirect($"/Account/UpdatePasswordOk");
            }

            return View("UpdatePassword", model);

        }

        [HttpGet]
        public IActionResult SendForgotEmail(string id)
        {

            JsonStatus resp = new JsonStatus();

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                if ((id == null || id == ""))
                {
                    resp.Messages.Add(new JsonMessage() { Type = JsonMessageType.Warning, Text = "Není zadán email" });
                    break;
                }

                var users = userServ.GetByEmail(id);


                if (users == null)
                {
                    resp.Messages.Add(new JsonMessage() { Type = JsonMessageType.Warning, Text = "Email nebyl nalezen" });
                    break;
                }
                else if (users.Count == 0)
                {
                    resp.Messages.Add(new JsonMessage() { Type = JsonMessageType.Warning, Text = $"Uživatel s emailem <b>{id}</b> nenalezen" });
                    break;
                }
                else if (users.Count > 1)
                {

                    resp.Messages.Add(new JsonMessage() { Type = JsonMessageType.Warning, Text = "Email je v systému zadán vícekrát. Nelze zaslat" });
                    break;
                }


                var user = users.FirstOrDefault();

                if (user == null)
                {
                    resp.Messages.Add(new JsonMessage() { Type = JsonMessageType.Warning, Text = $"Uživatel s emailem <b>{id}</b> nenalezen" });
                    break;
                }

                resp.Messages.Add(new JsonMessage() { Type = JsonMessageType.Success, Text = "Email nalezen" });

                if (useIdentity)
                {

                    string token = userManager.GeneratePasswordResetTokenAsync(user).Result;
                    var callback = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme);
                    comServ.SendZapomenuteHeslo(user, callback, resp);
                }
                else
                {
                    comServ.SendZapomenuteHeslo(user, "", resp);
                }



            } while (false);


            return Json(resp);


        }

        public IActionResult UpdatePasswordOk()
        {
            return View();
        }
        #endregion

        #region Gdpr

        public IActionResult SetGdpr(int? id)
        {
            var ret = new JsonStatus();

            if (id.HasValue)
            {
                var user = userServ.GetById(id.Value);
                if (user != null)
                {
                    user.GdprBase = true;
                    user.GdprBaseDT = DateTime.Now;
                    userServ.Update(user);

                    UserModel u = new UserModel(user);
                    SetUserToSession(u);

                    ret.Status = true;
                }
            }
            else
            {
                ret.Messages.Add(new JsonMessage() { Type = JsonMessageType.Danger, Text = "Nenalezl se uživatel." });
            }

            return Json(ret);
        }

        public IActionResult SetNews1(int? id)
        {
            var ret = new JsonStatus();

            if (id.HasValue)
            {
                var user = userServ.GetById(id.Value);
                if (user != null)
                {
                    user.GdprNews = true;
                    user.GdprNewsDT = DateTime.Now;
                    userServ.Update(user);

                    UserModel u = new UserModel(user);
                    SetUserToSession(u);

                    ret.Status = true;
                }
            }
            else
            {
                ret.Messages.Add(new JsonMessage() { Type = JsonMessageType.Danger, Text = "Nenalezl se uživatel." });
            }

            return Json(ret);
        }

        public IActionResult SetNews0(int? id)
        {
            var ret = new JsonStatus();

            if (id.HasValue)
            {
                var user = userServ.GetById(id.Value);
                if (user != null)
                {
                    user.GdprNews = false;
                    user.GdprNewsDT = DateTime.Now;
                    userServ.Update(user);

                    UserModel u = new UserModel(user);
                    SetUserToSession(u);

                    ret.Status = true;
                }
            }
            else
            {
                ret.Messages.Add(new JsonMessage() { Type = JsonMessageType.Danger, Text = "Nenalezl se uživatel." });
            }

            return Json(ret);
        }

        #endregion


    }
}