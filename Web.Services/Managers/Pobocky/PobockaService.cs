using Diva2.Core;
using Diva2.Core.Main;
using Diva2.Core.Main.Calendar;
using Diva2.Core.Main.Main;
using Diva2.Core.Main.Pobocky;
using Diva2.Core.Main.Users;
using Diva2.Data;
using Diva2.Services.Managers.Platby;
using Diva2.Services.Managers.Setting;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Diva2.Core.Branch;

namespace Diva2.Services.Managers.Pobocky
{
    public class PobockaService : IPobockaService
    {
        private CacheHelper cache;
        private string tenant;
        private IRepository<CasZacatek> casyRep;
        private IRepository<PobockaIni> pobockaIniRep;
        private IRepository<Pobocka> repository;
        private IRepository<MainIniCover> repMainIni;
        private IRepository<CalIniMinute> repIniMin;

        public PobockaService(ApplicationDbContext dbContext, IMemoryCache memoryCache,
                        IRepository<CasZacatek> repZac, IRepository<Pobocka> rep, IRepository<PobockaIni> repPobockaIni,
                        IRepository<MainIniCover> mainIni, IRepository<CalIniMinute> reIniMin, IRepository<ILektorService> lecRep,
                        IRepository<IPlatbaService> platbyRep)
        {

            this.cache = new CacheHelper(memoryCache, dbContext.SubDomain);

            this.tenant = dbContext.SubDomain;

            this.casyRep = repZac;
            this.repository = rep;
            this.pobockaIniRep = repPobockaIni;
            this.repMainIni = mainIni;
            this.repIniMin = reIniMin;
        }

        public void ClearPobocky()
        {
            string cacheItemKey = $"GetPobocky";
            cache.ClearDataSub(cacheItemKey);
        }
        public IList<Pobocka> GetPobocky()
        {
            string cacheItemKey = $"GetPobocky";
            var list = cache.GetDataSub<IList<Pobocka>>(cacheItemKey);

            if (list == null)
            {
                list = repository.Table.OrderBy(d => d.Order).ToList();
                cache.SetDataSub(cacheItemKey, list);
            }

            return list;
        }

        #region Pobocka


        public void Insert(List<CasZacatek> insertCasy)
        {
            foreach (var ca in insertCasy)
            {
                casyRep.Insert(ca);
            }
            ClearZacatky();
        }

        public void ClearZacatky()
        {
            string cacheItemKey = $"GetCasyky";
            cache.ClearDataSub(cacheItemKey);
        }
        public IList<CasZacatek> GetZacatky()
        {
            string cacheItemKey = $"GetCasyky";
            var list = cache.GetDataSub<IList<CasZacatek>>(cacheItemKey);
            if (list == null)
            {
                list = casyRep.Table.ToList();
                cache.SetDataSub(cacheItemKey, list);
            }

            return list.ToList();
        }

        #region Ini

        public Company GetCompany()
        {
            MainIniCover maniIni = GetMainIni();

            var comp = new Company(maniIni.MainIniObj);
            comp.Tenant = tenant;

            comp.Branches = new List<Branch>();

            var pobs = GetPobocky();

            foreach (Pobocka pob in pobs)
            {
                Branch br = new Branch(pob);
                br.Inis = new List<BranchIni>();
                foreach (var ini in GetPobockaInis(pob.Id))
                {
                    br.Inis.Add(new BranchIni() { Name = ini.Name, Value = ini.Value });
                }
                comp.Branches.Add(br);
            }




            return comp;
        }

        public PobockaIni GetIni(int pobId, string key)
        {
            var ini = pobockaIniRep.Table.Where(d => d.PobockaId == pobId && d.Name == key).FirstOrDefault();
            return ini;
        }

        public void Update(PobockaIni ini)
        {
            pobockaIniRep.Update(ini);

        }

        public void ClearPobockaInis(int pobId)
        {
            string cacheItemKey = $"GetPobockaIni-{pobId}-";
            cache.ClearDataSub(cacheItemKey);
        }

        public IList<PobockaIni> GetPobockaInis(int pobId)
        {

            string cacheItemKey = $"GetPobockaIni-{pobId}-";
            IList<PobockaIni> list = cache.GetDataSub<IList<PobockaIni>>(cacheItemKey);

            if (list == null)
            {
                list = new List<PobockaIni>();
                List<PobockaIni> defaults = GetDefaults();
                List<string> keys = defaults.Select(d => d.Name).ToList();

                var listDb = pobockaIniRep.TableUntracked.Where(d => d.PobockaId == pobId).ToList();
                List<PobockaIni> insert = new List<PobockaIni>();
                List<string> keysDb = listDb.Select(d => d.Name).ToList();

                foreach (var ini in listDb)
                {
                    list.Add(ini);

                    if (keys.Contains(ini.Name))
                    {
                        keys.Remove(ini.Name);
                        keysDb.Remove(ini.Name);
                    }
                }

                if (keys.Count() > 0)
                {
                    foreach (var key in keys)
                    {
                        var Ini = defaults.Where(d => d.Name == key).FirstOrDefault();
                        if (Ini != null)
                        {
                            Ini.Value = Ini.Default;
                            Ini.PobockaId = pobId;
                            insert.Add(Ini);
                            listDb.Add(Ini);
                        }
                    }
                }

                if (keysDb.Count() > 0)
                {

                }

                if (insert.Count > 0)
                {
                    foreach (var ini in insert)
                    {
                        pobockaIniRep.Insert(ini);
                        list.Add(ini);
                    }
                }
                cache.SetDataSub<IList<PobockaIni>>(cacheItemKey, list);
            }


            return list;

        }


        public List<PobockaIni> GetDefaults()
        {
            // $pole["nactena"] = 1;    // nactena znamena jestli jsou aktualni data jinak
            // pokud se menili dam nactena 0 a nactou se znova

            List<PobockaIni> list = new List<PobockaIni>();

            list.Add(new PobockaIni() { Name = "kreditNeomezeny", Default = "1", Type = PobockaIniType.Bolean, Desc = "Zda se ma v systému účtovat standardní časově neomezený kredit." });
            list.Add(new PobockaIni() { Name = "kreditCasovy", Default = "0", Type = PobockaIniType.Bolean, Desc = "Zda se ma v systému účtovat časový kredit." });
            list.Add(new PobockaIni() { Name = "pausalCasovy", Default = "0", Type = PobockaIniType.Bolean, Desc = "Zda se má v systému účtovat časové předplatné." });
            list.Add(new PobockaIni() { Name = "pobPlov", Default = "0", Type = PobockaIniType.Bolean, Desc = "Je pobočka generovaná s hodinami v časových intervalech." });
            list.Add(new PobockaIni() { Name = "pobVer", Default = "0", Type = PobockaIniType.Int, Desc = "Je pobočka  0-Normal,1-Plovoucí,2-Fly" });
            list.Add(new PobockaIni() { Name = "pokladna", Default = "1", Type = PobockaIniType.Bolean, Desc = "Do ktere pokladny se bude účtovat kredit." });
            list.Add(new PobockaIni() { Name = "zdrojPocet", Default = "1", Type = PobockaIniType.Int, Desc = "Počet zdrojů, drah, kurtů nebo místností kde se dá pobočka/rozvrh provozovat." });
            list.Add(new PobockaIni() { Name = "zdrojNazev", Default = "Dráha", Type = PobockaIniType.String, Desc = "Název dráhy,kurtu nebo místnosti zdroje." });
            list.Add(new PobockaIni() { Name = "plovZacatek", Default = "0", Type = PobockaIniType.Time, Desc = "Hodina od které se mají hodiny/lekce generovat." });
            list.Add(new PobockaIni() { Name = "plovDelkaM", Default = "20", Type = PobockaIniType.Int, Desc = "Počet minut kolik trvá jedna časově generovaná lekce." });
            list.Add(new PobockaIni() { Name = "pocetHodin", Default = "5", Type = PobockaIniType.Int, Desc = "Počet hodin, které se má generovat pro každý den." });
            list.Add(new PobockaIni() { Name = "pobocka_vazba_na_datum", Default = "1", Type = PobockaIniType.Bolean, Desc = "Standardní kontrola datumů vůči lekci" });

            list.Add(new PobockaIni() { Name = "lekceOdhlasPevne", Default = "1", Type = PobockaIniType.Bolean, Desc = "<b>Pevné</b> odlášení se váže na definovaný čas ten den nebo den předem bez <b>ohledu na začáteklekce</b> (např. do 22:00 den předem). V opačném případě se bude vázat na definovanou dobu před začátkem každé lekce (např. 7 hodin před začátekem každou lekcí).  " });
            list.Add(new PobockaIni() { Name = "lekceOdhlasPevneDenPred", Default = "1", Type = PobockaIniType.Int, Desc = "Počet dní před dnem, kdy je možné se odhlásit než se uskuteční lekce. 1=den předem / 0=v ten samý den" });
            list.Add(new PobockaIni() { Name = "lekceOdhlasPevneHodDne", Default = "23:59", Type = PobockaIniType.Int, Desc = "Čas ve zvoleném dni kdy je možné se ohlásit z lekce." });
            list.Add(new PobockaIni() { Name = "lekce_pevhod_prihlas_den", Default = "0", Type = PobockaIniType.Int, Desc = "Počet dní před dnem, kdy je se uskuteční lekce. 1=den předem / 0=v ten samý den" });
            list.Add(new PobockaIni() { Name = "lekce_pevhod_prihlas_cas", Default = "23:59", Type = PobockaIniType.Time, Desc = "Čas ve zvoleném dni kdy je možné se přihlásit z lekce." });


            list.Add(new PobockaIni() { Name = "lekceOdhlasPlov", Default = "0", Type = PobockaIniType.Bolean, Desc = "<b>Plovoucí</b> odlášení se váže na definovanou dobu před začátkem <b>každé lekce</b> (např. 7 hodin před začátekem každou lekcí). V opačném případě se bude vázat na den předem a konkrétní hodinu ve dni. (např. do 22:00 den předem)" });
            list.Add(new PobockaIni() { Name = "lekceOdhlasPlovHod", Default = "1", Type = PobockaIniType.Int, Desc = "Počet hodin před začátkem lekce/hodiny kdy je možné se odhlásit." });
            list.Add(new PobockaIni() { Name = "lekce_plovhod_prihlas", Default = "0", Type = PobockaIniType.Int, Desc = "Počet hodin před začátkem lekce/hodiny kdy je možné se přihlásit." });

            list.Add(new PobockaIni() { Name = "lekceUkazPocet", Default = "1", Type = PobockaIniType.Bolean, Desc = "Zobrazit počet zákazníků v lekci/hodině" });
            list.Add(new PobockaIni() { Name = "lekceUkazZakazniky", Default = "1", Type = PobockaIniType.Bolean, Desc = "Zobrazit jednotlivé zákazníky v lekci/hodině" });
            list.Add(new PobockaIni() { Name = "lekceUkazLimitOd", Default = "1", Type = PobockaIniType.Int, Desc = "Počet volných míst od kdy zobrazovat v rozvrhu 'více než'." });
            list.Add(new PobockaIni() { Name = "lekceOdhlasNabidniVolne", Default = "0", Type = PobockaIniType.Bolean, Desc = "Umožnit zákazníkovi nabídnout místo v lekci/hodině po termínu odhlášení." });

            list.Add(new PobockaIni() { Name = "rozvrhUkazPocet", Default = "0", Type = PobockaIniType.Bolean, Desc = "Zda se má v rozvrhu zobrazit počet voných míst." });
            list.Add(new PobockaIni() { Name = "rozvrhUkazTyp", Default = "0", Type = PobockaIniType.Bolean, Desc = "Zda se má v rozvrhu zobrazit Typ hodiny." });


            list.Add(new PobockaIni() { Name = "lekceDoMzdy", Default = "0", Type = PobockaIniType.Bolean, Desc = "Zda umožnit systému zadávat kteří zákazníci mají/nemají být započítáni do mzdy lektora." });
            list.Add(new PobockaIni() { Name = "lektor2", Default = "0", Type = PobockaIniType.Bolean, Desc = "Zda umožnit v systému zadávat druhého lektora v hodině." });
            list.Add(new PobockaIni() { Name = "maxKreditDoMinusu", Default = "10", Type = PobockaIniType.Int, Desc = "Počet kreditů, které může jít zákazník do mínusu." });
            list.Add(new PobockaIni() { Name = "maxKreditDoMinusu1", Default = "10", Type = PobockaIniType.Int, Desc = "Počet kreditů, které může jít zákazník do mínusu při první objednávce." });
            list.Add(new PobockaIni() { Name = "enable_spin_view_all_lesson", Default = "0", Type = PobockaIniType.Bolean, Desc = "Umožnit omezit lektorovi vidět jen své lekce." });

            list.Add(new PobockaIni() { Name = "lekce_max_count_per_user", Default = "0", Type = PobockaIniType.Int, Desc = "Maximální počet objednávek za jednoho uživatele." });

            list.Add(new PobockaIni() { Name = "public_lekce_ukaz_lektory", Default = "1", Type = PobockaIniType.Bolean, Desc = "Zobraz lektory na detailu lekce." });
            list.Add(new PobockaIni() { Name = "public_lekce_ukaz_typ", Default = "1", Type = PobockaIniType.Bolean, Desc = "Zobraz typ na detailu lekce." });
            list.Add(new PobockaIni() { Name = "public_lekce_ukaz_lektor_telefon", Default = "0", Type = PobockaIniType.Bolean, Desc = "Zobraz telefon lektora na detailu lekce." });
            list.Add(new PobockaIni() { Name = "public_lekce_ukaz_lektor_popis", Default = "0", Type = PobockaIniType.Bolean, Desc = "Zobraz popis lektora na detailu lekce." });

            list.Add(new PobockaIni() { Name = "public_rozvrh_ukaz_lektora", Default = "0", Type = PobockaIniType.Bolean, Desc = "Zobraz lektora v rozvrhu." });
            list.Add(new PobockaIni() { Name = "public_rozvrh_ukaz_kapacitu", Default = "0", Type = PobockaIniType.Bolean, Desc = "Zobraz možný počet zákazníků celkem." });
            list.Add(new PobockaIni() { Name = "public_rozvrh_ukaz_casy", Default = "1", Type = PobockaIniType.Bolean, Desc = "Zda se mají nad rozvrhem zobrazit časy." });


            list.Add(new PobockaIni() { Name = "lekce_uctuj_rychle", Default = "0", Type = PobockaIniType.Bolean, Desc = "Učtuj lekce i v nezaúčtovaných hodinách." });
            list.Add(new PobockaIni() { Name = "gate_pay", Default = "0", Type = PobockaIniType.Bolean, Desc = "Platební brány" });

            list.Add(new PobockaIni() { Name = "kredit_casovy_zobraz_od", Default = "0", Type = PobockaIniType.Bolean, Desc = "Zobrazit datum platnost od." });
            list.Add(new PobockaIni() { Name = "kredit_casovy_zobraz_do", Default = "0", Type = PobockaIniType.Bolean, Desc = "Zobrazit datum platnost do." });


            return list;
        }


        #endregion
        public void ClearMainIni()
        {
            string cacheItemKey = $"GetMainIni";
            cache.ClearDataSub(cacheItemKey);
        }

        public MainIniCover GetMainIni()
        {

            string cacheItemKey = $"GetMainIni";
            var data = cache.GetDataSub<MainIniCover>(cacheItemKey);

            if (data == null)
            {

                data = repMainIni.GetById(1);

                if (data != null)
                {
                    if (data.MainStyleObj == null)
                    {
                        data.SetDefaultStyles();
                    }
                    if (data.MainGatePaysObj == null)
                    {
                        data.SetDefaultPayGates();
                    }
                    if (data.BankAccountObj == null)
                    {
                        data.SetDefaultBankAccount();
                    }

                }
                else
                {
                    data = new MainIniCover();
                    data.SetDefault();
                    data.SetDefaultStyles();
                    data.SetDefaultPayGates();
                    data.SetDefaultBankAccount();

                    InsertMainIni(data);
                }
                cache.SetDataSub(cacheItemKey, data);
            }

            return data;

        }

        public void InsertMainIni(MainIniCover o)
        {
            repMainIni.Insert(o);
        }

        public void UpdateMainIni(MainIniCover o)
        {
            repMainIni.Update(o);

            ClearMainIni();
        }


        #endregion


        #region Ini minute

        public void ClearIniMinutes()
        {
            string cacheItemKey = $"GetIniMinutes";
            cache.ClearDataSub(cacheItemKey);
        }

        public IEnumerable<CalIniMinute> GetIniMinutes()
        {
            string cacheItemKey = $"GetIniMinutes";

            IEnumerable<CalIniMinute> list = cache.GetDataSub<IEnumerable<CalIniMinute>>(cacheItemKey);

            if (list == null)
            {
                list = repIniMin.TableUntracked.ToList();
                cache.SetDataSub(cacheItemKey, list);
            }

            return list;
        }

        public void Update(CalIniMinute o)
        {
            repIniMin.Update(o);
            ClearIniMinutes();
        }

        public void Update(IEnumerable<CalIniMinute> os)
        {
            foreach (var o in os)
            {
                repIniMin.Update(o);
            }
            ClearIniMinutes();
        }

        public void Insert(IEnumerable<CalIniMinute> os)
        {
            foreach (var o in os)
            {
                repIniMin.Insert(o);
            }
            ClearIniMinutes();
        }

        public void Delete(IEnumerable<CalIniMinute> os)
        {
            foreach (var o in os)
            {
                repIniMin.Delete(o);
            }
            ClearIniMinutes();
        }



        #endregion

    }
}
