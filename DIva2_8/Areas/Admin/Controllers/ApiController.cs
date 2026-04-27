using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Diva2.Core.Extensions;
using Diva2.Core.Main.Comunications;
using Diva2.Core.Main.PayGates;
using Diva2.Core.Main.Platby;
using Diva2.Core.Main.Trans;
using Diva2.Core.Main.Users;
using Diva2.Core.Main.Videa;
using Diva2.Core.Model.Calendar;
using Diva2.Core.Model.Json;
using Diva2.Core.Model.Money;
using Diva2.Data;
using Diva2.Services;
using Diva2.Services.Managers.Calendar;
using Diva2.Services.Managers.Customers;
using Diva2.Services.Managers.Emails;
using Diva2.Services.Managers.Mains;
using Diva2.Services.Managers.Platby;
using Diva2.Services.Managers.Pobocky;
using Diva2.Services.Managers.Setting;
using Diva2.Services.Managers.Users;
using Diva2.Services.Managers.Videa;
using Diva2Web.Models.Admin;
using Diva2Web.Models.Helpers;
using Diva2Web.Models.Lekces;
using Diva2Web.Models.Platby;
using Diva2Web.Models.Responses;
using Diva2Web.Models.Trans;
using Diva2Web.Models.Zakaznici;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using JsonAddMoney = Diva2.Core.Model.Json.JsonAddMoney;

namespace Diva2Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ApiController : BaseAdminController
    {


        private CultureInfo ci = CultureInfo.InvariantCulture;


        public ApiController(ApplicationDbContext dbContext,
           IMemoryCache memoryCache, ILogger<HomeController> logger,
           IHttpContextAccessor httpContextAccessor,
           ISkupinaZakaznikaService skupZakServ, IPobockaService pobSer, IUser8Service userSer,
           ILekceTypService ltService, ILekceMustrService lmuService, ILekceService lekceService,
            ILektorService lkService, IPlatbaService plService, ILogs8Service logSer, ICalendarService calSe,
            IObjednavkyService objSer, IComunicationService emailServ, IVideoService viSe) : base(dbContext, httpContextAccessor, memoryCache, userSer, pobSer, logSer, objSer)
        {
            //cache = new Diva2.Services.CacheHelper(memoryCache, dbContext.SubDomain);

            this.lektorServ = lkService;
            this.lekceMustrServ = lmuService;
            this.lekceTypServ = ltService;
            this.skupinaZakaznikaServ = skupZakServ;
            this.lekceServ = lekceService;
            this.platbaServ = plService;
            this.objServ = objSer;
            this.pobServ = pobSer;
            this.comServ = emailServ;
            this.videoServ = viSe;
            this.calServ = calSe;
        }

        /// <summary>
        /// Vrati typy plateb
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetPays(int id)
        {
            JsonPlatby resp = new JsonPlatby();
            SetSessions();
            if (id > 0)
            {

                if (id == 1 || id == 2)
                {
                    resp.Name = "Kreditní platby ";
                    IEnumerable<PlatbaKredit> li = null;
                    if (id == 1)
                    {
                        li = platbaServ.GetKreditAll(aa.Pobocka.PokladnaId).Where(d => d.Kategorie == 1 && d.Platnost == true).OrderBy(d => d.Castka);
                    }
                    else
                    {
                        resp.Name += " slev.";
                        li = platbaServ.GetKreditAll(aa.Pobocka.PokladnaId).Where(d => d.Kategorie == 2 && d.Platnost == true).OrderBy(d => d.Castka);
                    }
                    if (li.Count() > 0)
                    {
                        foreach (var p in li)
                        {
                            resp.Items.Add(new Diva2.Core.Model.Json.PlatbaBaseModel() { Id = p.Id, Popis = p.Popis, Castka = p.Castka });
                        }
                    }
                    resp.Status = true;
                }
                else if (id == 3 || id == 4)
                {
                    resp.Name = "Kreditní časové platby ";
                    IEnumerable<PlatbaKreditCas> li = null;
                    if (id == 3)
                    {
                        li = platbaServ.GetKreditCasAll(aa.Pobocka.PokladnaId).Where(d => d.Kategorie == 1 && d.Platnost == true).OrderBy(d => d.Castka);
                    }
                    else
                    {
                        resp.Name += " slev.";
                        li = platbaServ.GetKreditCasAll(aa.Pobocka.PokladnaId).Where(d => d.Kategorie == 2 && d.Platnost == true).OrderBy(d => d.Castka);
                    }
                    if (li.Count() > 0)
                    {
                        foreach (var p in li)
                        {
                            string pop = $"{p.PocetMesicu}m - {p.Popis}";
                            resp.Items.Add(new Diva2.Core.Model.Json.PlatbaBaseModel() { Id = p.Id, Popis = pop, Castka = p.Castka });
                        }
                    }
                    resp.Status = true;
                }
                else if (id == 5 || id == 6)
                {
                    resp.Name = "Časový paušál ";
                    IEnumerable<PlatbaCas> li = null;
                    if (id == 5)
                    {
                        li = platbaServ.GetCasAll(aa.Pobocka.PokladnaId).Where(d => d.Kategorie == 1).OrderBy(d => d.Castka);
                    }
                    else
                    {
                        resp.Name += " slev.";
                        li = platbaServ.GetCasAll(aa.Pobocka.PokladnaId).Where(d => d.Kategorie == 2).OrderBy(d => d.Castka);
                    }
                    if (li.Count() > 0)
                    {
                        foreach (var p in li)
                        {
                            resp.Items.Add(new Diva2.Core.Model.Json.PlatbaBaseModel() { Id = p.Id, Popis = p.Popis, Castka = p.Castka });
                        }

                    }
                    resp.Status = true;

                }
            }

            resp.Name += $" {resp.Items.Count()} pol.";

            return Json(resp);

        }

        public IActionResult LessonAdd(AddRemoveLessonModel m)
        {

            JsonAddRemoveUserLesson resp = new JsonAddRemoveUserLesson();

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                if (!aa.User.Id.HasValue)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není přihášen uživatel" });
                    break;
                }

                if (!(m.UserId > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id uživatele" });
                    break;
                }
                if (!(m.AktId > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id hodiny" });
                    break;
                }

                resp.LekceId = m.AktId;

                var objs = objServ.GetByLekce(m.AktId, false);
                var lek = lekceServ.GetById(m.AktId);

                if (lek == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nenačetla se lekce", Type = JsonMessageType.Danger });
                    break;
                }

                if (lek.Zauctovano)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Lekce je již zaúčtovaná", Type = JsonMessageType.Danger });
                    break;
                }

                resp.Lekce = lek;

                int krCasCount = 0;
                int krNormCount = 0;

                int objedKrCount = objServ.GetObjednaneLekceUzivatele(m.UserId).Select(d => d.Lekce).Sum(d => d.Kredit);
                int objedPocet = objServ.GetPocetObjednavekUzivatele(m.UserId);
                int pocetObjednavekUzivateleNaLekci = objs.Where(d => d.UserId == m.UserId).Count();

                int maxMinus = aa.GetIniPobInt("maxKreditDoMinusu");
                int maxMinus1 = aa.GetIniPobInt("maxKreditDoMinusu1");
                int iniLimitMax = (objedPocet > 0) ? maxMinus : maxMinus1;


                LekceUser lu = new LekceUser();
                lu.UserId = m.UserId;
                lu.LekceId = m.AktId;
                lu.PobockaId = aa.Pobocka.Id;
                lu.PokladnaId = aa.Pobocka.PokladnaId;
                lu.Poradi = objs.Count() + 1;

                lu.KontCislo = objServ.GetRandom(objs);
                lu.Nahradnik = objs.Count() >= lek.PocetMist;
                lu.NahradnikJa = objs.Count() >= lek.PocetMist;

                lu.Premiera = objedPocet == 0;

                lu.Aktivni = true;
                lu.DoMzdy = true;

                lu.Unix = DateTimeExtensions.ToUnix(lek.Datum);
                lu.Datum = lek.Datum;


                UserLekceLogIn chl = new UserLekceLogIn(lu)
                {
                    PocetZakazniku = objs.Count(),
                    PocetMist = lek.PocetMist,
                    ProvedlId = aa.User.Id.Value,
                    ObjednanychLekci = objedPocet,
                    ObjednanychVLekci = pocetObjednavekUzivateleNaLekci,
                    KreditInit = iniLimitMax,
                    KreditLekce = lek.Kredit,
                    Ts = DateTime.Now,
                    ObjednaneKredity = objedKrCount,
                    FromAdministrace = m.Administrace
                };



                UserZbytek zb = objServ.GetZbytekUzivatele(m.UserId, false);
                bool pridat = false;
                /* ----------- kreditNeomezeny ----------------- */

                if (aa.GetIniPobBool("kreditNeomezeny"))
                {

                    if (pridat == false)
                    {
                        if (aa.GetIniPobInt("lekce_max_count_per_user") > 0)
                        {
                            int mcpu = aa.GetIniPobInt("lekce_max_count_per_user");
                            chl.MaxPerUser = mcpu;
                            if (pocetObjednavekUzivateleNaLekci >= mcpu)
                            {
                                if (!m.Administrace)
                                {
                                    resp.MsgAdd($"Maximální možný počet objedávek u standardního kreditu na 1 hodinu je {mcpu} na účet jednoho zákazníka");
                                    // break
                                }
                            }
                        }
                    }
                    krNormCount = zb.Kredity.Where(d => d.Key == aa.Pobocka.PokladnaId).FirstOrDefault().Value;

                    chl.ZbytekKredit = krNormCount;

                    if ((krCasCount + krNormCount + iniLimitMax) >= (objedKrCount + lek.Kredit))
                    {
                        pridat = true;
                    }
                    else
                    {
                        pridat = false;
                    }

                }


                // ----------- kreditCasovy -----------------
                if (aa.GetIniPobBool("kreditCasovy"))
                {
                    krCasCount = zb.KredityCas.Where(d => d.PlatnostDoUnix > lu.Unix).Select(d => d.Kredit).Sum();
                    chl.ZbytekKreditCas = krCasCount;


                    var krCasFirst = zb.KredityCas.FirstOrDefault();
                    if (krCasCount > 0)
                    {
                        if (!m.Administrace)
                        {
                            // NE muze se platit z normalniho kreditu
                            if (!(krNormCount >= lek.Kredit))
                            {
                                chl.MaxPerUserTime = krCasFirst.PocetLidi;

                                if (pocetObjednavekUzivateleNaLekci >= krCasFirst.PocetLidi)
                                {
                                    resp.MsgAdd($"Maximální možný počet objednávek u časového kreditu na 1 hodinu je {krCasFirst.PocetLidi} na účet jednoho zákazníka");
                                    // break;
                                }
                            }
                        }
                        if ((krCasCount + krNormCount + iniLimitMax) >= (objedKrCount + lek.Kredit))
                        {
                            pridat = true;
                        }
                        else
                        {
                            pridat = false;
                        }

                    }

                }

                chl.Pridat = pridat;

                if (pridat == false)
                {
                    if (!m.Administrace)
                    {
                        resp.MsgAdd($"Společně s objednaným kreditem ({objedKrCount})  nedosahujete na novou objednávku za {lek.Kredit}kr. Vaše kredity neom. {krNormCount} + časové:{krCasCount}");
                        //break;
                    }
                }

                if (m.Administrace)
                {
                    chl.Pridat = true;
                }

                if (!chl.Pridat){
                    break;
                }

                zb.SetZbytekToActualLekceUser(lu, lek);

                resp.Nahradnik = lu.Nahradnik;
                resp.NahradnikJa = lu.NahradnikJa;

                if (!m.Administrace && aa.GetIniPobBool("lekceOdhlasNabidniVolne") == true)
                {
                    var pasivni = objs.Where(d => d.Aktivni == false).OrderBy(d => d.Poradi).FirstOrDefault();
                    if (pasivni != null)
                    {
                        objs.Remove(pasivni);
                        objServ.Delete(pasivni);
                    }
                }

                objs.Add(lu);
                objServ.Insert(lu);
                objServ.ClearObjednaneLekceUzivatele(m.UserId);

                lek.PocetZakazniku = objs.Where(d => d.Aktivni == true).Count();
                lekceServ.Update(lek);

                UserLekceChange ch = new UserLekceChange() { LekceId = lek.Id, UserId = m.UserId, ProvedlId = aa.User.Id.Value, Status = "+", Ts = DateTime.Now };
                objServ.AddUserChange(ch);

                objServ.AddUserChangeLogIn(chl);

                if (m.Text != "" && m.Text != null)
                {

                    objServ.Insert(new UserText() { LekceId = lek.Id, KontrolniCislo = lu.KontCislo, UserId = lu.UserId, Text = m.Text });
                }

                resp.Status = true;

            } while (false);
            return Json(resp);


        }


        public IActionResult LessonRemoveFrom(AddRemoveLessonModel m)
        {
            JsonAddRemoveUserLesson resp = new JsonAddRemoveUserLesson();

            SetSessions();
            TrySetUserFromSess(aa.User);

            do
            {
                if (!aa.User.Id.HasValue)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Neni prihlasen uzivatel" });
                    break;
                }

                if (!(m.AktId > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Neni id hodiny" });
                    break;
                } /**/

                if (!(m.Id > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Neni identifikator" });
                    break;
                }

                resp.LekceId = m.AktId;

                var objs = objServ.GetByLekce(m.AktId, false);

                var obj1 = objs.Where(d => d.Id == m.Id).FirstOrDefault();

                if (obj1 == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "nelze odebrat" });
                    break;
                }

                if (m.UserId == 0)
                {
                    m.UserId = obj1.UserId;
                }

                var lek = lekceServ.GetById(m.AktId);

                if (lek == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "nelze odebrat" });
                    break;
                }

                resp.Lekce = lek;

                User8 u = null;

                UserLekceChange ch = new UserLekceChange()
                {
                    LekceId = lek.Id,
                    UserId = m.UserId,
                    ProvedlId = aa.User.Id.Value,
                    Status = "-",
                    Ts = DateTime.Now,

                };

                UserLekceLogOut chl = new UserLekceLogOut(ch)
                {

                    Poradi = obj1.Poradi,
                    PocetZakazniku = objs.Count(),
                    PocetMist = lek.PocetMist

                };

                SmsContent sms = null;

                // je nejaky nahradnik
                if (lek.PocetZakazniku > lek.PocetMist)
                {
                    chl.ExistujeNahradnik = true;
                    // je platny
                    if (obj1.Poradi <= lek.PocetMist)
                    {
                        chl.JePlatny = true;

                        var nextZak = objs.Where(d => d.Poradi > lek.PocetMist).OrderBy(d => d.Poradi).FirstOrDefault();
                        chl.SmsUserId = nextZak.UserId;
                        if (nextZak.UserId != obj1.UserId)
                        {
                            if(nextZak.Nahradnik){
                                nextZak.NahradnikJa = false;
                                objServ.Update(nextZak);
                            }

                            if (aa.MainIniCover.MainIniObj.SmsActive)
                            {
                                chl.SmsActive = aa.MainIniCover.MainIniObj.SmsActive;
                                u = userServ.GetById(nextZak.UserId);
                                if (u.PhoneNumber != null && u.PhoneNumber != "")
                                {

                                    int.TryParse(u.PhoneNumber.Replace(" ", ""), out int ph);
                                    if (ph > 100000000)
                                    {

                                        DateTime dt = lek.Datum.Add(lek.Cas);

                                        sms = new SmsContent()
                                        {
                                            Name = aa.MainIniCover.MainIniObj.SmsName,
                                            Pass = aa.MainIniCover.MainIniObj.SmsPass,
                                            Number = ph,
                                            Text = $"V terminu {dt.ToString("dd.MM. HH:mm")} se uvolnilo misto a Vy se posouvate mezi zakazniky. Na tuto zpravu prosím neodpovidejte {aa.SubDomain}.diva2.cz"
                                        };
                                    }
                                }
                            }
                        }
                    }

                    if (sms != null)
                    {
                        int respId = -1;
                        SmsLog smsL = new SmsLog()
                        {
                            UserId = aa.User.Id.Value,
                            PobockaId = aa.Pobocka.Id,
                            Cislo = sms.Number.ToString(),
                            ZakaznikId = m.UserId,
                            Email = u.Email,
                            LekceId = lek.Id,
                            Stav = respId,
                            Text = sms.Text,
                            CreateUnix = DateTimeExtensions.ToUnix(DateTime.Now)
                        };

                        try
                        {
                            respId = SendSms(sms);
                            smsL.Credit = sms.Credit;
                            smsL.Stav = respId;
                            if (respId == 0)
                            {
                                smsL.SendUnix = DateTimeExtensions.ToUnix(DateTime.Now);
                                smsL.Vyrizeno = true;
                                chl.SmsStatus = respId;
                            }
                        }
                        catch (Exception ex)
                        {

                            smsL.Exception = ex.InnerException.ToString();
                        }
                        finally
                        {

                            smsL.Kod = sms.Kod;
                            smsL.Result = sms.Result;
                            comServ.Insert(smsL);
                        }

                    }


                }

                var msg = objServ.GetUserTextByLessonNumber(obj1.UserId, obj1.LekceId, obj1.KontCislo);
                if (msg != null)
                {
                    objServ.Delete(msg);
                }


                if (m.Akce == "del")
                {
                    objs.Remove(obj1);
                    objServ.Delete(obj1);
                    var changePoradi = objs.Where(d => d.Poradi > obj1.Poradi);
                    foreach (var obj in changePoradi)
                    {
                        obj.Poradi--;
                    }
                    objServ.Update(objs);
                    objServ.Update(changePoradi);
                    objServ.ClearObjednaneLekceUzivatele(m.UserId);
                }
                else if (m.Akce == "nab")
                {
                    obj1.Aktivni = false;
                    objServ.Update(obj1);
                    objServ.Delete(msg);
                }



                lek.PocetZakazniku = objs.Where(d => d.Aktivni == true).Count();
                lekceServ.Update(lek);

                resp.ZustaneVLekci = objs.Where(d => d.UserId == m.UserId).Count() > 0;


                objServ.AddUserChange(ch);
                objServ.AddUserChangeLog(chl);

                resp.Status = true;

            } while (false);

            return Json(resp);
        }

        /// <summary>
        /// Zauctovani hodiny
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostLesson(LekceZauctovatModel m)
        {

            JsonAddRemoveUserLesson resp = new JsonAddRemoveUserLesson();

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                if (!aa.User.Id.HasValue)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není přihášen uživatel", Type = JsonMessageType.Danger });
                    break;
                }

                if (!aa.User.HasRule("spin_close"))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nemáte práva pro zaúčtování", Type = JsonMessageType.Danger });
                    break;
                }

                if (m.KontCislo1 != m.KontCislo2)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nesouhlasí kontrolní čísla", Type = JsonMessageType.Danger });
                    break;
                }

                if (!(m.LekceId > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není lekce id", Type = JsonMessageType.Danger });
                    break;
                }

                var lek = lekceServ.GetById(m.LekceId);

                if (lek == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není lekce", Type = JsonMessageType.Danger });
                    break;
                }

                if (lek.Zauctovano)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Lekce je již zaúčtovaná", Type = JsonMessageType.Danger });
                    break;
                }

                var objs = objServ.GetByLekce(m.LekceId);
                if (objs.Count == 0)
                {
                }
                else
                {

                    int i = 0;
                    foreach (LekceUser ou in objs)
                    {

                        UserTransakce tran = new UserTransakce();
                        tran.UserId = ou.UserId;
                        tran.PokladnaId = aa.Pobocka.PokladnaId;
                        tran.ProvedlId = aa.User.Id.Value;
                        tran.Status = "-";
                        DateTime noe = DateTime.Now;
                        var dt = lek.Datum;
                        dt = dt.Add(lek.Cas);
                        tran.UnixTime = DateTimeExtensions.ToUnix(dt);
                        tran.Kredit = lek.Kredit;
                        tran.LekceId = lek.Id;
                        tran.Datum = noe;

                        bool zauctovano = objServ.RemoveKredit(tran, ++i, true);
                        objServ.ClearZbytekUzivatele(ou.UserId);
                        objServ.ClearHistoriTransakci(ou.UserId);
                        objServ.ClearHistorieRoky(ou.UserId);
                        var zbytek = objServ.GetZbytekUzivatele(ou.UserId);
                        zbytek.SetZbytekToActualLekceUser(ou, lek);
                        objServ.ClearZbytekVObjednavkachUzivatele(ou.UserId);
                    }
                    /**/
                }

                lek.Zauctovano = true;
                lek.ZauctovalUserId = aa.User.Id.Value;
                lek.ZauctovalUserName = aa.User.CeleJmeno;
                lek.ZauctovanoDate = DateTime.Now;

                lekceServ.Update(lek);
                /**/
                resp.Status = true;
                resp.Messages.Add(new JsonMessage() { Text = "Lekce se zaúčtovala", Type = JsonMessageType.Success });
                break;
            } while (false);

            return Json(resp);
        }

        public IActionResult AddMoney(AddMoneyModel m)
        {

            JsonAddMoney resp = new JsonAddMoney();

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

                if (!(m.TypPlatbyId > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není kategorie plateb", Type = JsonMessageType.Danger });
                    break;
                }

                if (!(m.PlatbaId > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není platba", Type = JsonMessageType.Danger });
                    break;
                }

                if (m.KontCislo1 != m.KontCislo2)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nesouhlasí kontrolní čísla", Type = JsonMessageType.Danger });
                    break;
                }

                AddMoneyTrans t = new AddMoneyTrans()
                {
                    ProvedlId = aa.User.Id.Value,
                    PlatbaId = m.PlatbaId,
                    TypPlatbyId = m.TypPlatbyId,
                    PokladnaId = pobocka.PokladnaId,
                    UserId = m.UserId,
                    DoPokladny = 1,
                    DateFrom = m.DateFrom,
                    DateTo = m.DateTo

                };

                var trans = platbaServ.AddMoney(t, resp);

                if (trans.IsOk)
                {
                    if (t.TypPlatbyId == 1 || t.TypPlatbyId == 2)
                    {
                        objServ.AddKredit(trans);
                    }
                    else if (t.TypPlatbyId == 3 || t.TypPlatbyId == 4)
                    {
                        objServ.AddKreditTime(trans);
                    }

                    UpdateZbytek(m.UserId, t.PokladnaId, resp);
                }

            } while (false);
            return Json(resp);


        }

        public IActionResult RemoveMoney(AddMoneyModel m)
        {

            JsonAddMoney resp = new JsonAddMoney();

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


                if (!(m.PlatbaId > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není platba", Type = JsonMessageType.Danger });
                    break;
                }

                if (m.KontCislo1 != m.KontCislo2)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nesouhlasí kontrolní čísla", Type = JsonMessageType.Danger });
                    break;
                }

                AddMoneyTrans t = new AddMoneyTrans()
                {
                    ProvedlId = aa.User.Id.Value,
                    PlatbaId = m.PlatbaId,
                    TypPlatbyId = m.TypPlatbyId,
                    PokladnaId = pobocka.PokladnaId,
                    UserId = m.UserId,
                    DoPokladny = 1,
                    DateFrom = m.DateFrom,
                    DateTo = m.DateTo,
                    Plus = false

                };

                int tranId = t.PlatbaId;


                var trans = platbaServ.AddMoney(t, resp);

                var tranObj = objServ.GetUserCreditsTimeById(m.PlatbaId);

                if (tranObj == null)
                {
                    resp.MsgAddDanger("Nenačetla se transakce");
                }

                var platbaObj = platbaServ.GetKreditCasById(tranObj.PlatbaId);
                if (platbaObj == null)
                {
                    resp.MsgAddDanger("Nenačetla se transakce");
                }

                trans.Typ = "kc";
                trans.PlatbaTime = platbaObj;
                trans.PlatbaId = platbaObj.Id;
                trans.Kredit = platbaObj.Kredity;
                trans.Castka = (platbaObj.Castka * -1);
                trans.Zbytek = 0;


                bool ok = objServ.ReturnKredit(tranId, trans);

                if (ok)
                {
                    resp.MsgAddSuccess("Kredity se odebraly");
                    UpdateZbytek(m.UserId, t.PokladnaId, resp);
                    resp.Status = true;
                }
                else
                {
                    resp.MsgAddDanger("Kredity se neodebraly");
                }

            } while (false);
            return Json(resp);


        }

        public IActionResult VideoBuy(JsonBuyVideo m)
        {

            JsonAddMoney resp = new JsonAddMoney();

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


                if (!(m.VideoId > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není video", Type = JsonMessageType.Danger });
                    break;
                }

                var video = videoServ.GetById(m.VideoId);

                if (video == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není video*", Type = JsonMessageType.Danger });
                    break;
                }



                UserTransakce tran = new UserTransakce();
                tran.UserId = m.UserId;
                tran.PokladnaId = aa.Pobocka.PokladnaId;
                tran.ProvedlId = aa.User.Id.Value;
                tran.Status = "-";
                tran.UnixTime = DateTimeExtensions.ToUnix(DateTime.Now);
                tran.Kredit = video.Kredity;
                tran.VideoId = video.Id;
                tran.Datum = DateTime.Now;

                bool zauctovano = objServ.RemoveKredit(tran, 1, false);

                if (zauctovano == false)
                {
                    resp.MsgAddDanger("Není dostatek kreditů na ůčtu");
                    break;
                }

                objServ.ClearZbytekUzivatele(m.UserId);
                objServ.ClearHistoriTransakci(m.UserId);
                objServ.ClearHistorieRoky(m.UserId);
                var zbytek = objServ.GetZbytekUzivatele(m.UserId);

                UserVideo uv = new UserVideo();
                uv.VideoId = video.Id;
                uv.UserId = m.UserId;
                uv.Zaplaceno = true;
                uv.Kredity = video.Kredity;
                uv.Nazev = video.Name;
                uv.Url = video.Url;
                if (video.Image == null || video.Image == "")
                {
                    video.Image = "/images/default-video-thumbnail.jpg";
                }
                uv.Image = video.Image;
                uv.ZaplacenoDt = DateTime.Now;

                videoServ.Insert(uv);

                resp.Status = true;

                resp.MsgAddSuccess("Video bylo zakoupeno, shlédnout múžete v sekci <a href=\"/moje-videa\"><b>Moje Videa</b></a>");


            } while (false);
            return Json(resp);


        }

        private void UpdateZbytek(int userId, int pokId, JsonAddMoney resp)
        {

            objServ.ClearZbytekUzivatele(userId);
            objServ.ClearHistoriTransakci(userId);
            objServ.ClearHistorieRoky(userId);
            objServ.ClearZbytekVObjednavkachUzivatele(userId);



            var Zbytek = objServ.GetZbytekUzivatele(userId);
            resp.Kredity = Zbytek.KredityItem(pokId);
            var zb = Zbytek.KredityTimeItem(pokId, DateTime.Now);
            if (zb != null)
            {
                resp.KredityCasove = $"{zb.Kredit} / {zb.ZbyvaDni}";
            }

            resp.Status = true;
        }

        /// <summary>
        /// Vloze kredit bokem bez pokladny
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public IActionResult AddCredit0(AddCredit0Model m)
        {

            JsonAddMoney resp = new JsonAddMoney();

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

                if (m.KontCislo1 != m.KontCislo2)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nesouhlasí kontrolní čísla", Type = JsonMessageType.Danger });
                    break;
                }

                bool isOk = false;

                var tran = new UserTransakce();
                tran.UserId = m.UserId;
                tran.PokladnaId = pobocka.PokladnaId;
                tran.ProvedlId = aa.User.Id.Value;
                tran.Datum = DateTime.Now;
                tran.UnixTime = DateTimeExtensions.ToUnix(DateTime.Now);
                tran.PlatnostOdUnix = null;
                tran.PlatnostDoUnix = null;
                tran.PlatnostOd = null;
                tran.PlatnostDo = null;
                tran.DoPokladny = 0;
                tran.Status = "+";

                string text = "";

                tran.Typ = "k";
                tran.Kredit = m.Kredit;
                tran.Castka = 0;

                objServ.AddKredit(tran);
                text = $"Připsáno <b>{tran.Kredit}</b> kreditů";
                isOk = true;

                if (isOk)
                {
                    resp.Messages.Add(new JsonMessage() { Text = text, Type = JsonMessageType.Success });
                    UpdateZbytek(m.UserId, aa.Pobocka.PokladnaId, resp);
                }

            } while (false);
            return Json(resp);


        }

        public IActionResult CreditPrevodSelf(PrevedKreditModel m)
        {
            JsonAddMoney resp = new JsonAddMoney();

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

                if (m.KontCislo1 != m.KontCislo2)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nesouhlasí kontrolní čísla", Type = JsonMessageType.Danger });
                    break;
                }

                bool isOk = false;

                HashSet<int> pokladny = new HashSet<int>();

                UserZbytek Zbytek = objServ.GetZbytekUzivatele(m.UserId);

                // prevede kredity standardni
                if (Zbytek.Kredity.Count() > 0)
                {
                    // zbytky v pokladnach v minusu
                    foreach (var kr in Zbytek.Kredity.Where(d => d.Value < 0))
                    {
                        pokladny.Add(kr.Key);
                        var zbytky = Zbytek.KredityCas.Where(d => d.PokladnaId == kr.Key && d.PlatnostDo > DateTime.Now);
                        int zvKc = zbytky.Sum(d => d.Kredit);

                        int odepisujeSe = Math.Abs(kr.Value);

                        if (zvKc >= odepisujeSe)
                        {

                            UserTransakce tran = new UserTransakce();
                            tran.UserId = m.UserId;
                            tran.PokladnaId = kr.Key;
                            tran.ProvedlId = aa.User.Id.Value;
                            tran.Status = "}";
                            DateTime noe = DateTime.Now;

                            tran.UnixTime = DateTimeExtensions.ToUnix(DateTime.Now);
                            tran.Kredit = Math.Abs(kr.Value);
                            tran.LekceId = 0;
                            tran.Datum = noe;

                            bool zauctovano = objServ.RemoveKredit(tran, 1, true);

                            if (zauctovano)
                            {
                                UserTransakce tran1 = tran.Clone() as UserTransakce;
                                tran1.Kredit = odepisujeSe;
                                tran1.Zbytek = 0;
                                tran1.ZbyvaDni = 0;
                                tran1.Increment = 1;
                                tran1.Status = "{";
                                objServ.InsertTransactionStandard(tran1);

                            }

                            objServ.ClearZbytekUzivatele(m.UserId);
                            objServ.ClearHistoriTransakci(m.UserId);
                            objServ.ClearHistorieRoky(m.UserId);
                            objServ.ClearZbytekVObjednavkachUzivatele(m.UserId);

                            resp.Status = true;
                        }
                        else
                        {

                            resp.Messages.Add(new JsonMessage() { Text = "Neexistuje dostatek časových kreditů pro odpis", Type = JsonMessageType.Danger });
                            break;
                        }

                    }
                }


            } while (false);

            return Json(resp);

        }

        /// <summary>
        /// Vloze kredit bokem bez pokladny
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public IActionResult CreditPrevod(PrevedKreditModel m)
        {

            JsonAddMoney resp = new JsonAddMoney();

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

                if (!(m.NewUserId > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id uživatele pro převod", Type = JsonMessageType.Danger });
                    break;
                }

                if (m.KontCislo1 != m.KontCislo2)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nesouhlasí kontrolní čísla", Type = JsonMessageType.Danger });
                    break;
                }

                bool isOk = false;


                HashSet<int> pokladny = new HashSet<int>();

                UserZbytek Zbytek = objServ.GetZbytekUzivatele(m.UserId);

                // prevede kredity standardni
                if (Zbytek.Kredity.Count() > 0)
                {
                    foreach (var kr in Zbytek.Kredity.Where(d => d.Value > 0))
                    {

                        pokladny.Add(kr.Key);

                        var tran = new UserTransakce();
                        tran.UserId = m.UserId;
                        tran.PokladnaId = kr.Key;
                        tran.ProvedlId = aa.User.Id.Value;
                        tran.Datum = DateTime.Now;
                        tran.UnixTime = DateTimeExtensions.ToUnix(DateTime.Now);
                        tran.PlatnostOdUnix = null;
                        tran.PlatnostDoUnix = null;
                        tran.PlatnostOd = null;
                        tran.PlatnostDo = null;
                        tran.DoPokladny = 0;
                        tran.Status = ">";
                        tran.Typ = "k";
                        tran.Kredit = kr.Value;

                        objServ.RemoveKredit(tran);
                        resp.Messages.Add(new JsonMessage() { Text = $"Odebráno zákazníkovi {m.UserId} <b>{tran.Kredit}</b> kreditů", Type = JsonMessageType.Success });

                        tran = new UserTransakce();
                        tran.UserId = m.NewUserId;
                        tran.PokladnaId = kr.Key;
                        tran.ProvedlId = aa.User.Id.Value;
                        tran.Datum = DateTime.Now;
                        tran.UnixTime = DateTimeExtensions.ToUnix(DateTime.Now);
                        tran.PlatnostOdUnix = null;
                        tran.PlatnostDoUnix = null;
                        tran.PlatnostOd = null;
                        tran.PlatnostDo = null;
                        tran.DoPokladny = 0;
                        tran.Status = "<";
                        tran.Typ = "k";
                        tran.Kredit = kr.Value;

                        objServ.AddKredit(tran);
                        resp.Messages.Add(new JsonMessage() { Text = $"Připsáno zákazníkovi {m.NewUserId} <b>{tran.Kredit}</b> kreditů", Type = JsonMessageType.Success });
                    }
                }

                // prevede kredity casove
                if (Zbytek.KredityCas.Count() > 0)
                {
                    foreach (var kr in Zbytek.KredityCas.Where(d => d.Kredit > 0 && d.PlatnostDo > DateTime.Now))
                    {

                        pokladny.Add(kr.PokladnaId);

                        var tran = new UserTransakce();
                        tran.UserId = m.UserId;
                        tran.PokladnaId = kr.PokladnaId;
                        tran.ProvedlId = aa.User.Id.Value;
                        tran.Datum = DateTime.Now;
                        tran.UnixTime = DateTimeExtensions.ToUnix(DateTime.Now);
                        tran.PlatnostOdUnix = kr.PlatnostOdUnix;
                        tran.PlatnostDoUnix = kr.PlatnostDoUnix;
                        tran.PlatnostOd = kr.PlatnostOd;
                        tran.PlatnostDo = kr.PlatnostDo;
                        tran.DoPokladny = 0;
                        tran.Status = ">";
                        tran.Typ = "kc";
                        tran.Kredit = kr.Kredit;
                        tran.Timestamp = DateTime.Now;

                        objServ.ApplyTreansaction(tran);

                        resp.Messages.Add(new JsonMessage() { Text = $"zákazníkovi ({m.UserId}) - odebráno  <b>{tran.Kredit}</b> časových kreditů", Type = JsonMessageType.Success });

                        tran = new UserTransakce();
                        tran.UserId = m.NewUserId;
                        tran.PokladnaId = kr.PokladnaId;
                        tran.ProvedlId = aa.User.Id.Value;
                        tran.Datum = DateTime.Now;
                        tran.UnixTime = DateTimeExtensions.ToUnix(DateTime.Now);
                        tran.PlatnostOdUnix = kr.PlatnostOdUnix;
                        tran.PlatnostDoUnix = kr.PlatnostDoUnix;
                        tran.PlatnostOd = kr.PlatnostOd;
                        tran.PlatnostDo = kr.PlatnostDo;
                        tran.DoPokladny = 0;
                        tran.Status = "<";
                        tran.Typ = "kc";
                        tran.Kredit = kr.Kredit;
                        tran.Timestamp = DateTime.Now;

                        objServ.ApplyTreansaction(tran);

                        resp.Messages.Add(new JsonMessage() { Text = $"zákazníkovi ({m.NewUserId}) - připsáno <b>{tran.Kredit}</b> časových kreditů", Type = JsonMessageType.Success });

                        kr.UserId = m.NewUserId;

                        objServ.Update(kr);
                    }
                }

                isOk = true;

                if (isOk)
                {
                    resp.Status = true;
                    foreach (var pok in pokladny)
                    {
                        UpdateZbytek(m.UserId, pok, resp);
                        UpdateZbytek(m.NewUserId, pok, resp);
                    }
                }
            } while (false);

            return Json(resp);
        }


        public IActionResult ExtendValidity(ExtendValidityModel m)
        {

            JsonAddMoney resp = new JsonAddMoney();

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                if (!aa.User.Id.HasValue)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není přihášen uživatel", Type = JsonMessageType.Danger });
                    break;
                }

                if (!(m.Count > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není doba pro prodloužení", Type = JsonMessageType.Danger });
                    break;
                }

                if (!(m.Id > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není platba pro prodloužení", Type = JsonMessageType.Danger });
                    break;
                }

                UserZbytekKreditCas pla = objServ.GetUserCreditsTimeById(m.Id);

                if (pla == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nenacetla se platba pro prodloužení", Type = JsonMessageType.Danger });
                    break;
                }

                /// neomezeno
                if (m.Count == 999) {

                    int zbytek = pla.Kredit;
                    pla.Kredit = 0;
                    objServ.Update(pla);


                    UserTransakce tran = new UserTransakce();
                    tran.UserId = pla.UserId;
                    tran.PokladnaId = pla.PokladnaId;
                    tran.ProvedlId = aa.User.Id.Value;
                    tran.Status = "}";
                    DateTime noe = DateTime.Now;

                    tran.UnixTime = DateTimeExtensions.ToUnix(DateTime.Now);
                    tran.Kredit = Math.Abs(zbytek);
                    tran.LekceId = 0;
                    tran.Datum = noe;
                    tran.DoPokladny = 0;

                    objServ.InsertTransactionTime(tran);

                    UserTransakce tran1 = tran.Clone() as UserTransakce;
                    tran1.Kredit = zbytek;
                    tran1.Zbytek = 0;
                    tran1.ZbyvaDni = 0;
                    tran1.Increment = 1;
                    tran1.Status = "{";
                    tran1.DoPokladny = 0;

                    string text = "";


                    objServ.AddKredit(tran1);
                    text = $"Převedeno <b>{tran1.Kredit}</b> kreditů";

                    resp.Messages.Add(new JsonMessage() { Text = text, Type = JsonMessageType.Success });
                    UpdateZbytek(tran.UserId, tran.PokladnaId, resp);


                }
                // stop
                else if (m.Count == 7777)
                {
                    pla.PlatnostUnixBreak = DateTimeExtensions.ToUnix(DateTime.Now);
                    
                    objServ.Update(pla);

                    UserZbytekKreditCasLog log = new UserZbytekKreditCasLog();
                    log.PlatbaId = pla.Id;
                    log.UnixFrom = pla.PlatnostOdUnix;
                    log.UnixTo = 0;
                    log.Days = m.Count;
                    log.UserId = aa.User.Id.Value;
                    log.Ts = DateTime.Now;
                    objServ.Insert(log);

                    string text = $"Platnost do je zastavena";
                    resp.Messages.Add(new JsonMessage() { Text = text, Type = JsonMessageType.Success });
                    UpdateZbytek(pla.UserId, aa.Pobocka.PokladnaId, resp);

                    resp.Status = true;
                }
                // pustit stop
                else if (m.Count == 8888 && pla.PlatnostUnixBreak.HasValue)
                {
                    int prodlouzitO = DateTimeExtensions.ToUnix(DateTime.Now) -  pla.PlatnostUnixBreak.Value;

                    pla.PlatnostDoUnix = prodlouzitO + pla.PlatnostDoUnix;

                    pla.PlatnostUnixBreak = null;

                    objServ.Update(pla);

                    UserZbytekKreditCasLog log = new UserZbytekKreditCasLog();
                    log.PlatbaId = pla.Id;
                    log.UnixFrom = pla.PlatnostOdUnix;
                    log.UnixTo = pla.PlatnostDoUnix;
                    log.Days = m.Count;
                    log.UserId = aa.User.Id.Value;
                    log.Ts = DateTime.Now;
                    objServ.Insert(log);

                    string text = $"Platnost se znovu spuštěna";
                    resp.Messages.Add(new JsonMessage() { Text = text, Type = JsonMessageType.Success });
                    UpdateZbytek(pla.UserId, aa.Pobocka.PokladnaId, resp);

                    resp.Status = true;
                }
                else 
                {

                    int from = pla.PlatnostDoUnix;
                    int to = pla.PlatnostDoUnix + (m.Count * 24 * 60 * 60);
                    pla.PlatnostDoUnix = to;
                    pla.Prodlouzeno = true;
                    objServ.Update(pla);

                    UserZbytekKreditCasLog log = new UserZbytekKreditCasLog();
                    log.PlatbaId = pla.Id;
                    log.UnixFrom = from;
                    log.UnixTo = to;
                    log.Days = m.Count;
                    log.UserId = aa.User.Id.Value;
                    log.Ts = DateTime.Now;
                    objServ.Insert(log);

                    string text = $"Zbytek prodloužen o <b>{m.Count}</b> dní do <b>{DateTimeExtensions.FromUnix(to)}</b>";
                    resp.Messages.Add(new JsonMessage() { Text = text, Type = JsonMessageType.Success });
                    UpdateZbytek(pla.UserId, aa.Pobocka.PokladnaId, resp);

                    resp.Status = true;
                }

                objServ.ClearZbytekVObjednavkachUzivatele(pla.UserId);

            } while (false);

            return Json(resp);
        }

        [HttpPost]
        public IActionResult SetOk(AddRemoveLessonModel m)
        {
            JsonAddRemoveUserLesson resp = new JsonAddRemoveUserLesson();

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                if (!aa.User.Id.HasValue)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není přihášen uživatel" });
                    break;
                }

                if (!(m.Id > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id" });
                    break;
                }

                var obj = objServ.GetById(m.Id, false);

                if (obj == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nenačetla se data" });
                    break;
                }

                var lek = lekceServ.GetById(obj.LekceId);

                if (lek == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není lekce", Type = JsonMessageType.Danger });
                    break;
                }

                if (lek.Zauctovano)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Lekce je již zaúčtovaná", Type = JsonMessageType.Danger });
                    break;
                }

                obj.BylTam = !obj.BylTam;

                objServ.Update(obj);

                resp.Status = true;

            } while (false);

            return Json(resp);
        }

        public IActionResult SetPas(AddRemoveLessonModel m)
        {

            JsonAddRemoveUserLesson resp = new JsonAddRemoveUserLesson();

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                if (!aa.User.Id.HasValue)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není přihášen uživatel" });
                    break;
                }

                if (!(m.Id > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id" });
                    break;
                }

                var obj = objServ.GetById(m.Id, false);

                if (obj == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Nenačetla se data" });
                    break;
                }

                var lek = lekceServ.GetById(obj.LekceId);

                if (lek == null)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není lekce", Type = JsonMessageType.Danger });
                    break;
                }

                if (lek.Zauctovano)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Lekce je již zaúčtovaná", Type = JsonMessageType.Danger });
                    break;
                }

                obj.Aktivni = !obj.Aktivni;

                objServ.Update(obj);

                resp.Status = true;

            } while (false);

            return Json(resp);
        }
        [HttpPost]

        public int SendSms(SmsContent sms)
        {

            string num = sms.Number.ToString().Substring(7, 2);
            var kod = getSmsCode(num);
            sms.Kod = kod;
            string url = $"https://{"sms.diva2.cz/index.php"}?c={kod}&u={sms.Name}&h={sms.Pass}&m={sms.Number}&t={sms.Text}";


            WebRequest request = WebRequest.CreateHttp(url);
            var response = request.GetResponse();

            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

            var result = readStream.ReadToEnd();
            sms.Result = result.ToString();

            var xml = System.Xml.Linq.XElement.Parse(result);
            var res = xml.Element("err").Value;
            double crDouble = -1;
            try{

                var cr = xml.Element("credit").Value;
                double.TryParse(cr.Replace(".", ","), out crDouble);
            }
            catch{ 
            }

            sms.Credit = crDouble;

            int.TryParse(res, out int id);

            return id;

        }

        private string getSmsCode(string num)
        {
            Random rnd = new Random();
            var ko = rnd.Next(1, 9);
            var za = rnd.Next(100, 99999);
            return $"{za}{num}{ko}";
        }

        [HttpPost]
        public IActionResult SendSmsByLesson(int id, string text)
        {

            JsonStatus resp = new JsonStatus();
            do
            {
                TrySetUserFromSess(aa.User);
                SetSessions();

                if (!aa.User.Id.HasValue)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není přihášen uživatel", Type = JsonMessageType.Danger });
                    break;
                }

                var objs = objServ.GetByLekce(id, true);

                if (objs != null && objs.Count() > 0)
                {

                    var sms = new SmsContent()
                    {
                        Name = aa.MainIniCover.MainIniObj.SmsName,
                        Pass = aa.MainIniCover.MainIniObj.SmsPass,
                        Text = text
                    };
                    sms.Numbers = new Dictionary<int, int>();

                    var user = objs.FirstOrDefault().User;

                    int.TryParse(user.PhoneNumber.Replace(" ", ""), out int ph);
                    if (ph > 100000000)
                    {
                        sms.Number = ph;
                        sms.Numbers.Add(user.Id, ph);
                    }

                    foreach (var obj in objs)
                    {

                        int.TryParse(obj.User.PhoneNumber.Replace(" ", ""), out int ph1);
                        if (ph1 > 100000000)
                        {
                            if (!sms.Numbers.ContainsKey(obj.User.Id))
                            {
                                sms.Numbers.Add(obj.User.Id, ph1);
                            }
                        }
                    }

                    if (sms.Numbers.Count == 0)
                    {
                        resp.MsgAddDanger("Nejsou platná čísla u objednaných");
                        break;
                    }

                    SendSmsMore(sms);

                }
                else
                {
                    resp.MsgAddDanger("Nejsou objednaní");
                }
            } while (false);
            return Json(resp);

        }

        public async Task SendSmsMore(SmsContent sms)
        {

            var client = new HttpClient();

            string num = sms.Number.ToString().Substring(7, 2);
            var kod = getSmsCode(num);

            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string, string>("c", kod));
            values.Add(new KeyValuePair<string, string>("u", sms.Name));
            values.Add(new KeyValuePair<string, string>("h", sms.Pass));
            values.Add(new KeyValuePair<string, string>("m", sms.Number.ToString()));
            values.Add(new KeyValuePair<string, string>("t", sms.Text));

            foreach (var n in sms.Numbers)
            {
                values.Add(new KeyValuePair<string, string>($"ms[{n.Key}]", n.Value.ToString()));
            }

            var content = new FormUrlEncodedContent(values);

            var url = $"https://{"sms.diva2.cz/index2.php"}";

            var response = await client.PostAsync(url, content);

            var responseString = await response.Content.ReadAsStringAsync();

        }

        public IActionResult SavePayTry(IdTypeModel m)
        {

            JsonStatus resp = new JsonStatus();

            TrySetUserFromSess(aa.User);
            SetSessions();

            do
            {
                if (!(aa.MainIniCover.MainGatePaysObj.Pays_Active))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není aktivní platba" });
                    break;
                }

                if (!aa.User.Id.HasValue)
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není přihlášen uživatel" });
                    break;
                }

                if (!(m.Id > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není id" });
                    break;
                }

                if (!(m.Type > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není typ" });
                    break;
                }

                PlatbaBase plb = new PlatbaBase();
                if (m.Type == 1 || m.Type == 2)
                {
                    plb = platbaServ.GetKreditById(m.Id);
                    if (plb == null)
                    {
                        resp.Messages.Add(new JsonMessage() { Text = "Není platba" });
                        break;
                    }
                }
                else if (m.Type == 3 || m.Type == 4)
                {
                    plb = platbaServ.GetKreditCasById(m.Id);
                    if (plb == null)
                    {
                        resp.Messages.Add(new JsonMessage() { Text = "Není platba" });
                        break;
                    }
                }

                PaysItem it = new PaysItem()
                {
                    Merchant = aa.MainIniCover.MainGatePaysObj.Pays_Merchant,
                    Shop = aa.MainIniCover.MainGatePaysObj.Pays_Shop,
                    Price = plb.Castka,
                    UserId = aa.User.Id.Value,
                    PokladnaId = aa.Pobocka.PokladnaId,
                    Email = aa.User.Email,
                    ItemId = plb.Id,
                    TypeId = m.Type,
                    CreatedDt = DateTime.Now,
                    Currency = "CZK",
                    Lang = "CS-CZ",
                    YearMonth = int.Parse(DateTime.Now.ToString("yyyyMM")),
                    AddedCredit = false
                };

                platbaServ.Insert(it);

                string url = $"https://www.pays.cz/paymentorder?Merchant={it.Merchant}&Shop={it.Shop}&Currency=CZK&Amount={it.Price * 100}&MerchantOrderNumber={it.Id}&Email={it.Email}&Lang=CS-CZ";

                resp.Meta = url;

                resp.Status = true;

            } while (false);

            return Json(resp);
        }


        public IActionResult AddEvent(AddEvent m)
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

                if (!(m.M > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není hodina", Type = JsonMessageType.Danger });
                    break;
                }

                if (!(m.Dr > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není dráha", Type = JsonMessageType.Danger });
                    break;
                }

                if (!(m.D > 0))
                {
                    resp.Messages.Add(new JsonMessage() { Text = "Není den", Type = JsonMessageType.Danger });
                    break;
                }




            } while (false);
            return Json(resp);


        }

    }
}