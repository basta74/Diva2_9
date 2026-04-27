using Diva2.Core;
using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Platby;
using Diva2.Core.Main.Trans;
using Diva2.Core.Main.Users;
using Diva2.Core.Main.Zakaznik;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diva2.Services.Managers.Platby
{
    public interface IObjednavkyService
    {
        IList<LekceUser> GetByLekce(int id, bool include = true);
        LekceUser GetBy(int userId, int lekceId, int kontCisloId);

        LekceUser GetById(int id, bool includeUser);

        void Insert(LekceUser obj);
        void Update(LekceUser obj);

        void Update(IEnumerable<LekceUser> objs);

        void Delete(LekceUser obj);

        List<KeyValuePair<string, int>> GetTrzbyRok(int pokladnaId, int year);

        List<KeyValuePair<string, int>> GetTrzbyMesic(int poklId, int rok, int mes);

        List<KeyValuePair<string, int>> GetTrzbyDen(int poklId, DateTime dt);

        IList<Zakaznik> GetByCountOrder(DateTime date);

        int GetPocetObjednavekUzivatele(int id);

        void ClearObjednaneLekceUzivatele(int id);

        public IList<LekceUser> GetObjednaneLekceUzivatele(int id);

        void ClearUskutecneneLekceUzivatele(int id);
        public IList<LekceUser> GetUskutecneneLekceUzivatele(int id);

        public IList<LekceUser> GetByDay(DateTime dt, int id);

        public void ClearZbytekVObjednavkachUzivatele(int id);

        public void ClearHistorieRoky(int id);

        public IList<int> GetHistorieRoky(int id);

        void ClearHistoriTransakci(int id);

        #region UserTransakce
        public IList<UserTransakce> GetHistoriTransakci(int id, bool readCache = true);

        public UserTransakce GetTransakciById(int id, bool includeOthers);

        public bool RemoveKredit(UserTransakce tran, int i, bool jitDoMinusu);

        public void AddKredit(UserTransakce tran);

        public void ApplyTreansaction(UserTransakce tran);

        public void RemoveKredit(UserTransakce tran);

        public void AddKreditTime(UserTransakce tran);

        public void AddPausal(UserTransakce tran);

        public void InsertTransactionStandard(UserTransakce t);

        public void InsertTransactionTime(UserTransakce t);
        #endregion

        void ClearZbytekUzivatele(int id);
        public UserZbytek GetZbytekUzivatele(int id, bool fromCache = true);

        int GetRandom(IList<LekceUser> obj);

        #region UserLekceChange
        public void AddUserChange(UserLekceChange ch);

        public IEnumerable<UserLekceChange> GetChangesByLesson(int id);

        void AddUserChangeLog(UserLekceLogOut chl);

        IPagedList<UserLekceLogOut> GetLogOutAll(int page, int PAGE_SIZE);

        void AddUserChangeLogIn(UserLekceLogIn chl);

        IPagedList<UserLekceLogIn> GetLogInAll(int page, int PAGE_SIZE);

        #endregion

        #region UserText
        void Insert(UserText obj);

        void Delete(UserText obj);

        public IList<UserText> GetUserTextByLesson(int lekceId);

        public UserText GetUserTextByLessonNumber(int userId, int lekceId, int number);
        #endregion

        #region PrijmoveDoklady

        PrijmoveDoklady GetValuPrijmoveDoklady(int year);

        void Insert(PrijmoveDoklady pd);
        void Update(PrijmoveDoklady pd);

        #endregion

        #region KreditCas

        bool ReturnKredit(int typPlatbyId, UserTransakce t);

        UserZbytekKreditCas GetUserCreditsTimeById(int id);

        void Update(UserZbytekKreditCas pla);


        void Insert(UserZbytekKreditCasLog log);

        IEnumerable<UserZbytekKreditCasLog> GetLogsByPlatbaId(int plaId);

        IEnumerable<UserZbytekKreditCasLog> GetUpFromDate(int from);


        #endregion

    }
}
