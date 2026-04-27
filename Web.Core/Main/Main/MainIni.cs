using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Diva2.Core.Main.Main
{

    public class MainIniCover : BaseEntity
    {
        public MainIniCover()
        {
            if (StyleFiles.Count == 0)
            {
                StyleFiles.Add("default", "Hlavní");
                StyleFiles.Add("celurean", "Celurean");
                StyleFiles.Add("journal", "Journal");
                StyleFiles.Add("yeti", "Yeti");
                StyleFiles.Add("united", "United");
                StyleFiles.Add("materia", "Materia");
                StyleFiles.Add("cosmo", "Cosmo");
                StyleFiles.Add("flatly", "Flatly");
                StyleFiles.Add("lumen", "Lumen");
                StyleFiles.Add("superHero", "SuperHero");
                StyleFiles.Add("minty", "minty");
                StyleFiles.Add("spacelab", "Spacelab");



                NavbarThemeFore.Add("navbar-dark", "navbar-dark");
                NavbarThemeFore.Add("navbar-light", "navbar-light");

                NavbarThemeBg.Add("bg-primary", "bg-primary");
                NavbarThemeBg.Add("bg-dark", "bg-dark");
                NavbarThemeBg.Add("bg-light", "bg-light");
            }
        }

        #region MainIni
        public string Data
        {
            get
            {
                return JsonConvert.SerializeObject(MainIniObj);
            }
            set
            {
                MainIniObj = JsonConvert.DeserializeObject<MainIni>(value);
            }
        }
        public MainIni MainIniObj { get; set; }
        public void SetDefault()
        {
            MainIniObj = new MainIni();
            MainIniObj.MasterUrl = "www.seznam.cz";
            MainIniObj.MasterUrlText = "Moje sport centrum";
            MainIniObj.MasterName = "";
            MainIniObj.MasterStreet = "";
            MainIniObj.MasterZip = "";
            MainIniObj.MasterPost = "";

            MainIniObj.RegisterEmail = true;
        }
        #endregion

        #region Styly
        public string Styly
        {
            get
            {
                return JsonConvert.SerializeObject(MainStyleObj);
            }
            set
            {
                MainStyleObj = JsonConvert.DeserializeObject<MainIniStyle>(value);
            }
        }
        public MainIniStyle MainStyleObj { get; set; }
        public void SetDefaultStyles()
        {

            MainStyleObj = new MainIniStyle();
            MainStyleObj.File = "default";
            MainStyleObj.NavbarThemeBg = "bg-primary";
            MainStyleObj.NavbarThemeFore = "navbar-dark";
        }
        #endregion

        #region Gates

        public string GatePays
        {
            get
            {
                return JsonConvert.SerializeObject(MainGatePaysObj);
            }
            set
            {
                MainGatePaysObj = JsonConvert.DeserializeObject<MainIniPayGates>(value);
            }
        }
        public MainIniPayGates MainGatePaysObj { get; set; }
        public void SetDefaultPayGates()
        {
            MainGatePaysObj = new MainIniPayGates();
        }
        #endregion

        #region Bank

        public bool BankAccountUse { get; set; } = false;

        public string BankAccount
        {
            get
            {
                return JsonConvert.SerializeObject(BankAccountObj);
            }
            set
            {
                BankAccountObj = JsonConvert.DeserializeObject<MainIniBankAccount>(value);
            }
        }
        public MainIniBankAccount BankAccountObj { get; set; }
        public void SetDefaultBankAccount()
        {
            BankAccountObj = new MainIniBankAccount();
        }
        #endregion



        public bool UseVideo { get; set; }

        public static Dictionary<string, string> StyleFiles = new Dictionary<string, string>();
        public static Dictionary<string, string> NavbarThemeFore = new Dictionary<string, string>();
        public static Dictionary<string, string> NavbarThemeBg = new Dictionary<string, string>();


        public bool HasRule(MainIniRuleItem key)
        {

            bool ret = false;

            if (key == MainIniRuleItem.UseVideo)
            {
                ret = UseVideo == true;
            }

            return ret;
        }

    }

    public enum MainIniRuleItem { Empty = 0, UseVideo }

    public class MainIni
    {

        [Display(Name = "Url firmy")]
        public string MasterUrl { get; set; }

        [Display(Name = "Text firmy pro url")]
        public string MasterUrlText { get; set; }


        [Display(Name = "Jméno majitele")]
        public string MasterJmenoMajitele { get; set; }

        [Display(Name = "Název")]
        public string MasterName { get; set; }

        [Display(Name = "Ulice")]
        public string MasterStreet { get; set; }

        [Display(Name = "Psč")]
        public string MasterZip { get; set; }

        [Display(Name = "Pošta")]
        public string MasterPost { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "IČO")]
        public string ICO { get; set; }


        [Display(Name = "DIČ")]
        public string DIC { get; set; }

        [Display(Name = "Po registraci vyžadovat potvrzení emailem")]
        public bool RegisterEmail { get; set; }

        [Display(Name = "Odesílat sms přes bránu")]
        public bool SmsActive { get; set; }

        [Display(Name = "Jméno pro sms bránu")]
        public string SmsName { get; set; }

        [Display(Name = "Heslo pro sms bránu")]
        public string SmsPass { get; set; }

        [Display(Name = "Text na příjmový doklad")]

        public string TextNaPrijmovyDoklad { get; set; }

        public bool ShowRestPlacesOnBoard { get; set; } = false;

    }

    public class MainIniStyle
    {

        [Display(Name = "Hlavní soubor")]
        public string File { get; set; }

        [Display(Name = "Styl navbaru")]
        public string NavbarThemeFore { get; set; }

        [Display(Name = "Style pozadí navbaru")]
        public string NavbarThemeBg { get; set; }

        [Display(Name = "Pozadí navbaru (přepíše barvy)")]
        public string NavbarBackgroundColor { get; set; } = "";

        [Display(Name = "Rámeček navbaru (barva)")]
        public string NavbarBorderColor { get; set; } = "";

        [Display(Name = "Posunututí od vrchu o X px")]
        public string NavbarBotomTop { get; set; } = "";

        [Display(Name = "Pozadí obsahu (za rozvrhem)")]
        public string BackgroundContainer { get; set; } = "";

        [Display(Name = "Pozadí stránky")]
        public string BackgroundPage { get; set; } = "";


        [Display(Name = "Zobraz lektory")]
        public bool ShowLectorPage { get; set; } = true;
    }


    public class MainIniPayGates
    {
        [Display(Name = "Aktivní")]
        public bool Pays_Active { get; set; }

        [Display(Name = "Merchant")]
        public int Pays_Merchant { get; set; }

        [Display(Name = "Shop")]
        public int Pays_Shop { get; set; }

        [Display(Name = "Heslo")]
        public string Pays_Pass { get; set; }
    }

    public class MainIniBankAccount
    {
        [Display(Name = "Aktivní")]
        public bool Active { get; set; } = false;

        [Display(Name = "Předčíslí účtu")]
        public string AccountPrefix { get; set; }

        [Display(Name = "Číslo účtu")]
        public string AccountNumber { get; set; }

        [Display(Name = "Kód banky")]
        public string BankCode { get; set; }

    }


}
