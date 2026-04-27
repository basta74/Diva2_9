
using Diva2.Core.Main.PayGates;
using Diva2.Core.Main.Platby;
using Diva2.Core.Main.Trans;
using Diva2.Core.Model.Json;
using Diva2.Core.Model.Money;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Services.Managers.Platby
{
    public interface IPlatbaService
    {
        #region Kredit
        PlatbaKredit GetKreditById(int id);
        IList<PlatbaKredit> GetKreditAll(int pokId);

        void Insert(PlatbaKredit obj);
        void Update(PlatbaKredit obj);
        void Delete(PlatbaKredit obj);


        #endregion

        #region Casove

        PlatbaKreditCas GetKreditCasById(int id);
        IList<PlatbaKreditCas> GetKreditCasAll(int pokId);

        void Insert(PlatbaKreditCas obj);
        void Update(PlatbaKreditCas obj);
        void Delete(PlatbaKreditCas obj);

        #endregion

        #region Pausal
        PlatbaCas GetCasById(int id);
        IList<PlatbaCas> GetCasAll(int pokId);

        void Insert(PlatbaCas obj);
        void Update(PlatbaCas obj);
        void Delete(PlatbaCas obj);
        #endregion

        #region Pays.cz
        PaysItem GetPaysById(int id);

        IEnumerable<PaysItem> GetPaysByUserId(int id);

        IEnumerable<PaysItem> GetPaysByMonth(int month, int year);

        void Insert(PaysItem obj);
        void Update(PaysItem obj);
        #endregion

        UserTransakce AddMoney(AddMoneyTrans m, JsonAddMoney j);


    }
}
