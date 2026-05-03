using cloudscribe.Pagination.Models;
using Diva2.Core;
using Diva2.Core.Main;
using Diva2.Core.Main.Calendar;
using Diva2.Core.Main.Content;
using Diva2.Core.Main.Lektori;
using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Main;
using Diva2.Core.Main.Platby;
using Diva2.Core.Main.Pobocky;
using Diva2.Core.Main.Trans;
using Diva2.Core.Main.Users;
using Diva2.Core.Main.Videa;
using Diva2.Core.Model.Json;
using Diva2.Core.Models;
using Diva2Web.Models.Content;
using Diva2Web.Models.Lekces;
using Diva2Web.Models.Platby;
using Diva2Web.Models.Responses;
using Diva2Web.Models.Users;
using Diva2Web.Models.Videos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Admin
{
    public class AdminPageModel
    {
        public string ActionName { get; internal set; }

        public string ControllerName { get; internal set; }

        public MainIniCover MainIniCover { get; set; }

        public int? Id { get; set; }

        public UserModel User { get; set; } = new UserModel();

        public int UserId { get { return User.Id.HasValue ? User.Id.Value : 0; } }

        public string SubDomain { get; set; }

        public Pobocka Pobocka { get; set; }

        public string Title { get; set; } = "Rezervační systém";

        public IEnumerable<Pobocka> Pobocky { get; set; }

        public IEnumerable<CalIniMinute> IniMinutes { get; set; }

        //  public IList<CasZacatek> Zacatky { get; set; }

        public List<MenuItem> AdminMenuItems { get; set; } = new List<MenuItem>();

        public List<MenuItem> PublicMenuItems { get; set; } = new List<MenuItem>();

        public IList<Lektor> Lektori { get; set; }

        public IList<User8Group> SkupinyZakaznika { get; set; }

        public IList<LekceTyp> TypyLekci { get; internal set; }

        public IList<LekceMustrTyp> LekceMustrTypy { get; set; }

        public IList<LekceMustrModel> LekceMustr { get; set; }

        public Dictionary<int, string> Days { get; set; } = new Dictionary<int, string>();

        public IList<PlatbaKredit> PlatbyKredit { get; set; }

        public IList<PlatbaKreditCas> PlatbyKreditCas { get; set; }

        public IList<PlatbaCas> PlatbyCas { get; set; }

        public bool ShowCalendar { get; set; } = false;
        public bool ShowSwitchPob { get; set; } = true;

        public DateTime CurrentDate { get; set; } = new DateTime();

        public DateTime Monday { get; set; } = new DateTime();

        public IEnumerable<Lekce> LekceTyden { get; set; }

        public Dictionary<string, string> InisPob { get; set; }

        public Dictionary<int, LekceTyden> Rozvrhy { get; internal set; }

        public Rozvrhy RozvrhyObj { get; internal set; }

        public Rozvrhy2 Rozvrhy2Obj { get; internal set; }

        public Rozvrhy3 Rozvrhy3Obj { get; internal set; }

        public StyleModel Style { get; set; }
        public IList<User8> Users { get; internal set; } = new List<User8>();

        public LekceBoardModel LekceOneBoard { get; set; }

        public JsonStatus JsonStatus { get; set; }

        public IList<UserText> Messages { get; internal set; }

        public IEnumerable<Page> Pages { get; set; }

        public PageModel Page { get; set; }

        public IEnumerable<VideoModel> Videos { get; set; }

        public IEnumerable<CalEvent> Events { get; set; }

        public VideoModel Video { get; set; }

        public List<DateTime> _weaks;
        public List<DateTime> Weaks
        {
            get
            {
                if (_weaks == null)
                {
                    _weaks = new List<DateTime>();
                    _weaks.Add(DateTime.Parse("2025-09-01"));
                    _weaks.Add(DateTime.Parse("2025-01-27"));
                    _weaks.Add(DateTime.Parse("2024-09-02"));
                    _weaks.Add(DateTime.Parse("2024-01-29"));
                    _weaks.Add(DateTime.Parse("2023-09-04"));
                    _weaks.Add(DateTime.Parse("2023-01-30"));
                    _weaks.Add(DateTime.Parse("2022-08-29"));
                }
                return _weaks;
            }

        }

        public AdminPageModel()
        {


        }


        public void AplyRulesToAdminMenu()
        {
            foreach (var L1 in AdminMenuItems)
            {
                L1.Visible = true;

                if (L1.SettingMainIni != MainIniRuleItem.Empty)
                {
                    L1.Visible = MainIniCover.HasRule(L1.SettingMainIni);
                    if (L1.Visible == false)
                    {
                        continue;
                    }
                }

                if (L1.SettingPobIni != null && L1.SettingPobIni.Length > 0)
                {
                    L1.Visible = GetIniPobBool(L1.SettingPobIni);
                    if (L1.Visible == false)
                    {
                        continue;
                    }
                }

                if (L1.Pravo != null && L1.Pravo.Length > 0)
                {
                    L1.Visible = User.HasRule(L1.Pravo);
                }



                if (L1.Visible)
                {
                    if (L1.Items != null)
                    {
                        foreach (var L2 in L1.Items)
                        {
                            L2.Visible = true;
                            if (L2.SettingMainIni != MainIniRuleItem.Empty)
                            {
                                L2.Visible = MainIniCover.HasRule(L2.SettingMainIni);
                                if (L2.Visible == false)
                                {
                                    continue;
                                }
                            }

                            if (L2.SettingPobIni != null && L2.SettingPobIni.Length > 0)
                            {
                                L2.Visible = GetIniPobBool(L2.SettingPobIni);
                            }

                            if (L2.Pravo != null && L2.Pravo.Length > 0)
                            {
                                L2.Visible = User.HasRule(L2.Pravo);
                            }


                        }
                    }
                }
            }
        }

        public void PopulateAdminMenu()
        {


            MenuItem mnHome = new MenuItem() { Area = "Board", Text = "Rozvrh", Url = "Board", Pravo = "spin_view" };
            AdminMenuItems.Add(mnHome);

            MenuItem mnCust = new MenuItem() { Area = "Customer", Text = "Zákazníci", Url = "Customer", Pravo = "spin_view" };
            AdminMenuItems.Add(mnCust);
            mnCust.Items = new List<MenuItem>();
            mnCust.Items.Add(new MenuItem() { Text = "Zákazníci", Url = "Index", Pravo = "spin_view" });
            mnCust.Items.Add(new MenuItem() { Text = "Smazaní", Url = "Deleted", Pravo = "spin_view" });
            mnCust.Items.Add(new MenuItem() { Text = "Prodloužit platnost", Url = "Extend", Pravo = "spin_view" });


            MenuItem mnSetting = new MenuItem() { Area = "Setting", Text = "Nastavení", Url = "Setting", Pravo = "spinstat_view" };
            AdminMenuItems.Add(mnSetting);
            mnSetting.Items = new List<MenuItem>();
            mnSetting.Items.Add(new MenuItem() { Text = "Hlavní šablona", Url = "Mustr", Pravo = "spinstat_view" });
            mnSetting.Items.Add(new MenuItem() { Text = "Týdenní kopie", Url = "WeakCopy", Pravo = "spinstat_view" });
            mnSetting.Items.Add(new MenuItem() { Text = "Šablony lekcí", Url = "MusterTypes", Pravo = "spinstat_view" });
            mnSetting.Items.Add(new MenuItem() { Text = "Tržby", Url = "Sales", Pravo = "spinstat_view" });
            mnSetting.Items.Add(new MenuItem() { Text = "Mzdy", Url = "Wages", Pravo = "spinstat_view" });
            mnSetting.Items.Add(new MenuItem() { Text = "Typy lekcí", Url = "LessonsType", Pravo = "spinstat_view" });
            mnSetting.Items.Add(new MenuItem() { Text = "Lektoři", Url = "Lectors", Pravo = "lektor_view" });
            mnSetting.Items.Add(new MenuItem() { Text = "Platby", Url = "Payments", Pravo = "spinstat_view" });
            mnSetting.Items.Add(new MenuItem() { Text = "Kategorie zákazníka", Url = "CustomersGroup", Pravo = "spinstat_view" });
            mnSetting.Items.Add(new MenuItem() { Text = "Nastavení pobočky", Url = "SettingPob", Pravo = "spinstat_view" });
            mnSetting.Items.Add(new MenuItem() { Text = "Nastavení systému", Url = "SettingMain", Pravo = "spinstat_view" });
            mnSetting.Items.Add(new MenuItem() { Text = "Nastavení platebních bran", Url = "SettingMainPayGates", SettingPobIni = "gate_pay" });

            if (MainIniCover.BankAccountUse)
            {
                mnSetting.Items.Add(new MenuItem() { Text = "Nastavení bankovního účtu", Url = "SettingMainBankAccount" });
            }

            mnSetting.Items.Add(new MenuItem() { Text = "Neuzavřené hodiny", Url = "NoClosedLessons", Pravo = "spin_close" });

            MenuItem mnSms = new MenuItem() { Area = "Sms", Text = "Sms", Url = "Sms", Pravo = IniItemsSetting.sms_view.ToString() };
            AdminMenuItems.Add(mnSms);
            mnSms.Items = new List<MenuItem>();
            mnSms.Items.Add(new MenuItem() { Text = "Odhlášení", Url = "Logout", Pravo = IniItemsSetting.sms_view.ToString() });
            mnSms.Items.Add(new MenuItem() { Text = "Přihlášení", Url = "Login", Pravo = IniItemsSetting.sms_view.ToString() });
            mnSms.Items.Add(new MenuItem() { Text = "SMS", Url = "Index", Pravo = IniItemsSetting.sms_view.ToString() });
            mnSms.Items.Add(new MenuItem() { Text = "SMS - Chybové", Url = "Errors", Pravo = IniItemsSetting.sms_view.ToString() });

            MenuItem mnContent = new MenuItem() { Area = "Content", Text = "Obsah", Url = "Content", Pravo = "content_manage" };
            AdminMenuItems.Add(mnContent);
            mnContent.Items = new List<MenuItem>();
            mnContent.Items.Add(new MenuItem() { Text = "Stránky", Url = "Pages", Pravo = "role_view" });


            MenuItem mnVideo = new MenuItem() { Area = "Video", Text = "Videa", Url = "Video", Pravo = "video_manage", SettingMainIni = MainIniRuleItem.UseVideo };
            AdminMenuItems.Add(mnVideo);
            mnVideo.Items = new List<MenuItem>();
            mnVideo.Items.Add(new MenuItem() { Text = "Seznam", Url = "Index", Pravo = "video_manage" });
            mnVideo.Items.Add(new MenuItem() { Text = "Prodaná", Url = "Sold", Pravo = "video_manage" });

            MenuItem mnPrava = new MenuItem() { Area = "Rules", Text = "Práva", Url = "Rules", Pravo = "prava_view" };
            AdminMenuItems.Add(mnPrava);

            mnPrava.Items = new List<MenuItem>();
            mnPrava.Items.Add(new MenuItem() { Text = "Role", Url = "Roles", Pravo = "role_view" });
            mnPrava.Items.Add(new MenuItem() { Text = "Účty", Url = "Accounts", Pravo = "role_view" });
            mnPrava.Items.Add(new MenuItem() { Text = "Admin", Url = "AdminThings", Pravo = "all_able" });

            MenuItem mnHelp = new MenuItem() { Area = "Help", Text = "Help", Url = "Help" };
            mnHelp.Items = new List<MenuItem>
            {
                new MenuItem() { Text = "Index", Url = "Index" },
                new MenuItem() { Text = "Rozvrh", Url = "Rozvrh" },
                new MenuItem() { Text = "Platby", Url = "Platby" },
                new MenuItem() { Text = "Vlastní stránky", Url = "Pages" },
                new MenuItem() { Text = "Zákazník", Url = "Zakaznik" },
                new MenuItem() { Text = "Videa", Url = "Video" },
                new MenuItem() { Text = "Generování rozvrhu", Url = "GenerovaniRozvrhu" }
            };

            /*
            mnHelp.Items.Add(new MenuItem() { Text = "", Url = "" });
            /**/
            AdminMenuItems.Add(mnHelp);
        }

        public void PopulatePublicMenu(IUrlHelper urlHeper)
        {
            MenuItem mnBoard = new MenuItem() { Area = "Home", Text = "Rozvrh", Url = urlHeper.RouteUrl("rozvrh") };
            PublicMenuItems.Add(mnBoard);

            mnBoard.Items = new List<MenuItem>();
            mnBoard.Items.Add(new MenuItem() { Text = "Ceník", Url = urlHeper.RouteUrl("cenik"), Method = "Prices", Order = 10 });
            if (MainIniCover.MainStyleObj.ShowLectorPage)
            {
                mnBoard.Items.Add(new MenuItem() { Text = "Lektoři", Url = urlHeper.RouteUrl("lektori"), Method = "Lectors", Order = 20 });
            }
            mnBoard.Items.Add(new MenuItem() { Text = "Pomoc", Url = urlHeper.RouteUrl("help"), Method = "Help", Order = 30 });
            mnBoard.Items.Add(new MenuItem() { Text = "Gdpr", Url = urlHeper.RouteUrl("gdpr"), Method = "Gdpr", Order = 40 });

            if (MainIniCover.UseVideo)
            {
                mnBoard.Items.Add(new MenuItem() { Text = "Moje videa", Url = urlHeper.RouteUrl("mojevidea"), Method = "MyVideos", Order = 60, Area = "Video", Controller = "Video" });
            }

        }

        public string GetIniPob(string key)
        {
            string ret = "";
            InisPob.TryGetValue(key, out ret);

            return ret;
        }

        public int GetIniPobInt(string key)
        {
            string ret = "";
            InisPob.TryGetValue(key, out ret);

            int.TryParse(ret, out int retInt);

            return retInt;
        }

        public bool GetIniPobBool(string key)
        {
            string ret = "";
            InisPob.TryGetValue(key, out ret);

            int.TryParse(ret, out int retInt);

            return retInt > 0;
        }

        public void PopulateDays()
        {


            Days.Add(1, "Pondělí");
            Days.Add(2, "Úterý");
            Days.Add(3, "Středa");
            Days.Add(4, "Čtvrtek");
            Days.Add(5, "Pátek");
            Days.Add(6, "Sobota");
            Days.Add(7, "Neděle");
        }

        #region Paged

        public long PageNumber { get; set; }

        public int PageSize { get; set; }

        public long TotalItemCount { get; set; }

        internal void SetFromPaged<T>(PagedResult<T> paged) where T : class
        {
            PageSize = paged.PageSize;
            PageNumber = paged.PageNumber;
            TotalItemCount = paged.TotalItems;
        }

        internal void SetFromPaged<T>(IPagedList<T> paged) where T : class
        {
            PageSize = paged.PageSize;
            PageNumber = paged.PageNumber;
            TotalItemCount = paged.TotalItems;
        }
        #endregion


      
        public double DPH { get; internal set; }

       }
}
