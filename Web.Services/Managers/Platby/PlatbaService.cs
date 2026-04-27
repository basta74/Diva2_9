using Diva2.Core.Extensions;
using Diva2.Core.Main.PayGates;
using Diva2.Core.Main.Platby;
using Diva2.Core.Main.Trans;
using Diva2.Core.Model.Json;
using Diva2.Core.Model.Money;
using Diva2.Data;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diva2.Services.Managers.Platby
{
    public class PlatbaService : IPlatbaService
    {
        private readonly IRepository<PlatbaKredit> repKredit;
        private readonly IRepository<PlatbaKreditCas> repKreditCas;
        private readonly IRepository<PaysItem> repPays;
        
        public PlatbaService(ApplicationDbContext dbContext, IMemoryCache memoryCache,
            IRepository<PlatbaKredit> repositoryKr, IRepository<PlatbaKreditCas> repositoryKrCas, IRepository<PaysItem> rePa)
        {
          
            this.repKredit = repositoryKr;
            this.repKreditCas = repositoryKrCas;
            this.repPays = rePa;
        }

        #region Kredit

        public IList<PlatbaKredit> GetKreditAll(int pokId)
        {
            return repKredit.Table.Where(d => d.PokladnaId == pokId).ToList();
        }

        public PlatbaKredit GetKreditById(int id)
        {
            return repKredit.Table.Where(d => d.Id == id).FirstOrDefault();
        }
        public void Insert(PlatbaKredit obj)
        {
            repKredit.Insert(obj);
        }
        public void Update(PlatbaKredit obj)
        {
            repKredit.Update(obj);
        }

        public void Delete(PlatbaKredit obj)
        {
            repKredit.Delete(obj);
        }

        #endregion

        #region KreditCas

        public IList<PlatbaKreditCas> GetKreditCasAll(int pokId)
        {
            return repKreditCas.Table.Where(d => d.PokladnaId == pokId).ToList();
        }

        public PlatbaKreditCas GetKreditCasById(int id)
        {
            return repKreditCas.Table.Where(d => d.Id == id).FirstOrDefault();
        }


        public void Insert(PlatbaKreditCas obj)
        {
            repKreditCas.Insert(obj);
        }

        public void Update(PlatbaKreditCas obj)
        {
            repKreditCas.Update(obj);
        }

        public void Delete(PlatbaKreditCas obj)
        {
            repKreditCas.Delete(obj);
        }

        #endregion

        #region Cas

        public IList<PlatbaCas> GetCasAll(int PokId)
        {
            throw new NotImplementedException();
        }

        public PlatbaCas GetCasById(int id)
        {
            throw new NotImplementedException();
        }


        public void Insert(PlatbaCas obj)
        {
            throw new NotImplementedException();
        }
        public void Update(PlatbaCas obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(PlatbaCas obj)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region PaysCz
        public PaysItem GetPaysById(int id)
        {
            return repPays.GetById(id);
        }

        public IEnumerable<PaysItem> GetPaysByUserId(int id)
        {
            return repPays.TableUntracked.Where(d => d.UserId == id);
        }

        public IEnumerable<PaysItem> GetPaysByMonth(int year, int month)
        {
            int yn = Int32.Parse($"{year}{month}");
            return repPays.TableUntracked.Where(d => d.YearMonth == yn);
        }

        public void Insert(PaysItem obj)
        {
            repPays.Insert(obj);
        }

        public void Update(PaysItem obj)
        {
            repPays.Update(obj);
        }

        public UserTransakce AddMoney(AddMoneyTrans m, JsonAddMoney resp)
        {
            var tran = new UserTransakce();

            do
            {
                tran.UserId = m.UserId;
                tran.PokladnaId = m.PokladnaId;
                tran.ProvedlId = m.ProvedlId;
                tran.Datum = DateTime.Now;
                tran.UnixTime = DateTimeExtensions.ToUnix(DateTime.Now);
                tran.PlatnostOdUnix = null;
                tran.PlatnostDoUnix = null;
                tran.PlatnostOd = null;
                tran.PlatnostDo = null;



                tran.DoPokladny = m.DoPokladny;
                tran.ZBanky = m.ZBanky;
                tran.Status = m.Plus?"+":"-";

                string text = "";

               
               

                if (m.TypPlatbyId == 1 || m.TypPlatbyId == 2)
                {
                    var platba = GetKreditById(m.PlatbaId);
                    if (platba == null)
                    {
                        resp.Messages.Add(new JsonMessage() { Text = "Nenačetla se platba", Type = JsonMessageType.Danger });
                        break;
                    }

                    tran.Typ = "k";
                    tran.PlatbaId = m.PlatbaId;
                    tran.Kredit = platba.Kredity;
                    tran.Castka = platba.Castka;
                    
                    tran.IsOk = true;

                    text = $"Připsáno <b>{tran.Kredit}</b> kreditů za <b>{platba.Castka}</b> Kč";
                    resp.Messages.Add(new JsonMessage() { Text = text, Type = JsonMessageType.Success });
                   
                }
                else if (m.TypPlatbyId == 3 || m.TypPlatbyId == 4)
                {
                    var platba = GetKreditCasById(m.PlatbaId);
                    if (platba == null)
                    {
                        resp.Messages.Add(new JsonMessage() { Text = "Nenačetla se platba", Type = JsonMessageType.Danger });
                        break;
                    }

                    tran.Typ = "kc";
                    tran.PlatbaTime = platba;
                    tran.PlatbaId = platba.Id;
                    tran.Kredit = platba.Kredity;
                    tran.Castka = platba.Castka;
                    tran.Zbytek = platba.Kredity;

                    if (m.DateTo.HasValue)
                    {
                        if (m.DateFrom.HasValue)
                        {
                            tran.PlatnostOd = m.DateFrom.Value;
                            tran.PlatnostDo = m.DateTo.Value;
                        }
                        else
                        {
                            tran.PlatnostOd = m.DateFrom.Value.AddMonths(-platba.PocetMesicu);
                        }
                    }
                    else
                    {
                        if (m.DateFrom.HasValue)
                        {
                            tran.PlatnostOd = m.DateFrom.Value;
                            tran.PlatnostDo = m.DateFrom.Value.AddMonths(platba.PocetMesicu);
                            
                        }
                        else
                        {
                        }
                    }


                    text = $"Připsáno <b>{tran.Kredit}</b> časových kreditů za <b>{platba.Castka}</b> Kč na <b>{platba.PocetMesicu}</b> měsíců";
                    resp.Messages.Add(new JsonMessage() { Text = text, Type = JsonMessageType.Success });
                    tran.IsOk = true;
                }
                else if (m.TypPlatbyId == 5 || m.TypPlatbyId == 6)
                {
                    var platba = GetCasById(m.PlatbaId);
                    tran.Typ = "c";
                    tran.PlatbaPausal = platba;
                    tran.PlatbaId = platba.Id;
                    tran.Castka = platba.Castka;
                }

               
            } while (false);

            return tran;
        }
      
        #endregion


    }
}
