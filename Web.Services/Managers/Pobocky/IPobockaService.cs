using Diva2.Core;
using Diva2.Core.Main;
using Diva2.Core.Main.Calendar;
using Diva2.Core.Main.Main;
using Diva2.Core.Main.Pobocky;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Services.Managers.Pobocky
{
    public interface IPobockaService
    {
        void ClearPobocky();
        IList<Pobocka> GetPobocky();

        void ClearZacatky();

        IList<CasZacatek> GetZacatky();

        void Insert(List<CasZacatek> insertCasy);

        #region Ini

        public void ClearPobockaInis(int pobId);
        public IList<PobockaIni> GetPobockaInis(int pobId);

        public PobockaIni GetIni(int pobId, string key);

        public List<PobockaIni> GetDefaults();

        void Update(PobockaIni ini);
        #endregion

        #region MainIni
        void ClearMainIni();
        MainIniCover GetMainIni();

        void UpdateMainIni(MainIniCover o);

        //void InsertMainIni(MainIniCover o);
        #endregion

        #region Ini minutes

        void ClearIniMinutes();
        IEnumerable<CalIniMinute> GetIniMinutes();

        void Update(CalIniMinute o);

        void Update(IEnumerable<CalIniMinute> o);

        void Insert(IEnumerable<CalIniMinute> o);

        void Delete(IEnumerable<CalIniMinute> o);
        Company GetCompany();


        #endregion
    }
}
